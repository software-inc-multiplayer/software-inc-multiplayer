using Multiplayer.Debugging;
using RoWa;
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
        static string Username = "Player";
        static string ServerPassword = "test";

        public static void Connect(string ip, ushort port)
        {
            // create and connect the client
            client.Connect(ip, port);
            isRunning = true;
            Read();
        }

        static async void Read()
		{
            Logging.Info("[Client] Starts reading");
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
            Logging.Info("[Client] Ends reading");
        }

        static void Receive(byte[] data)
		{
            string datastr = Encoding.UTF8.GetString(data);
            Logging.Info("[Client] Data from Server: " + datastr);
            Helpers.TcpResponse tcpresponse = XML.From<Helpers.TcpResponse>(datastr);
            if (tcpresponse != null && tcpresponse.Header == "response")
                OnServerResponse(tcpresponse);
        }

        static void OnServerResponse(Helpers.TcpResponse response)
		{
            object type = response.Data.GetValue("type");
            if(type == null)
			{
                Logging.Warn("[Client] Type is null!");
                return;
			}
            if((string)type == "login_request")
			{
                Send(new Helpers.TcpLogin(Username, ServerPassword));
			}
		}

		#region Messages
		public static void Send(Helpers.TcpLogin login)
		{
            Logging.Info("[Client] Sending login message");
            client.Send(login.ToArray());
		}

        public static void Send(Helpers.TcpGameWorld changes)
		{
            Logging.Info("[Client] Sending gameworld update");
            client.Send(changes.ToArray());
		}

        public static void Send(Helpers.TcpChat chatmsg)
		{
            Logging.Info("[Client] Sending chat message");
            client.Send(chatmsg.ToArray());
		}
		#endregion

		public static void Disconnect()
		{
            isRunning = false;
            client.Disconnect();
		}
	}
}
