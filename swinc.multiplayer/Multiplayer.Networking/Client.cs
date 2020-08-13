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
        public static bool Connected { get { return client.Connected; } }
        static Telepathy.Client client = new Telepathy.Client();
        static string Username = "Player";
        static string ServerPassword = "";

        public static async void Connect(string ip, ushort port)
        {
            // create and connect the client
            client.Connect(ip, port);
            Logging.Info("[Client] Trying to connect!");
            await Task.Run(() => {
                while (client.Connecting)
                {

                }
                if (client.Connected)
                {
                    Logging.Info("[Client] Connected to the Server!");
                    Read();
                }
                else
                {
                    Logging.Warn("[Client] Couldn't connect to the Server");
                }
            });
            
        }

        static async void Read()
		{
            Logging.Info("[Client] Starts reading");
            await Task.Run(() => {
                while (Connected)
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

            //Handle TcpResponse
            Helpers.TcpResponse tcpresponse = XML.From<Helpers.TcpResponse>(datastr);
            if (tcpresponse != null && tcpresponse.Header == "response")
                OnServerResponse(tcpresponse);

            //Handle TcpChat
            Helpers.TcpChat tcpchat = XML.From<Helpers.TcpChat>(datastr);
            if (tcpchat != null && tcpchat.Header == "chat")
                OnChatReceived(tcpchat);
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

        static void OnChatReceived(Helpers.TcpChat chat)
		{
            Helpers.User sender = (Helpers.User)chat.Data.GetValue("sender");
            if (sender == null)
                sender = new Helpers.User() { Username = "Server" };
            Logging.Info("[Client] Chat received from " + sender.Username + ": " + (string)chat.Data.GetValue("message"));
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

        public static void Send(Helpers.TcpRequest request)
		{
            Logging.Info("[Client] Sending request");
            client.Send(request.ToArray());
		}

        public static void Send(Helpers.TcpResponse response)
		{
            Logging.Info("[Client] Sending response");
            client.Send(response.ToArray());
		}
		#endregion

		public static void Disconnect()
		{
            if (!Connected)
            {
                Logging.Warn("[Client] You can't disconnect a client that isn't connected...");
                return;
			}
            client.Disconnect();
		}
	}
}
