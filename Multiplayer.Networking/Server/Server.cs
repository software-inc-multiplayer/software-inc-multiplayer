using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telepathy;
using MessagePack;

using Multiplayer.Debugging;
using Multiplayer.Networking;
using Multiplayer.Extensions;
using Multiplayer.Shared;
using Multiplayer.Networking.Utility;
using Multiplayer.Networking.Packet;

namespace Multiplayer.Networking
{
    public partial class Server : IDisposable
    {
        private readonly ILogger logger;
        private readonly PacketSerializer packetSerializer;
        #region Events
        // for future reference https://itchyowl.com/events-in-unity/ maybe use unityevent some time
        public event EventHandler ServerStarted;
        public event EventHandler ServerStopped;

        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        // this WAS a nice idea but triggers ALL handlers for EVERY packet
        // should be some kind of dictionary lookup per defined type and some kind of packet handler with filter options
        public event EventHandler<ReceivedPacketEventArgs> ReceivedPacket;
        #endregion

        public Telepathy.Server RawServer { get; set; }
        public UserManager UserManager { get; set; }
        public ServerInfo ServerInfomation { get; set; }

        public List<int> ConnectedClients { get; set; } = new List<int>();

        public Server(ILogger logger, PacketSerializer packetSerializer)
        {
            this.logger = logger;
            this.packetSerializer = packetSerializer;
            this.RawServer = new Telepathy.Server();
        }

        public void Dispose()
        {
            // stop the server
            this.RawServer.Stop();
            // clear all events
            this.ServerStarted = null;
            this.ServerStopped = null;
            this.ClientConnected = null;
            this.ClientDisconnected = null;
        }

        public void Start(int port, string password = "")
        {
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

        protected void Send(int connectionId, IPacket packet)
        {
            this.RawServer.Send(connectionId, this.packetSerializer.SerializePacket(packet));
        }

        protected void Broadcast(IPacket packet)
        {
            foreach (var connectionId in this.ConnectedClients)
            {
                this.RawServer.Send(connectionId, this.packetSerializer.SerializePacket(packet));
            }
        }

        public bool HandleMessages()
        {
            if (!this.RawServer.Active)
                return false;
            var hadMessage = false;
            while (this.RawServer.GetNextMessage(out Message msg))
            {
                hadMessage = true;
                var sender = msg.connectionId;
                switch (msg.eventType)
                {
                    case EventType.Connected:
                        this.ConnectedClients.Add(sender);

                        var eventArgs = new ClientConnectedEventArgs(sender);
                        this.ClientConnected?.Invoke(this, eventArgs);
                        if (eventArgs.Cancel)
                        {
                            this.Send(sender, new Disconnect("invalid handshake"));

                            this.RawServer.Disconnect(sender);
                            this.ConnectedClients.Remove(sender);
                        }
                        break;
                    case EventType.Data:
                        if (msg.data == null || !msg.data.Any())
                            break;

                        var packet = this.packetSerializer.DeserializePacket(msg.data);

                        if(packet == null)
                        {
#if DEBUG
                            // maybe add some logging here
#endif
                            break;
                        }

                        var packetEventArgs = new ReceivedPacketEventArgs(sender, packet);
                        this.ReceivedPacket?.Invoke(this, packetEventArgs);

                        if(!packetEventArgs.Handled)
                        {
                            // What shall we do here?
                            // for the start we enforce a disconnect
                            this.Send(sender, new Disconnect("unhandled packet received"));
                            this.RawServer.Disconnect(sender);
                            this.ConnectedClients.Remove(sender);
#if TEST
                            throw new Exception($"unhandled packet {packet.GetType()}");
#endif
                        }
                        break;
                    case EventType.Disconnected:
                        this.ConnectedClients.Remove(sender);
                        this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(sender));
                        break;
                }
            }
            return hadMessage;
        }

        public class ClientConnectedEventArgs : EventArgs
        {
            public int ConnectionId { get; }
            public bool Cancel { get; set; } = false;
            public ClientConnectedEventArgs(int connectionId)
            {
                this.ConnectionId = connectionId;
            }
        }

        public class ClientDisconnectedEventArgs : EventArgs
        {
            public int ConnectionId { get; }
            public ClientDisconnectedEventArgs(int connectionId)
            {
                this.ConnectionId = connectionId;
            }
        }

        public class ReceivedPacketEventArgs : EventArgs
        {
            public int ConnectionId { get; }
            public IPacket Packet { get; }
            public bool Handled { get; set; } = false;

            public ReceivedPacketEventArgs(int connectionId, IPacket packet)
            {
                this.ConnectionId = connectionId;
                this.Packet = packet;
            }
        }
    }
}
