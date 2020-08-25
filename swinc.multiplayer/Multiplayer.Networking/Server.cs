using Multiplayer.Debugging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Multiplayer.Networking
{
	public static class Server
    {
        public static List<Helpers.User> Users = new List<Helpers.User>();
        public static string ServerName = "My Server";
        public static string Password = "";
        public static ushort MaxPlayers = 10;
        public static ushort Port;
        public static int Difficulty;
        public static bool hasAI = false;
        static Telepathy.Server server = new Telepathy.Server();
        static ServerData serverdata = new ServerData("test");
        static bool isRunning = false;
        public static bool Runs { get { return isRunning; } }

        //gets fired if the server wants to save data.
        public static EventHandler OnSavingServer;

        /// <summary>
        /// Starts the server, use Server.Stop() to stop it.
        /// </summary>
        /// <param name="port">The port the server will listening on</param>
        public static void Start(ushort port)
		{
            if(server.Active)
			{
                Logging.Warn("[Server] You can't start the server because its already active!");
                return;
			}
            Port = port;
            Difficulty = GameSettings.Instance.Difficulty;
            Logging.Info("[Server] Start listening on Port " + port);
            server.MaxMessageSize = int.MaxValue;
            server.Start(port);
            isRunning = true;
            serverdata.UpdateServer();
            Read();
        }

        /// <summary>
        /// Gets called after the server is started and stays in a loop until 'isRunning' is false (If you call Server.Stop() for example)
        /// </summary>
        static async void Read()
		{
            Logging.Info("[Server] Start reading messages");
            await Task.Run(() => { 
                while(isRunning)
				{
                    // grab all new messages. do this in your Update loop.
                    Telepathy.Message msg;
                    while (server.GetNextMessage(out msg))
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

        /// <summary>
        /// Gets called whenever an user connects to the server
        /// </summary>
        /// <param name="msg">The Telepathy.Message sent by the server.getNextMessage() function</param>
        static void OnUserConnect(Telepathy.Message msg)
		{
            Logging.Info("[Server] " + msg.connectionId + " Connected");

            //Send a ServerResponse to the user to login
            Send(msg.connectionId,new Helpers.TcpResponse("login_request", ""));
        }

        /// <summary>
        /// Gets called whenever an user disconnects to the server
        /// </summary>
        /// <param name="msg">The Telepathy.Message sent by the server.getNextMessage() function</param>
        static void OnUserDisconnect(Telepathy.Message msg)
		{
            Logging.Info("[Server] " + msg.connectionId + " Disconnected");

            //Check if the user with the connectionid exists and if so delete him from the database
            Helpers.User user = Users.Find(x => x.ID == msg.connectionId);
            if (user == null)
                Logging.Warn($"[Server] User with the Id {msg.connectionId} did disconnect but no User object was found for him!");
            else
            {
                Logging.Info($"[Server] User {user.Username} did disconnect from the Server!");
                Users.Remove(user);
            }
        }

		#region Messages
        public static void Send(int clientid, Helpers.TcpRequest request)
		{
            Logging.Info("[Server] Sending request to client " + clientid);
            //server.Send(clientid, request.ToArray());
            server.Send(clientid, request.Serialize());
		}

        public static void Send(int clientid, Helpers.TcpResponse response)
		{
            Logging.Info("[Server] Sending response to client " + clientid);
            //server.Send(clientid, response.ToArray());
            server.Send(clientid, response.Serialize());
        }

        public static void Send(int clientid, Helpers.TcpGameWorld changes)
        {
            Logging.Info("[Server] Sending GameWorldChanges to client " + clientid + " => " + (bool)changes.Data.GetValue("addition"));
            server.Send(clientid, changes.Serialize());
        }

        public static void Send(Helpers.TcpGameWorld changes)
        {
            Logging.Info("[Server] Sending GameWorldChanges to all clients");
            foreach(Helpers.User user in Users)
			{
                server.Send(user.ID, changes.Serialize());
			}
        }

        public static void Send(int clientid, Helpers.TcpChat message)
		{
            int receiver = (int)message.Data.GetValue("receiver");
            string msg = (string)message.Data.GetValue("message");
            Logging.Info("[Server] Redirecting Chat message from " + clientid + " to " + receiver);
            server.Send(receiver, new Helpers.TcpChat(msg, GetUser(clientid)).Serialize());
		}

        public static void Send(int clientid, Helpers.TcpGamespeed speed)
		{
            Logging.Info("[Server] Sending GameSpeed to client " + clientid);
            server.Send(clientid, speed.Serialize());
		}

        public static void Send(Helpers.TcpGamespeed speed)
		{
            Logging.Info("[Server] Sending GameSpeed to all clients");
            foreach(Helpers.User user in Users)
			{
                server.Send(user.ID, speed.Serialize());
			}
		}
        #endregion

        /// <summary>
        /// Gets called whenever a message (other than connect or disconnect) gets received
        /// </summary>
        /// <param name="msg">The Telepathy.Message sent by the server.getNextMessage() function</param>
        static void Receive(Telepathy.Message msg)
        {
            //string datastr = Encoding.UTF8.GetString(msg.data);
            //Logging.Info($"[Server] From Connection {msg.connectionId}: " + datastr);
            Logging.Info($"[Server] Data from Connection {msg.connectionId}: {msg.data.Length} bytes");

            //Handle TCPLogin
            //Helpers.TcpLogin tcplogin = XML.From<Helpers.TcpLogin>(datastr);
            Helpers.TcpLogin tcplogin = Helpers.TcpLogin.Deserialize(msg.data);
            if (tcplogin != null && tcplogin.Header == "login")
                OnUserLogin(msg.connectionId, tcplogin);

            //Handle TCPChat
            //Helpers.TcpChat tcpchat = XML.From<Helpers.TcpChat>(datastr);
            Helpers.TcpChat tcpchat = Helpers.TcpChat.Deserialize(msg.data);
            if (tcpchat != null && tcpchat.Header == "chat")
                OnUserChat(msg.connectionId, tcpchat);

            //Handle TCPRequests
            //Helpers.TcpRequest tcprequest = XML.From<Helpers.TcpRequest>(datastr);
            Helpers.TcpRequest tcprequest = Helpers.TcpRequest.Deserialize(msg.data);
            if (tcprequest != null && tcprequest.Header == "request")
			{
                string req = (string)tcprequest.Data.GetValue("request");
                if (req == "gameworld")
                    OnRequestGameWorld(msg.connectionId);
                else if (req == "userlist")
                    OnRequestUserList(msg.connectionId);
			}

            Helpers.TcpGamespeed tcpspeed = Helpers.TcpGamespeed.Deserialize(msg.data);
            if (tcpspeed != null && tcpspeed.Header == "gamespeed")
                OnGamespeedChange(msg.connectionId, tcpspeed);
        }

        /// <summary>
        /// Gets called when the Header of Receive is "login"
        /// </summary>
        /// <param name="connectionid">The connection id of the Telepathy.Message</param>
        /// <param name="login">The Helpers.TcpLogin</param>
        static void OnUserLogin(int connectionid, Helpers.TcpLogin login)
        {
            Logging.Info($"[Server] User {login.Data.GetValue("username")} ({connectionid}) tries to login to the server");
            if (Users.Count >= MaxPlayers)
            {
                Logging.Info($"[Server] User {connectionid} tries login but max users reached");
                Send(connectionid, new Helpers.TcpResponse("login_response", "max_players"));
            }
            else if (Password != (string)login.Data.GetValue("password"))
			{
                Logging.Info($"[Server] User {connectionid} tries login with password {(string)login.Data.GetValue("password")} but pass is {Password}");
                Send(connectionid, new Helpers.TcpResponse("login_response", "wrong_password"));
            }
            else if (Users.Count < MaxPlayers && Password == (string)login.Data.GetValue("password"))
			{
                Send(connectionid, new Helpers.TcpResponse("login_response", connectionid + ""));
                Users.Add(new Helpers.User()
                {
                    ID = connectionid,
                    Role = Users.Count < 1 ? Helpers.UserRole.Host : Helpers.UserRole.Client,
                    UniqueID = (string)login.Data.GetValue("uniqueid"),
                    Username = (string)login.Data.GetValue("username")
                });
                Logging.Info($"[Server] User {(string)login.Data.GetValue("username")} logged in!");
                return;
            }
            else
                Logging.Warn($"[Server] Invalid TcpLogin data!");
            Logging.Warn("[Server] User didn't login, check client for details");
		}

        static void OnUserChat(int connectionid, Helpers.TcpChat chat)
		{
            if(chat.Data.GetValue("receiver") == null)
			{
                Helpers.TcpChat chatmsg = new Helpers.TcpChat((string)chat.Data.GetValue("message"), GetUser(connectionid));
                //Send to all connected users
                Logging.Info($"[Server] User {connectionid} sends a chat to all connected users");
                foreach (Helpers.User u in Users)
                    if(u.ID != connectionid)
                        server.Send(u.ID, chatmsg.Serialize());
			}else
            {
                Helpers.TcpChat chatmsg = new Helpers.TcpChat((string)chat.Data.GetValue("message"), GetUser(connectionid));
                //Send to a receiver
                Logging.Info($"[Server] User {connectionid} sends a chat to {(int)chatmsg.Data.GetValue("receiver")}");
                server.Send((int)chatmsg.Data.GetValue("receiver"), chatmsg.Serialize());
			}
		}

        static void OnRequestGameWorld(int connectionid)
		{
            Logging.Info("[Server] Sending GameWorld to user " + connectionid);
            GameWorld.Server.Instance.UpdateClient(GetUser(connectionid));
		}

        static void OnRequestUserList(int connectionid)
		{
            Logging.Info("[Server] Sending Userlist to user " + connectionid);
            Logging.Warn("[Server] Can't send Userlist because there is an exception, this problem is known!");
            //Helpers.TcpResponse response = new Helpers.TcpResponse("userlist", new XML.XMLDictionary(new XML.XMLDictionaryPair("users", Users.ToArray())));
            //server.Send(connectionid, response.ToArray());
		}

        static void OnGamespeedChange(int connectionid, Helpers.TcpGamespeed speed)
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
                Logging.Warn($"[Server] User {connectionid} can't change gamespeed if type is 1 (vote) because votes aren't included yet");
		}

        /// <summary>
        /// Stops the Server
        /// </summary>
        public static void Stop()
		{
            if(!isRunning)
			{
                Logging.Warn("[Server] Can't stop a Server that isn't running...");
                return;
			}
            Logging.Info("[Server] Stop listening");
            isRunning = false;
            server.Stop();
            Users.Clear();
		}

        /// <summary>
        /// Saves the server by firing the OnSavingServer event
        /// </summary>
        public static void Save()
		{
            OnSavingServer?.Invoke(null, null);
		}

        /// <summary>
        /// Returns the user with the ID id
        /// </summary>
        /// <param name="id">The 'connectionId' of the user</param>
        /// <returns>A Helpers.User object representing an user or null</returns>
        public static Helpers.User GetUser(int id)
		{
            return Users.Find(x => x.ID == id);
		}

        /// <summary>
        /// Returns the user with the Unique ID
        /// </summary>
        /// <param name="uniqueid">The User.UniqueId generated by GetUniqueID</param>
        /// <returns>A Helpers.user object representing an user or null</returns>
        public static Helpers.User GetUserByUnique(string uniqueid)
		{
            return Users.Find(x => x.UniqueID == uniqueid);
		}

        /// <summary>
        /// Returns the user with the username
        /// </summary>
        /// <param name="username">The username of the user</param>
        /// <returns>A Helpers.User object representing an user or null</returns>
        public static Helpers.User GetUser(string username)
		{
            return Users.Find(x => x.Username == username);
		}
    }
}