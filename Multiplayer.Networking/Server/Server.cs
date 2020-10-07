using Multiplayer.Debugging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Multiplayer.Networking
{
    public static partial class Server
    {       
        public static string ServerName = "";
        public static string Password = "";
        public static ushort MaxPlayers = 10;
        public static ushort Port;
        public static int Difficulty;
        public static bool hasAI = false;
        public static bool IsRunning = false;

        public static Telepathy.Server server = new Telepathy.Server();     
            
        /// <summary>
        /// Starts the server, use Server.Stop() to stop it.
        /// </summary>
        /// <param name="port">The port the server will listening on</param>
        public static void Start(ushort port, string password = "")
        {
            if (server.Active)
            {
                Logging.Warn("[Server] You can't start the server because its already active!");
                //WindowManager.SpawnDialog("You can't start the server because its already active!", true, DialogWindow.DialogType.Warning);
                return;
            }
            Serverdata = new ServerData(port.ToString());
            Port = port;
            Password = password;
            Difficulty = GameSettings.Instance.Difficulty;
            Logging.Info("[Server] Start listening on Port " + port);
            server.MaxMessageSize = int.MaxValue;
            server.Start(port);
            IsRunning = true;
            Serverdata.UpdateServer();
            Read();
        }

        /// <summary>
        /// Gets called after the server is started and stays in a loop until 'isRunning' is false (If you call Server.Stop() for example)
        /// </summary>
        private static async void Read()
        {
            Logging.Info("[Server] Start reading messages");
            await Task.Run(() =>
            {
                while (IsRunning)
                {
                    // grab all new messages. do this in your Update loop.
                    while (server.GetNextMessage(out Telepathy.Message msg))
                    {
                        switch (msg.eventType)
                        {
                            case Telepathy.EventType.Connected:
                                OnUserConnect(msg);
                                break;
                            case Telepathy.EventType.Data:
                                Receive(msg);
                                break;
                            case Telepathy.EventType.Disconnected:
                                OnUserDisconnect(msg);
                                break;
                        }
                    }
                }
            });
            Logging.Info("[Server] End reading messages");
        }   

        #region Messages
        public static void Send(TcpServerChat tcpServerChat)
        {
            Logging.Info("[Server] Sending server wide message: " + (string)tcpServerChat.Data.GetValue("message"));
            foreach (Helpers.User user in Users)
            {
                server.Send(user.ID, tcpServerChat.Serialize());
            }
        }

        public static void Send(int clientid, TcpRequest request)
        {
            Logging.Info("[Server] Sending request to client " + clientid);
            //server.Send(clientid, request.ToArray());
            server.Send(clientid, request.Serialize());
        }

        public static void Send(int clientid, TcpResponse response)
        {
            Logging.Info("[Server] Sending response to client " + clientid);
            //server.Send(clientid, response.ToArray());
            server.Send(clientid, response.Serialize());
        }

        public static void Send(int clientid, TcpChat message)
        {
            int receiver = (int)message.Data.GetValue("receiver");
            string msg = (string)message.Data.GetValue("message");
            Logging.Info("[Server] Redirecting Chat message from " + clientid + " to " + receiver);
            server.Send(receiver, new TcpChat(msg, GetUser(clientid)).Serialize());
        }

        public static void Send(int clientid, TcpGamespeed speed)
        {
            Logging.Info("[Server] Sending GameSpeed to client " + clientid);
            server.Send(clientid, speed.Serialize());
        }

        public static void Send(TcpGamespeed speed)
        {
            Logging.Info("[Server] Sending GameSpeed to all clients");
            foreach (Helpers.User user in Users)
            {
                server.Send(user.ID, speed.Serialize());
            }
        }
        #endregion

        /// <summary>
        /// Gets called whenever a message (other than connect or disconnect) gets received
        /// </summary>
        /// <param name="msg">The Telepathy.Message sent by the server.getNextMessage() function</param>
        private static void Receive(Telepathy.Message msg)
        {
            //string datastr = Encoding.UTF8.GetString(msg.data);
            //Logging.Info($"[Server] From Connection {msg.connectionId}: " + datastr);
            Logging.Info($"[Server] Data from Connection {msg.connectionId}: {msg.data.Length} bytes");

            //Handle TCPLogin
            //TcpLogin tcplogin = XML.From<TcpLogin>(datastr);
            TcpLogin tcplogin = TcpLogin.Deserialize(msg.data);
            if (tcplogin != null && tcplogin.Header == "login")
            {
                OnUserLogin(msg.connectionId, tcplogin);
            }

            //Handle TCPChat
            //TcpChat tcpchat = XML.From<TcpChat>(datastr);
            TcpChat tcpchat = TcpChat.Deserialize(msg.data);
            if (tcpchat != null && tcpchat.Header == "chat")
            {
                OnUserChat(msg.connectionId, tcpchat);
            }
            //HandleTcpPrivateChat 
            TcpPrivateChat tcpPrivateChat = TcpPrivateChat.Deserialize(msg.data);
            if (tcpPrivateChat != null && tcpPrivateChat.Header == "pm")
            {
                OnPrivateChat(tcpPrivateChat);
            }
            //Handle TCPRequests
            //TcpRequest tcprequest = XML.From<TcpRequest>(datastr);
            TcpRequest tcprequest = TcpRequest.Deserialize(msg.data);
            if (tcprequest != null && tcprequest.Header == "request")
            {
                string req = (string)tcprequest.Data.GetValue("request");
                if (req == "gameworld")
                {
                    OnRequestGameWorld(msg.connectionId);
                }
                else if (req == "userlist")
                {
                    OnRequestUserList(msg.connectionId);
                }
            }

            TcpGamespeed tcpspeed = TcpGamespeed.Deserialize(msg.data);
            if (tcpspeed != null && tcpspeed.Header == "gamespeed")
            {
                OnGamespeedChange(msg.connectionId, tcpspeed);
            }
        }

        private static void OnUserChat(int connectionid, TcpChat chat)
        {
            if ((Helpers.User)chat.Data.GetValue("receiver") != null)
            {
                Helpers.User user = GetUser(((Helpers.User)chat.Data.GetValue("receiver")).Username);
                //Send to a receiver
                Logging.Info($"[Server] User {((Helpers.User)chat.Data.GetValue("sender")).Username} sends a chat to {user.Username}");
                server.Send(user.ID, chat.Serialize());
                return;
            }
            //Send to all connected users
            Logging.Info($"[Server] User {connectionid} sends a chat to all connected users");
            foreach (Helpers.User u in Users)
            {
                if (u.ID != connectionid)
                {
                    server.Send(u.ID, chat.Serialize());
                }
            }
        }

        private static void OnRequestGameWorld(int connectionid)
        {
            Logging.Info("[Server] Sending GameWorld to user " + connectionid);
            GameWorld.Server.Instance.UpdateClient(GetUser(connectionid));
        }

        private static void OnRequestUserList(int connectionid)
        {
            Logging.Info("[Server] Sending Userlist to user " + connectionid);
            Logging.Warn("[Server] Can't send Userlist because there is an exception, this problem is known!");
            //TcpResponse response = new TcpResponse("userlist", new XML.XMLDictionary(new XML.XMLDictionaryPair("users", Users.ToArray())));
            //server.Send(connectionid, response.ToArray());
        }

        private static void OnGamespeedChange(int connectionid, TcpGamespeed speed)
        {
            if ((int)speed.Data.GetValue("type") == 0)
            {
                Logging.Info($"[Server] Sending updated GameSpeed to all clients => {(int)speed.Data.GetValue("speed")} usercount: {Users.Count}");
                foreach (Helpers.User u in Users)
                {
                    Logging.Info($"[Server] Sent GameSpeed to connection {u.ID}");
                    Send(u.ID, speed);
                }
            }
            else
            {
                Logging.Warn($"[Server] User {connectionid} can't change gamespeed if type is 1 (vote) because votes aren't included yet");
            }
        }

        /// <summary>
        /// Stops the Server
        /// </summary>
        public static async void Stop()
        {
            if (!IsRunning)
            {
                Logging.Warn("[Server] Can't stop a Server that isn't running...");
                return;
            }
            await Task.Run(() => Send(new TcpServerChat($"The server has been stopped and you have been disconnected from it.", TcpServerChatType.Warn)));
            Logging.Info("[Server] Stop listening");
            IsRunning = false;
            Serverdata.SaveData(null, null);
            server.Stop();
            Users.Clear();
        }

           
    }
}