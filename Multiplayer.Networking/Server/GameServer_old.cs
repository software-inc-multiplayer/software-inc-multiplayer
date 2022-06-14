// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
//
// /*using Multiplayer.Debugging;
// using Multiplayer.Extensions;*/
// using Multiplayer.Shared;
// using Multiplayer.Networking.Utility;
// using Packets;
// using Multiplayer.Networking.Shared;
// using Multiplayer.Networking.Server.Managers;
// using Multiplayer.Networking.Server.Handlers;
// using Facepunch.Steamworks;
// using Facepunch.Steamworks.Data;
// using System.Runtime.InteropServices;
//
// namespace Multiplayer.Networking.Server
// {
//     public class GameServer_old : ISocketManager, IDisposable
//     {
//         private readonly ILogger logger;
//         private readonly PacketSerializer packetSerializer;
//
//         #region Events
//         // for future reference https://itchyowl.com/events-in-unity/ maybe use unityevent some time
//         public event EventHandler ServerStarted;
//         public event EventHandler ServerStopped;
//
//         public event EventHandler<ClientConnectedEventArgs> ClientConnected;
//         public event EventHandler<ClientDisconnectedEventArgs> ClientDisconnected;
//
//         public event EventHandler<UserConnectedEventArgs> UserConnected;
//         public event EventHandler<UserDisconnectedEventArgs> UserDisconnected;
//
//         #endregion
//
//         public SocketManager RawServer { get; private set; }
//         public IUserManager UserManager { get; }
//         public BanManager BanManager { get; }
//         public ServerInfo ServerInfo { get; set; }
//
//         // a client can be connected but might not have a GameUser attached
//         private readonly HashSet<uint> connectedClients = new HashSet<uint>();
//
//         private readonly Dictionary<uint, GameUser> connectionIdToUser = new Dictionary<uint, GameUser>();
//         private readonly Dictionary<ulong, uint> userIdToConnectionId = new Dictionary<ulong, uint>();
//
//
//         //private readonly Dictionary<connectionId>
//
//
//         /// <summary>
//         /// The list of "known" connections
//         /// </summary>
//         public IReadOnlyCollection<uint> ConnectedClients { get => this.connectedClients; }
//         /// <summary>
//         /// The list of active users
//         /// </summary>
//         public IReadOnlyCollection<GameUser> ConnectedUsers { get => this.connectionIdToUser.Values; }
//
//         public GameServer_old(ILogger logger, PacketSerializer packetSerializer, IUserManager userManager, BanManager banManager)
//         {
//             this.logger = logger;
//             this.packetSerializer = packetSerializer;
//             this.UserManager = userManager;
//             this.BanManager = banManager;
//
//             /*try
//             {
//                 var serverInit = new SteamServerInit();
//                 serverInit.DedicatedServer = false;
//                 serverInit.GameDescription = "Testserver";
//                 serverInit.GamePort = 1337;
//                 serverInit.ModDir = "SINCMP";
//                 SteamServer.Init(362620, serverInit);
//             }
//             catch (Exception ex)
//             {
//                 this.logger.Error(ex);
//             }*/
//         }
//
//         public void OnConnecting(Connection connection, ConnectionInfo info)
//         {
//             if (!info.Identity.IsSteamId || !info.Identity.SteamId.IsValid)
//                 connection.Close(false, -1); //disconnect non steam or invalid identity connections
//             //TODO handle result
//             var result = connection.Accept();
//             switch (result)
//             {
//                 case Result.OK:
//                     break;
//                 default:
//                     connection.Close(false, -1);
//                     break;
//             }
//         }
//
//         public void OnConnected(Connection connection, ConnectionInfo info)
//         {
//             var sender = connection.Id;
//             this.connectedClients.Add(sender);
//
//             var eventArgs = new ClientConnectedEventArgs(sender);
//             this.ClientConnected?.Invoke(this, eventArgs);
//             this.logger.Debug($"[server] accepted connection {sender}");
//         }
//
//         public void OnConnectionChanged(Connection connection, ConnectionInfo info)
//         {
//             this.logger.Debug($"[server] connection changed {connection.Id} - {info.State}");
//             //TODO don't know what to do here so far
//             switch (info.State)
//             {
//                 case ConnectionState.Connected:
//                     break;
//                 case ConnectionState.Connecting:
//                     break;
//                 case ConnectionState.FindingRoute:
//                     break;
//                 case ConnectionState.FinWait:
//                     break;
//                 case ConnectionState.Linger:
//                     break;
//                 case ConnectionState.ProblemDetectedLocally:
//                 case ConnectionState.ClosedByPeer:
//                 case ConnectionState.Dead:
//                 case ConnectionState.None:
//                     this.connectedClients.Remove(connection.Id);
//                     connection.Close();
//                     break;
//             }
//         }
//
//         public void OnDisconnected(Connection connection, ConnectionInfo info)
//         {
//             //TODO signal user left the game
//             this.connectedClients.Remove(connection.Id);
//             connection.Close();
//         }
//
//         private bool hadMessage = false;
//
//         public void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
//         {
//             this.logger.Debug($"[server] received a packet");
//             var sender = connection.Id;
//
//             this.hadMessage = true;
//             var buffer = new byte[512];
//             Marshal.Copy(data, buffer, 0, size);
//
//             //if (!buffer.Any())
//             //    return; // ignore null packets
//             IPacket packet = null;
//             try
//             {
//                 packet = this.packetSerializer.DeserializePacket(buffer);
//             }
//             catch (Exception)
//             {
//                 // ignore serializer exceptions here
//             }
//
//             /*if (packet == null)
//             {
//                 this.logger.Warn($"[server] packet received from connectionId {msg.connectionId} is null! Ignoring packet for now.");
//                 return;
//             }*/
//
//             if (packet.Sender == 0)
//             {
//                 // do not allow server impersonation !!!
//
//                 if (this.connectionIdToUser.TryGetValue(sender, out var badUser))
//                 {
//                     var badDisconnect = new Disconnect(badUser.Id, DisconnectReason.Kicked);
//                     // graceful disconnect
//                     this.HandleDisconnect(sender, badUser, badDisconnect);
//                 }
//                 else
//                 {
//                     // this one did not complete the handshake
//                     //this.Send(sender, new Disconnect(0, DisconnectReason.Kicked));
//                     // cut the connection then...
//                     connection.Close();
//                     //this.RawServer.Disconnect(sender);
//                 }
//                 return;
//             }
//
//             if (packet is Handshake handshake)
//             {
//                 this.HandleHandshake(sender, handshake);
//                 return;
//             }
//
//             if (!this.connectionIdToUser.TryGetValue(sender, out var gameUser))
//             {
//                 // something is off ...
//                 // this user has not completed handshake but tries to send a different packet
//                 //throw new Exception("cannot handle packet for unknown user");
//                 this.logger.Warn($"[server] incompleted handshake {sender}");
//                 return;
//             }
//
//             if (packet is Disconnect disconnect)
//             {
//                 this.HandleDisconnect(sender, gameUser, disconnect);
//                 return;
//             }
//
//             if (this.bakedHandlers.TryGetValue(packet.GetType(), out var packetHandlers))
//             {
//                 foreach (var packetHandler in packetHandlers)
//                 {
//                     packetHandler.HandlePacket(gameUser, packet);
//                 }
//             }
//             else
//             {
//                 // there is no packet handler here :(
//                 this.logger.Warn($"[server] missing packet handler {sender} {packet.GetType()}");
//             }
//         }
//
//         public void Dispose()
//         {
//             // stop the server
//             this.RawServer.Close();
//
//             // clear all events
//             this.ServerStarted = null;
//             this.ServerStopped = null;
//             this.ClientConnected = null;
//             this.ClientDisconnected = null;
//         }
//
//         public void Start(ServerInfo serverInfo)
//         {
//             this.ServerInfo = serverInfo;
//             this.BakeHandlers();
//             this.RawServer = SteamNetworkingSockets.CreateNormalSocket(NetAddress.AnyIp(serverInfo.Port), this);
//
//             //this.RawServer.Start(serverInfo.Port);
//             this.ServerStarted?.Invoke(this, null);
//         }
//
//         public void Stop()
//         {
//             var stopPacket = this.packetSerializer.SerializePacket(new Disconnect(0, DisconnectReason.ServerStop));
//
//             // first send a clean disconnect to all clients
//             /*foreach (var connectionId in this.ConnectedClients)
//             {
//                 this.RawServer.Send(connectionId, stopPacket);
//             }*/
//
//             foreach (var connection in this.RawServer.Connected)
//             {
//                 connection.SendMessage(stopPacket, SendType.Reliable | SendType.NoNagle);
//                 connection.Flush();
//             }
//
//             // then kill all connections
//             foreach (var connection in this.RawServer.Connected)
//             {
//                 connection.Close();
//                 /*this.connectedClients.Remove(connectionId);
//
//                 if (this.connectionIdToUser.TryGetValue(connectionId, out var deadUser))
//                 {
//                     this.connectionIdToUser.Remove(connectionId);
//                     this.userIdToConnectionId.Remove(deadUser.Id);
//                 }*/
//             }
//
//             this.RawServer.Close();
//             this.ServerStopped?.Invoke(this, null);
//         }
//
//         protected void Send(uint connectionId, IPacket packet)
//         {
//             /*if (!this.connectedClients.Contains(connectionId))
//                 return;
//
//             this.RawServer.Send(connectionId, this.packetSerializer.SerializePacket(packet));*/
//         }
//
//         public void Send(GameUser target, IPacket packet)
//         {
//             /*if (!this.userIdToConnectionId.TryGetValue(target.Id, out var connectionId))
//                 return;
//
//             this.Send(connectionId, packet);*/
//         }
//
//         public void Broadcast(IPacket packet)
//         {
//             var serializedPacket = this.packetSerializer.SerializePacket(packet);
//             foreach (var connection in this.RawServer.Connected)
//             {
//                 connection.SendMessage(serializedPacket);
//             }
//         }
//
//         protected void Multicast(uint exceptConnectionId, IPacket packet)
//         {
//             /*var serializedPacket = this.packetSerializer.SerializePacket(packet);
//             foreach (var connectionId in this.ConnectedClients)
//             {
//                 if (connectionId != exceptConnectionId)
//                     this.RawServer.Send(connectionId, serializedPacket);
//             }*/
//         }
//
//         public void Multicast(GameUser sender, IPacket packet)
//         {
//             if (!this.userIdToConnectionId.TryGetValue(sender.Id, out var connectionId))
//                 return;
//
//             this.Multicast(connectionId, packet);
//         }
//
//         private void HandleHandshake(uint sender, Handshake handshake)
//         {
//             // wrong password
//             // TODO implement brute-force countermeasures
//             if (this.ServerInfo.HasPassword && this.ServerInfo.Password != handshake.Password)
//             {
//                 this.logger.Debug($"[server] wrong password {sender}");
//                 this.Send(sender, new Disconnect(0, DisconnectReason.InvalidPassword));
//                 //this.RawServer.Disconnect(sender);
//
//                 return;
//             }
//
//             var newUser = this.UserManager.GetOrAddUser(new GameUser()
//             {
//                 Id = handshake.Sender,
//                 Name = handshake.UserName,
//                 Role = this.ServerInfo.DefaultRole
//             });
//
//             this.connectionIdToUser.Add(sender, newUser);
//             this.userIdToConnectionId.Add(newUser.Id, sender);
//
//             var welcomePacket = new WelcomeUser(handshake.Sender, handshake.UserName);
//             this.Broadcast(welcomePacket);
//
//             this.UserConnected?.Invoke(this, new UserConnectedEventArgs(newUser));
//             this.logger.Debug($"[server] accepted user {sender} {newUser.Id} {newUser.Name}");
//         }
//
//         private void HandleDisconnect(uint connectionId, GameUser sender, Disconnect disconnect)
//         {
//             // received a graceful disconnect
//             this.Broadcast(disconnect);
//
//             // this gets cleaned up on EventType.Disconnected
//             //this.connectionIdToUser.Remove(sender);
//             //this.connectedClients.Remove(sender);
//             //this.RawServer.Disconnect(connectionId);
//         }
//
//         public bool HandleMessages()
//         {
//             //this might be unreliable with high message load
//             var hadMessage = this.hadMessage;
//             this.hadMessage = false;
//             return hadMessage;
//         }
//
//         private readonly Dictionary<Type, List<(int priority, IPacketHandler handler)>> packetHandlers = new Dictionary<Type, List<(int, IPacketHandler)>>();
//         private readonly Dictionary<Type, List<IPacketHandler>> bakedHandlers = new Dictionary<Type, List<IPacketHandler>>();
//         //public IReadOnlyDictionary<Type, IReadOnlyList<IPacketHandler>> PacketHandlers { get => this.bakedHandlers; }
//
//         public void RegisterPacketHandler(IPacketHandler packetHandler)
//         {
//             foreach (var packetType in packetHandler.PacketsFilter)
//             {
//                 if (!this.packetHandlers.TryGetValue(packetType, out var handlers))
//                 {
//                     handlers = new List<(int priority, IPacketHandler handler)>();
//                     this.packetHandlers.Add(packetType, handlers);
//                 }
//                 handlers.Add((packetHandler.Priority, packetHandler));
//             }
//         }
//
//         private void BakeHandlers()
//         {
//             this.bakedHandlers.Clear();
//             foreach (var handlerKeyValue in this.packetHandlers)
//             {
//                 this.bakedHandlers.Add(handlerKeyValue.Key,
//                     handlerKeyValue.Value
//                         .OrderBy(x => x.priority)
//                         .Select(x => x.handler)
//                         .ToList());
//             }
//         }
//     }
// }
