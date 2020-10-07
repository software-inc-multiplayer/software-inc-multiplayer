using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using Multiplayer.Networking.Packet;
using Multiplayer.Networking.Utility;
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
        private readonly PacketSerializer packetSerializer;

        public Client(ILogger logger, PacketSerializer packetSerializer)
        {
            this.logger = logger;
            this.packetSerializer = packetSerializer;
            this.RawClient = new Telepathy.Client();
        }
        
        public User MyUser { get; set; }
        
        public Telepathy.Client RawClient { get; set; }

        public bool HandleMessages()
        {
            var hadMessage = false;

            while (this.RawClient.GetNextMessage(out Message msg))
            {
                hadMessage = true;
                switch (msg.eventType)
                {
                    case EventType.Connected:
                        this.ClientConnected?.Invoke(this, new ClientConnectedEventArgs(msg.connectionId));

                        var handshakePacket = new Handshake() { };
                        if(!this.RawClient.Send(this.packetSerializer.SerializePacket(handshakePacket)))
                        {
                            this.logger.Error("could not send handshake packet");
                        }

                        break;
                    case EventType.Data:

                        break;
                    case EventType.Disconnected:
                        this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(msg.connectionId));
                        break;
                }
            }
            return hadMessage;
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
