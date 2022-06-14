using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Google.Protobuf;
using Multiplayer.Networking.Shared;
using Multiplayer.Packets;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Server
{
    public class GameServerSocket : SocketManager, IDisposable
    {

        public GameServer Parent { get; set; }

        public ILogger Logger;

        private readonly ArrayPool<byte> bufferPool = ArrayPool<byte>.Create();

        private readonly List<IPacketHandler> packetHandlers;

        public GameServerSocket()
        {
            packetHandlers = RegisterManager.FetchInstancesWithAttribute(RegisterType.SERVER, this);
        }

        public override void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            base.OnConnectionChanged(connection, info);
        }


        public override void OnConnected(Connection connection, ConnectionInfo info)
        {
            Logger.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            Logger.Debug($"Connected!");
        }

        public override void OnConnecting(Connection connection, ConnectionInfo info)
        {
            Logger.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            var result = connection.Accept();
            Logger.Debug($"Accepted");
        }

        public override void OnDisconnected(Connection connection, ConnectionInfo info)
        {
            Logger.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            var result = connection.Close();
            Logger.Debug($"Disconnected and Closed");
        }

        public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            Logger.Debug($"Connection: {connection}, identity: {identity.Address}");
            Logger.Debug($"On Message, size: {size}, messageNum: {messageNum}");

            // TODO this should have a reasonable size
            if (size > 10 * 1024 * 1024) // 10kb
            {
                Logger.Warn("Discarding large packet");
                return;
            }

            var buffer = this.bufferPool.Rent(size);
            Marshal.Copy(data, buffer, 0, size);

            var gamePacket = GamePacket.Parser.ParseFrom(buffer, 0, size);

            var types = Enum.GetValues(typeof(GamePacket.PacketOneofCase));

            var assembly = typeof(GameServerSocket).Assembly;
            
            foreach (GamePacket.PacketOneofCase type in types)
            {
                if (type == GamePacket.PacketOneofCase.None) continue;
                foreach (var packetHandler in packetHandlers.Where(packetHandler => packetHandler.GetType().GetGenericArguments()[0] == assembly.GetType("Multiplayer.Packets." + type)))
                {
                    // using connection.Id probably wont work for now, we need a way to assign GameUsers connection IDs
                    packetHandler.HandlePacket(Parent.UserManager.GetUser(connection.Id), gamePacket);
                }
            }
            
            // TODO: Move this switch into packet handlers.
            
            // switch (gamePacket.PacketCase)
            // {
            //     case GamePacket.PacketOneofCase.None: // error
            //         break;
            //     case GamePacket.PacketOneofCase.Handshake:
            //         var handshakePacket = gamePacket.Handshake;
            //         if (Parent.ServerInfo.HasPassword)
            //         {
            //             if (Parent.ServerInfo.Password != handshakePacket.Password)
            //             {
            //                 // Invalid Password
            //                 Send(new AuthResponse()
            //                 {
            //                     Type = ResponseType.Bad
            //                 }, connection);
            //                 return;
            //             }
            //
            //             // Password correct.
            //             GameUser user = new GameUser()
            //             {
            //                 Name = handshakePacket.Username,
            //                 Role = UserRole.Guest,
            //                 Id = handshakePacket.Id
            //             };
            //
            //             Parent.UserManager.GetOrAddUser(user);
            //
            //             BanInformation? banned = Parent.UserManager.CheckBanned(user);
            //
            //             if (banned != null)
            //             {
            //                 Send(new AuthResponse()
            //                 {
            //                     Type = ResponseType.Banned,
            //                     BanInfo = banned
            //                 }, connection);
            //                 return;
            //             }
            //
            //             Send(new Handshake()
            //             {
            //                 Server = true
            //             }, connection);
            //         }
            //
            //         break;
            //     case GamePacket.PacketOneofCase.ChatMessage:
            //         break;
            // }
        }

        public unsafe void Send<T>(T message, Connection connection) where T : IMessage<T>
        {
            //TODO implement a sending queue and a background task/thread
            Logger.Debug($"Sending message of type {typeof(T)}: {message}");
            using (var serializationStream = new MemoryStream())
            {
                message.WriteTo(serializationStream);
                var buffer = serializationStream.GetBuffer();
                var messageSize = message.CalculateSize();
                fixed (byte* p = buffer)
                {
                    connection.SendMessage((IntPtr)p, messageSize);
                }
                serializationStream.Position = 0;
            }

            Logger.Debug($"Message sent");

        }

        public unsafe void SendAll<T>(T message) where T : IMessage<T>
        {
            //TODO implement a sending queue and a background task/thread
            Logger.Debug($"Sending message of type {typeof(T)}: {message}");
            using (var serializationStream = new MemoryStream())
            {
                message.WriteTo(serializationStream);
                var buffer = serializationStream.GetBuffer();
                var messageSize = message.CalculateSize();
                fixed (byte* p = buffer)
                {
                    foreach (var connection in Socket.Manager.Connected)
                    {
                        connection.SendMessage((IntPtr)p, messageSize);
                    }
                }
                serializationStream.Position = 0;
            }

            Logger.Debug($"Message sent");

        }

        public void Dispose()
        {
            try
            {
                foreach (var connection in Connected)
                {
                    connection.Close();
                }
                this.Close();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
    }
}
