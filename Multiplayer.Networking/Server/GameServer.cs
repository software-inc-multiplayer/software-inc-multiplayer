using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Telepathy;

using Multiplayer.Debugging;
using Multiplayer.Extensions;
using Multiplayer.Shared;
using Multiplayer.Networking.Utility;
using Packets;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Server.Managers;
using Multiplayer.Networking.Server.Handlers;

namespace Multiplayer.Networking.Server
{
    public class GameServer : IDisposable
    {
        private readonly ILogger logger;
        private readonly PacketSerializer packetSerializer;

        #region Events
        // for future reference https://itchyowl.com/events-in-unity/ maybe use unityevent some time
        public event EventHandler ServerStarted;
        public event EventHandler ServerStopped;

        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;

        public event EventHandler<UserConnectedEventArgs> UserConnected;
        public event EventHandler<UserDisconnectedEventArgs> UserDisconnected;

        #endregion

        public Telepathy.Server RawServer { get; }
        public IUserManager UserManager { get; }
        public BanManager BanManager { get; }
        public ServerInfo ServerInfo { get; set; }

        // a client can be connected but might not have a GameUser attached
        private readonly HashSet<int> connectedClients = new HashSet<int>();
        private readonly Dictionary<int, GameUser> connectionIdToUser = new Dictionary<int, GameUser>();
        private readonly Dictionary<ulong, int> userIdToConnectionId = new Dictionary<ulong, int>();

        /// <summary>
        /// The list of "known" connections
        /// </summary>
        public IReadOnlyCollection<int> ConnectedClients { get => this.connectedClients; }
        /// <summary>
        /// The list of active users
        /// </summary>
        public IReadOnlyCollection<GameUser> ConnectedUsers { get => this.connectionIdToUser.Values; }

