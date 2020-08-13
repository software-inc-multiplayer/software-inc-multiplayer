using Multiplayer.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiplayer.Networking
{
	public static class Client
	{
        static Telepathy.Client client = new Telepathy.Client();
        static bool isRunning = false;

        public static void Connect(string ip, ushort port)
        {
            // create and connect the client
            client.Connect(ip, port);
            isRunning = true;
            Read();
        }

        public static async void Read()
		{
            await Task.Run(() => {
                while (isRunning)
                {
                    // grab all new messages. do this in your Update loop.
                    Telepathy.Message msg;
                    while (client.GetNextMessage(out msg))
                    {
                        switch (msg.eventType)
                        {
                            case Telepathy.EventType.Connected:
                                Logging.Info("[Client] Connected");
                                break;
                            case Telepathy.EventType.Data:
                                Receive(msg.data);
                                break;
                            case Telepathy.EventType.Disconnected:
                                Logging.Info("[Client] Disconnected");
                                break;
                        }
                    }
                }
            });
        }

        public static void Receive(byte[] data)
		{
            Logging.Info("[Client] Data from Server: " + Encoding.UTF8.GetString(data));
		}

        public static void Send(Helpers.TcpLogin login)
		{
            client.Send(login.ToArray());
		}

        public static void Disconnect()
		{
            isRunning = false;
            client.Disconnect();
		}
	}
}
