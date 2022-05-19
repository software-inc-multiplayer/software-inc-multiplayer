using System;

using Multiplayer.Networking.Utility;
using Multiplayer.Shared;

using Packets;
using System.Threading;
using Multiplayer.Networking.Shared;
using System.Collections.Generic;
using System.Linq;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;

namespace Multiplayer.Networking.Client
{
    public class GameClient_old : ISocketManager, IDisposable
    {
        #region Events
        public event EventHandler<ClientConnectedEventArgs> ClientConnected;
        public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
        public event EventHandler ConnectionReady;
        public event EventHandler<UserConnectedEventArgs> UserConnected;
        public event EventHandler<UserDisconnectedEventArgs> UserDisconnected;
        #endregion

        private readonly ILogger logger;
        private readonly PacketSerializer packetSerializer;
        public IUserManager UserManager { get; }
        private readonly GameUser gameUser;

        public GameClient_old(ILogger logger, GameUser user, PacketSerializer packetSerializer, IUserManager userManager)
        {
            this.logger = logger;
            this.packetSerializer = packetSerializer;
            this.UserManager = userManager;
            this.gameUser = user;
            try
            {
                SteamClient.Init(362620);
            }
            catch (Exception ex)
            {
                this.logger.Error(ex);
            }
        }

        public void Dispose()
        {
            // disconnect the client
            this.Disconnect();
            // clear all events
            this.ClientConnected = null;
            this.ClientDisconnected = null;

            SteamClient.Shutdown();
        }

        public SocketManager RawClient { get; private set; }

        public void Send(IPacket packet)
        {
            // maybe add a check if we are still connected
            /*if (!this.RawClient.Send(this.packetSerializer.SerializePacket(packet)))
            {
                this.logger.Error("could not send packet");
            }*/
        }

        private void HandleWelcomeUser(WelcomeUser welcomeUser)
        {
            this.logger.Debug($"[client] welcome client {welcomeUser.Sender}, {welcomeUser.UserName}");

            var newUser = this.UserManager.GetOrAddUser(new GameUser()
            {
                Id = welcomeUser.Sender,
                Name = welcomeUser.UserName,
            });

            if (welcomeUser.Sender == this.gameUser.Id)
                this.ConnectionReady?.Invoke(this, null);
            this.UserConnected?.Invoke(this, new UserConnectedEventArgs(newUser));
        }

        private void HandleDisconnect(GameUser user, Disconnect disconnect)
        {
            var disconnectedUserId = disconnect.Sender;

            if (disconnectedUserId == 0UL || disconnectedUserId == this.gameUser.Id)
            {
                // duh something bad happened and this client is doomed...
                //this.RawClient.Disconnect();
                return;
            }

            this.UserManager.RemoveUser(user);
            this.logger.Debug($"[client] removing client {disconnectedUserId}");
        }

        /*private void InternalHandleMessage(Message msg)
        {
            switch (msg.eventType)
            {
                case EventType.Connected:
                    this.ClientConnected?.Invoke(this, new ClientConnectedEventArgs(msg.connectionId));

                    this.Send(new Handshake(this.gameUser.Id, this.gameUser.Name));
                    this.logger.Debug("[client] connected to server");
                    break;
                case EventType.Data:

                    // TODO check msg.connectionId. It should not change
                    IPacket packet = null;
                    try
                    {
                        packet = this.packetSerializer.DeserializePacket(msg.data);
                    } catch(Exception)
                    {
                        // ignore exceptions
                    }
                    if (packet == null)
                    {
                        // maybe add some more details
                        this.logger.Warn("[client] received unknown packet");
                        break;
                    }

                    if (packet is WelcomeUser welcomeUser)
                    {
                        this.HandleWelcomeUser(welcomeUser);
                        break;
                    }

                    var gameUser = this.UserManager.GetUser(packet.Sender);

                    if (gameUser == null && packet.Sender != 0UL)
                    {
                        // this packet is not sent by the server and has no known user
                        this.logger.Warn($"[client] incompleted handshake {packet.Sender}");
                        break;
                    }

                    if (packet is Disconnect disconnect)
                    {
                        this.HandleDisconnect(gameUser, disconnect);
                        break;
                    }

                    if (this.bakedHandlers.TryGetValue(packet.GetType(), out var packetHandlers))
                    {
                        foreach (var packetHandler in packetHandlers)
                        {
                            packetHandler.HandlePacket(gameUser, packet);
                        }
                    }
                    else
                    {
                        // there is no packet handler here :(
                        this.logger.Warn($"[client] missing packet handler {packet.GetType()}");
                    }

                    break;
                case EventType.Disconnected:

                    // maybe we should fire a userdisconnected for all users here?
                    this.UserManager.Clear();
                    this.ClientDisconnected?.Invoke(this, new ClientDisconnectedEventArgs(msg.connectionId));
                    this.logger.Debug("[client] disconnected from server");
                    break;
            }
        }*/

        public bool HandleMessages()
        {
            var hadMessage = false;

            /*while (this.RawClient.GetNextMessage(out Message msg))
            {
                hadMessage = true;
                this.InternalHandleMessage(msg);
            }*/
            return hadMessage;
        }

        public void Connect(string ip, ushort port)
        {
            this.RawClient = SteamNetworkingSockets.CreateNormalSocket(NetAddress.From(ip, port), this);
        }

        public void Disconnect()
        {
            // TODO this send does not really work as the disconnect kills the connection
            this.Send(new Disconnect(this.gameUser.Id, DisconnectReason.Leaving));
            //this.RawClient.Disconnect();
        }

        private readonly Dictionary<Type, List<(int priority, IPacketHandler handler)>> packetHandlers = new Dictionary<Type, List<(int, IPacketHandler)>>();
        private readonly Dictionary<Type, List<IPacketHandler>> bakedHandlers = new Dictionary<Type, List<IPacketHandler>>();
        //public IReadOnlyDictionary<Type, IReadOnlyList<IPacketHandler>> PacketHandlers { get => this.bakedHandlers; }

        public void RegisterPacketHandler(IPacketHandler packetHandler)
        {
            foreach (var packetType in packetHandler.PacketsFilter)
            {
                if (!this.packetHandlers.TryGetValue(packetType, out List<(int, IPacketHandler)> handlers))
                {
                    handlers = new List<(int, IPacketHandler)>();
                    this.packetHandlers.Add(packetType, handlers);
                }
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

        public void OnConnecting(Connection connection, ConnectionInfo info)
        {
            connection.Accept();
        }

        public void OnConnected(Connection connection, ConnectionInfo info)
        {
            this.logger.Debug($"[client] connected to {info.Address.Address}");
        }

        public void OnDisconnected(Connection connection, ConnectionInfo info)
        {
            this.logger.Debug($"[client] disconnected");
        }

        public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            this.logger.Debug($"[client] message");
        }
    }
}