        public GameServer(ILogger logger, PacketSerializer packetSerializer, IUserManager userManager, BanManager banManager)
        {
            this.logger = logger;
            this.packetSerializer = packetSerializer;
            this.UserManager = userManager;
            this.BanManager = banManager;
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

        public void Start(ServerInfo serverInfo)
        {
            this.ServerInfo = serverInfo;
            this.BakeHandlers();
            this.RawServer.Start(serverInfo.Port);
            this.ServerStarted?.Invoke(this, null);
        }

        public void Stop()
        {
            var stopPacket = this.packetSerializer.SerializePacket(new Disconnect(0, DisconnectReason.ServerStop));

            // first send a clean disconnect to all clients
            foreach (var connectionId in this.ConnectedClients)
            {
                this.RawServer.Send(connectionId, stopPacket);
            }

            // then kill all connections
            foreach (var connectionId in this.ConnectedClients)
            {
                this.RawServer.Disconnect(connectionId);
                this.connectedClients.Remove(connectionId);

                if(this.connectionIdToUser.TryGetValue(connectionId, out var deadUser))
                {
                    this.connectionIdToUser.Remove(connectionId);
                    this.userIdToConnectionId.Remove(deadUser.Id);
                }
            }

            this.RawServer.Stop();
            this.ServerStopped?.Invoke(this, null);
        }

        protected void Send(int connectionId, IPacket packet)
        {
            if (!this.connectedClients.Contains(connectionId))
                return;

            this.RawServer.Send(connectionId, this.packetSerializer.SerializePacket(packet));
        }

        public void Send(GameUser target, IPacket packet)
        {
            if (!this.userIdToConnectionId.TryGetValue(target.Id, out var connectionId))
                return;

            this.Send(connectionId, packet);
        }

        public void Broadcast(IPacket packet)
        {
            var serializedPacket = this.packetSerializer.SerializePacket(packet);
            foreach (var connectionId in this.ConnectedClients)
            {
                this.RawServer.Send(connectionId, serializedPacket);
            }
        }

        protected void Multicast(int exceptConnectionId, IPacket packet)
        {
            var serializedPacket = this.packetSerializer.SerializePacket(packet);
            foreach (var connectionId in this.ConnectedClients)
            {
                if (connectionId != exceptConnectionId)
                    this.RawServer.Send(connectionId, serializedPacket);
            }
        }

        public void Multicast(GameUser sender, IPacket packet)
        {
            if (!this.userIdToConnectionId.TryGetValue(sender.Id, out var connectionId))
                return;

            this.Multicast(connectionId, packet);
        }

        private void HandleHandshake(int sender, Handshake handshake)
        {
            // wrong password
            // TODO implement brute-force countermeasures
            if (this.ServerInfo.HasPassword && this.ServerInfo.Password != handshake.Password)
            {
                this.logger.Debug($"[server] wrong password {sender}");
                this.Send(sender, new Disconnect(0, DisconnectReason.InvalidPassword));
                this.RawServer.Disconnect(sender);

                return;
            }

            var newUser = this.UserManager.GetOrAddUser(new GameUser() {
                Id = handshake.Sender,
                Name = handshake.UserName,
                Role = this.ServerInfo.DefaultRole
            });

            this.connectionIdToUser.Add(sender, newUser);
            this.userIdToConnectionId.Add(newUser.Id, sender);

            var welcomePacket = new WelcomeUser(handshake.Sender, handshake.UserName);
            this.Broadcast(welcomePacket);

            this.UserConnected?.Invoke(this, new UserConnectedEventArgs(newUser));
            this.logger.Debug($"[server] accepted user {sender} {newUser.Id} {newUser.Name}");
        }

        private void HandleDisconnect(int connectionId, GameUser sender, Disconnect disconnect)
        {
            // received a graceful disconnect
            this.Broadcast(disconnect);

            // this gets cleaned up on EventType.Disconnected
            //this.connectionIdToUser.Remove(sender);
            //this.connectedClients.Remove(sender);
            this.RawServer.Disconnect(connectionId);
        }

        private void InternalHandleMessage(Message msg)
        {
            var sender = msg.connectionId;
            switch (msg.eventType)
            {
                case EventType.Connected:
                    this.connectedClients.Add(sender);

                    var eventArgs = new ClientConnectedEventArgs(sender);
                    this.ClientConnected?.Invoke(this, eventArgs);
                    this.logger.Debug($"[server] accepted connection {sender}");

                    break;
                case EventType.Data:
                    if (msg.data == null || !msg.data.Any())
                        break; // ignore null packets
                    IPacket packet = null;
                    try
                    {
                        packet = this.packetSerializer.DeserializePacket(msg.data);
                    }
                    catch(Exception)
                    {
                        // ignore serializer exceptions here
                    }
                    if (packet == null)
                    {
                        this.logger.Warn($"[server] packet received from connectionId {msg.connectionId} is null! Ignoring packet for now.");
                        break;
                    }

                    if (packet.Sender == 0)
                    {
                        // do not allow server impersonation !!!
                        
                        if (this.connectionIdToUser.TryGetValue(sender, out var badUser))
                        {
                            var badDisconnect = new Disconnect(badUser.Id, DisconnectReason.Kicked);
                            // graceful disconnect
                            this.HandleDisconnect(sender, badUser, badDisconnect);
                        }
                        else
                        {
                            // this one did not complete the handshake
                            this.Send(sender, new Disconnect(0, DisconnectReason.Kicked));
                            // cut the connection then...
                            this.RawServer.Disconnect(sender);
                        }
                        break;
                    }

                    if (packet is Handshake handshake)
                    {
                        this.HandleHandshake(sender, handshake);
                        break;
                    }

                    if (!this.connectionIdToUser.TryGetValue(sender, out var gameUser))
                    {
                        // something is off ...
                        // this user has not completed handshake but tries to send a different packet
                        //throw new Exception("cannot handle packet for unknown user");
                        this.logger.Warn($"[server] incompleted handshake {sender}");
                        break;
                    }

                    if (packet is Disconnect disconnect)
                    {
                        this.HandleDisconnect(sender, gameUser, disconnect);
                        break;
                    }

                    if(this.bakedHandlers.TryGetValue(packet.GetType(), out var packetHandlers))
                    {
                        foreach(var packetHandler in packetHandlers)
                        {
                            packetHandler.HandlePacket(gameUser, packet);
                        }
                    } else
                    {
                        // there is no packet handler here :(
                        this.logger.Warn($"[server] missing packet handler {sender} {packet.GetType()}");
                    }
                    break;
                case EventType.Disconnected:
                    // for some reason we have to close a connection
                    this.logger.Debug($"[server] closing connection {sender}");
                    if (this.connectionIdToUser.TryGetValue(sender, out var disconnectUser))
                    {
                        this.logger.Debug($"[server] removing user {sender} {disconnectUser.Id}");
                        this.userIdToConnectionId.Remove(disconnectUser.Id);
                        this.UserManager.RemoveUser(disconnectUser);
                        this.logger.Debug($"[server] removed user {sender} {disconnectUser.Id}");
                    }
                    this.connectionIdToUser.Remove(sender);
                    this.connectedClients.Remove(sender);

                    this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(sender));
                    this.UserDisconnected?.Invoke(this, new UserDisconnectedEventArgs(disconnectUser));
                    this.logger.Debug($"[server] closed connection {sender}");
                    break;
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
                this.InternalHandleMessage(msg);
            }
            return hadMessage;
        }

        private readonly Dictionary<Type, List<(int priority, IPacketHandler handler)>> packetHandlers = new Dictionary<Type, List<(int, IPacketHandler)>>();
        private readonly Dictionary<Type, List<IPacketHandler>> bakedHandlers = new Dictionary<Type, List<IPacketHandler>>();
        //public IReadOnlyDictionary<Type, IReadOnlyList<IPacketHandler>> PacketHandlers { get => this.bakedHandlers; }

        public void RegisterPacketHandler(IPacketHandler packetHandler)
        {
            foreach (var packetType in packetHandler.PacketsFilter)
            {
                var handlers = this.packetHandlers.GetOrAdd(packetType, (_) => new List<(int, IPacketHandler)>());
                handlers.Add((packetHandler.Priority, packetHandler));
            }
        }

        private void BakeHandlers()
        {
            this.bakedHandlers.Clear();
            foreach (var handlerKeyValue in this.packetHandlers)
            {
                this.bakedHandlers.Add(handlerKeyValue.Key,
                    handlerKeyValue.Value
                        .OrderBy(x => x.priority)
                        .Select(x => x.handler)
                        .ToList());
            }
        }
    }
}
