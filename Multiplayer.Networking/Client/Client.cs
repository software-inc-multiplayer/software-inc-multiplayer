using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Shared;
using Telepathy;

namespace Multiplayer.Networking
{
    public class Client
    {
        #region Events
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        #endregion

        private readonly ILogger logger;

        public Client(ILogger logger)
        {
            this.logger = logger;
            this.RawClient = new Telepathy.Client();
        }
        
        public User MyUser { get; set; }
        
        public Telepathy.Client RawClient { get; set; }

        public void HandleMessages()
        {
            while (this.RawClient.GetNextMessage(out Message msg))
            {
                switch (msg.eventType)
                {
                    case EventType.Connected:
                        this.ClientConnected?.Invoke(this, new ClientConnectedEventArgs(msg.connectionId));
                        break;
                    case EventType.Data:

                        break;
                    case EventType.Disconnected:
                        this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(msg.connectionId));
                        break;
                }
            }
        }

        public void Connect(string ip, int port)
        {
            MyUser = new User();
            this.RawClient.Connect(ip, port);
        }

        public void Disconnect()
        {
            this.RawClient.Disconnect();
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
