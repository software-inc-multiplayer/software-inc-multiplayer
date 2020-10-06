using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telepathy;
using Multiplayer.Debugging;
using Multiplayer.Networking;
using Multiplayer.Extensions;
using Multiplayer.Shared;

namespace Multiplayer.Networking
{
    public partial class Server
    {
        private readonly ILogger logger;
        #region Events
        // for future reference https://itchyowl.com/events-in-unity/ maybe use unityevent some time
        public event EventHandler ServerStarted;
        public event EventHandler ServerStopped;

        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        #endregion

        public Telepathy.Server RawServer { get; set; }
        public UserManager UserManager { get; set; }
        public ServerInfo ServerInfomation { get; set; }

        public List<int> ConnectedClients { get; set; } = new List<int>();

        public Server(ILogger logger)
        {
            this.logger = logger;
        }

        public void Start(int port, string password = "")
        {
            this.RawServer = new Telepathy.Server();
            this.ServerInfomation = new ServerInfo()
                {
                    Port = port,
                    Password = password,
                    Host = new User(),
                };
            this.RawServer.Start(port);
            this.ServerStarted?.Invoke(this, null);
        }

        public void Stop()
        {
            // TODO maybe we should gracefully "remove" all clients
            this.RawServer.Stop();
            this.ServerStopped?.Invoke(this, null);
        }

        public void HandleMessages()
        {
            if (!this.RawServer.Active)
                return;

            while (this.RawServer.GetNextMessage(out Message msg))
            {
                switch (msg.eventType)
                {
                    case EventType.Connected:
                        this.ConnectedClients.Add(msg.connectionId);

                        var eventArgs = new ClientConnectedEventArgs(msg.connectionId);
                        this.ClientConnected?.Invoke(this, eventArgs);
                        if (eventArgs.Cancel)
                        {
                            this.RawServer.Disconnect(msg.connectionId);
                            this.ConnectedClients.Remove(msg.connectionId);
                        }
                        //OnClientConnect(data);
                        break;
                    case EventType.Data:

                        break;
                    case EventType.Disconnected:
                        this.ConnectedClients.Remove(msg.connectionId);
                        this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(msg.connectionId));
                        break;
                }
            }
        }

        public class ClientConnectedEventArgs : EventArgs
        {
            public ClientConnectedEventArgs(int connectionId)
            {
                this.ConnectionId = connectionId;
            }
            public int ConnectionId { get; set; }
            public bool Cancel { get; set; } = false;
        }

        public class ClientDisconnectedEventArgs : EventArgs
        {
            public ClientDisconnectedEventArgs(int connectionId)
            {
                this.ConnectionId = connectionId;
            }
            public int ConnectionId { get; set; }
        }
    }
}
