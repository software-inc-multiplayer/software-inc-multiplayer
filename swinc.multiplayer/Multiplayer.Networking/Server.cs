using Multiplayer.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            server.Send(msg.connectionId,new Helpers.TcpResponse("login_request", "").ToArray());
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
        }

        public static void Send(int clientid, Helpers.TcpGameWorld changes)
        {

        }

        public static void Send(Helpers.TcpGameWorld changes)
        {

        }
        #endregion

        /// <summary>
        /// Gets called whenever a message (other than connect or disconnect) gets received
        /// </summary>
        /// <param name="msg">The Telepathy.Message sent by the server.getNextMessage() function</param>
        static void Receive(Telepathy.Message msg)
        {
            Logging.Info($"[Server] From Connection {msg.connectionId}:" + Encoding.UTF8.GetString(msg.data));
        }

        /// <summary>
        /// Stops the Server
        /// </summary>
        public static void Stop()
		{
            Logging.Info("[Server] Stop listening");
            isRunning = false;
            server.Stop();
		}
    }
}

//INStalLer CraSHES IF NO INTERNET CONNECTiON