using System;

using Multiplayer.Networking.Utility;
using Multiplayer.Shared;
using Telepathy;
using Packets;
using System.Threading;

namespace Multiplayer.Networking
{
    public class Client : IDisposable
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

        public void Dispose()
        {
            // disconnect the client
            this.Disconnect();
            // clear all events
            this.ClientConnected = null;
            this.ClientDisconnected = null;
        }

        public User MyUser { get; set; }

        public Telepathy.Client RawClient { get; set; }

        protected void Send(IPacket packet)
        {
            // maybe add a check if we are still connected
            if (!this.RawClient.Send(this.packetSerializer.SerializePacket(packet)))
            {
                this.logger.Error("could not send packet");
            }
        }

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

                        this.Send(new Handshake(this.MyUser));

                        break;
                    case EventType.Data:

                        var genericPacket = this.packetSerializer.DeserializePacket(msg.data);
                        if (genericPacket == null)
                        {
#if DEBUG
                            // maybe add some more details
                            this.logger.Warn("received unknown packet");
#endif
                        }

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
            // TODO this send does not really work as the disconnect kills the connection
            this.Send(new Disconnect(DisconnectReason.Leaving));
            //this.RawClient.Disconnect();
        }


        public class ClientConnectedEventArgs : EventArgs
        {
            public ClientConnectedEventArgs(int connectionId)
            {
                this.ConnectionId = connectionId;
            }
            public int ConnectionId { get; set; }
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
