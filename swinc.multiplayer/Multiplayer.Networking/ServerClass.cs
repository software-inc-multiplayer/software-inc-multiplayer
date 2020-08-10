using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Multiplayer.Debugging;
using WatsonTcp;

namespace Multiplayer.Networking
{

    public class ServerClass : IDisposable
    {
        public static ServerClass Instance;
        private ushort Port = 52512;
        public ushort MaxPlayers = 10;
        private string Password = "";
        WatsonTcpServer server;
		private bool disposedValue;

        public ServerClass()
        {
            Instance = this;
        }

        public void Start(ushort port = 52512)
        {
            Port = port;
            server = new WatsonTcpServer(null, Port);
            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += MessageReceived;
            server.SyncRequestReceived = SyncRequestReceived;
            server.Start();
            Logging.Info($"[Server] Server started.");
        }
        public void Stop()
        {
            server.Dispose();
            Logging.Info($"[Server] Server disposed.");
        }

        void ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            Logging.Info("[Server] Client connected: " + args.IpPort);
        }

        void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            Logging.Info("[Server] Client disconnected: " + args.IpPort + ": " + args.Reason.ToString());
        }

        void MessageReceived(object sender, MessageReceivedFromClientEventArgs args)
        {
            Logging.Info("[Server] Message received from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
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
                    server.Dispose();
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
