using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Debugging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Steamworks;
using WatsonTcp;

namespace Multiplayer.Networking
{
    public class ClientClass : IDisposable
    {
        public static ClientClass Instance;
        bool isLoggedin = false;
        static WatsonTcpClient client;
		private bool disposedValue;

        public ClientClass()
		{
            Instance = this;
		}

		public async void Connect(string ip, ushort port = 52512)
        {
            client = new WatsonTcpClient(ip, port);
            client.ServerConnected += ServerConnected;
            client.ServerDisconnected += ServerDisconnected;
            client.MessageReceived += MessageReceived;
            client.SyncRequestReceived = SyncRequestReceived;
            client.Start();

            // check connectivity
            Logging.Info("[Client] Am I Connected? " + client.Connected);

            // send a message
            client.Send("Hello!");
            // send a message with metadata
            Dictionary<object, object> md = new Dictionary<object, object>
            {
                { "foo", "bar" }
            };
            client.Send(md, "Hello, client!  Here's some metadata!");

            // send async!
            await client.SendAsync("Hello, client!  I'm async!");

            // send and wait for a response
            try
            {
                SyncResponse resp = client.SendAndWait(5000, "Hey, say hello back within 5 seconds!");
                Logging.Info("[Client] My friend says: " + Encoding.UTF8.GetString(resp.Data));
            }
            catch (TimeoutException)
            {
                Logging.Info("[Client] Too slow...");
            }
            //Helpers.SystemMessage sysmsg = new Helpers.SystemMessage(Helpers.SysMessageType.Login, "User", Helpers.UserRole.Host);
            //client.Send(sysmsg.AsMessage().ToJson());
        }

        void MessageReceived(object sender, MessageReceivedFromServerEventArgs args)
        {
            Logging.Info("[Client] Message from server: " + Encoding.UTF8.GetString(args.Data));
        }

        void ServerConnected(object sender, EventArgs args)
        {
            Logging.Info("[Client] Server connected");
        }

        void ServerDisconnected(object sender, EventArgs args)
        {
            Logging.Error("[Client] Server disconnected");
        }

        SyncResponse SyncRequestReceived(SyncRequest req)
        {
            return new SyncResponse(req, "Hello back at you!");
        }

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
                    client.Dispose();
				}
				disposedValue = true;
			}
		}

		public void Dispose()
		{
			Dispose(disposing: true);
			GC.SuppressFinalize(this);
		}
	}
}
