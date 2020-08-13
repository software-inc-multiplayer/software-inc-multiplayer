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


        public static void Start(ushort port)
		{
            Port = port;
            // create and start the server
            Logging.Info("[Server] Start listening on Port " + port);
            server.Start(port);
            isRunning = true;
            Read();
        }

        public static async void Read()
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
                                Logging.Info("[Server] " + msg.connectionId + " Connected");
                                break;
                            case Telepathy.EventType.Data:
                                Receive(msg.connectionId, msg.data);
                                break;
                            case Telepathy.EventType.Disconnected:
                                Logging.Info("[Server] " + msg.connectionId + " Disconnected");
                                break;
                        }
                    }
                }
            });
            Logging.Info("[Server] End reading messages");
        }

        public static void Receive(int connectionid, byte[] data)
        {
            Logging.Info($"[Server] From Connection {connectionid}:" + Encoding.UTF8.GetString(data));
        }

        public static void Stop()
		{
            Logging.Info("[Server] Stop listening");
            isRunning = false;
            server.Stop();
		}
    }
}
