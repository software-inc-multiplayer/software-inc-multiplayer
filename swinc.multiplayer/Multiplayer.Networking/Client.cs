using Multiplayer.Debugging;
using System;
using System.Threading.Tasks;

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
			try
			{
                Username = Steamworks.SteamFriends.GetPersonaName();
            }
            catch(Exception ex)
			{
                Logging.Warn("[Client] Couldn't fetch username from Steam! If you've a DRM-Free version thats why. => " + ex.Message);
			}
            client.MaxMessageSize = int.MaxValue;
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
                    GameWorld.Client client = new GameWorld.Client();                 
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
            Logging.Info("[Client] Data from Server: " + data.Length + " bytes");

            //Handle TcpResponse
            Helpers.TcpResponse tcpresponse = Helpers.TcpResponse.Deserialize(data);
            if (tcpresponse != null && tcpresponse.Header == "response")
                OnServerResponse(tcpresponse);

            //Handle TcpChat
            Helpers.TcpChat tcpchat = Helpers.TcpChat.Deserialize(data);
            if (tcpchat != null && tcpchat.Header == "chat")
                Chat.RecieveMessage(tcpchat);
            //Handle GameWorld
            Helpers.TcpGameWorld tcpworld = Helpers.TcpGameWorld.Deserialize(data);
            if (tcpworld != null && tcpworld.Header == "gameworld")
                OnGameWorldReceived(tcpworld);

            //Handle Gamespeed
            Helpers.TcpGamespeed tcpspeed = Helpers.TcpGamespeed.Deserialize(data);
            if (tcpspeed != null && tcpspeed.Header == "gamespeed")
                OnGamespeedChange(tcpspeed);
        }

		private static void OnGamespeedChange(Helpers.TcpGamespeed tcpspeed)
		{
            Logging.Info("gamespeedchange...");
            int type = (int)tcpspeed.Data.GetValue("type");
            int speed = (int)tcpspeed.Data.GetValue("speed");
            if(type == 0)
			{
				HUD.Instance.GameSpeed = (int)speed;
			}
        }
        static void OnServerResponse(Helpers.TcpResponse response)
        {
            object type = response.Data.GetValue("type");
            if (type == null)
            {
                Logging.Warn("[Client] Type is null!");
                return;
            }
            if ((string)type == "login_request")
            {
                Send(new Helpers.TcpLogin(Username, ServerPassword));
            }
            else if ((string)type == "login_response")
            {
                string res = (string)response.Data.GetValue("data");
                if (res == "ok")
                {
                    //Login ok
                    Logging.Info("[Client] You're logged in now!");
                    //Send request to get GameWorld
                    Send(new Helpers.TcpRequest("gameworld"));

                }
                else if (res == "max_players")
                {
                    //Server full
                    Logging.Warn("[Client] The server is full");
                }
                else if (res == "wrong_password")
                {
                    //Wrong password
                    Logging.Warn("[Client] You did enter the wrong password");
                }
            }
        }

        static void OnGameWorldReceived(Helpers.TcpGameWorld world)
		{
            GameWorld.World changes = (GameWorld.World)world.Data.GetValue("changes");
            bool addition = (bool)world.Data.GetValue("addition");
            Logging.Info($"[Client] Updating GameWorld => " + addition);
            GameWorld.Client.Instance.UpdateLocalWorld(changes, addition);
        }

		#region Messages
		public static void Send(Helpers.TcpLogin login)
		{
            Logging.Info("[Client] Sending login message");
            client.Send(login.Serialize());
		}

        public static void Send(Helpers.TcpGameWorld changes)
		{
            Logging.Info("[Client] Sending gameworld update");
            client.Send(changes.Serialize());
		}

        public static void Send(Helpers.TcpChat chatmsg)
		{
            Logging.Info("[Message] You: " + (string)chatmsg.Data.GetValue("message"));
            client.Send(chatmsg.Serialize());
		}

        public static void Send(Helpers.TcpRequest request)
		{
            Logging.Info("[Client] Sending request");
            client.Send(request.Serialize());
		}

        public static void Send(Helpers.TcpResponse response)
		{
            Logging.Info("[Client] Sending response");
            client.Send(response.Serialize());
		}

        public static void Send(Helpers.TcpGamespeed speed)
		{
            Logging.Info("[Client] Sending gamespeed");
            client.Send(speed.Serialize());
		}

		#endregion

		public static void Disconnect()
		{
            if (!Connected)
            {
                Logging.Warn("[Client] You can't disconnect a client that isn't connected...");
                return;
			}
            Chat.ClearHistory();
            client.Disconnect();
		}
	}
}
