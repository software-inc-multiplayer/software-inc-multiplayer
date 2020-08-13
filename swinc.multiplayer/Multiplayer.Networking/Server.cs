using Multiplayer.Debugging;
using RoWa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using UnityEngine;

namespace Multiplayer.Networking
{
	public static class Server
    {
        public static List<Helpers.User> Users = new List<Helpers.User>();
        public static string ServerName = "My Server";
        public static string Password = "";
        public static ushort MaxPlayers = 10;
        public static ushort Port;
        static Telepathy.Server server = new Telepathy.Server();
        static bool isRunning = false;

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
            Logging.Info("[Server] Start listening on Port " + port);
            server.Start(port);
            isRunning = true;
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
        public static void Send(int clientid, Helpers.TcpResponse response)
		{
            Logging.Info("[Server] Sending response to client " + clientid);
            server.Send(clientid, response.ToArray());
        }

        public static void Send(int clientid, Helpers.TcpGameWorld changes)
        {
            Logging.Info("[Server] Sending GameWorldChanges to client " + clientid);
            server.Send(clientid, changes.ToArray());
        }

        public static void Send(Helpers.TcpGameWorld changes)
        {
            Logging.Info("[Server] Sending GameWorldChanges to all clients");
            foreach(Helpers.User user in Users)
			{
                server.Send(user.ID, changes.ToArray());
			}
        }

        public static void Send(int clientid, Helpers.TcpChat message)
		{
            int receiver = (int)message.Data.GetValue("receiver");
            string msg = (string)message.Data.GetValue("message");
            Logging.Info("[Server] Redirecting Chat message from " + clientid + " to " + receiver);
            server.Send(receiver, new Helpers.TcpChat(receiver, msg, clientid).ToArray());
		}
        #endregion

        /// <summary>
        /// Gets called whenever a message (other than connect or disconnect) gets received
        /// </summary>
        /// <param name="msg">The Telepathy.Message sent by the server.getNextMessage() function</param>
        static void Receive(Telepathy.Message msg)
        {
            string datastr = Encoding.UTF8.GetString(msg.data);
            Logging.Info($"[Server] From Connection {msg.connectionId}: " + datastr);

            //Handle TCPLogin
            Helpers.TcpLogin tcplogin = XML.From<Helpers.TcpLogin>(datastr);
            if (tcplogin != null && tcplogin.Header == "login")
                OnUserLogin(msg.connectionId, tcplogin);

            //Handle TCPChat
            Helpers.TcpChat tcpchat = XML.From<Helpers.TcpChat>(datastr);
            if (tcpchat != null && tcpchat.Header == "chat")
                OnUserChat(msg.connectionId, tcpchat);
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
                Send(connectionid, new Helpers.TcpResponse("login_response", "ok"));
                Users.Add(new Helpers.User()
                {
                    ID = connectionid,
                    Role = Users.Count < 1 ? Helpers.UserRole.Host : Helpers.UserRole.Client,
                    UniqueID = (string)login.Data.GetValue("uniqueid"),
                    Username = (string)login.Data.GetValue("username")
                });
                Logging.Info($"[Server] User logged in!");
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
                //Send to all connected users
                Logging.Info($"[Server] User {connectionid} sends a chat to all connected users");
                foreach (Helpers.User u in Users)
                    server.Send(u.ID, chat.ToArray());
			}else
			{
                //Send to a receiver
                Logging.Info($"[Server] User {connectionid} sends a chat to {(int)chat.Data.GetValue("receiver")}");
                server.Send((int)chat.Data.GetValue("receiver"), chat.ToArray());
			}
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
		}
    }
}