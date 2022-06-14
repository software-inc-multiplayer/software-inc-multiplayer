using System;
using System.IO;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Google.Protobuf;
using Multiplayer.Debugging;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Utility;
using Multiplayer.Packets;
using Multiplayer.Shared;
using System.Buffers;

namespace Multiplayer.Networking.Server
{
    public class GameServerSocket : SocketManager, IDisposable
    {

        public GameServer Parent { get; set; }

        private ILogger log;

        private readonly ArrayPool<byte> bufferPool = ArrayPool<byte>.Create();
        public GameServerSocket()
        {
            this.log = new FileLogger();
        }

        public override void OnConnectionChanged(Connection connection, ConnectionInfo info)
        {
            base.OnConnectionChanged(connection, info);
        }


        public override void OnConnected(Connection connection, ConnectionInfo info)
        {
            log.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            log.Debug($"Connected!");
        }

        public override void OnConnecting(Connection connection, ConnectionInfo info)
        {
            log.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            var result = connection.Accept();
            log.Debug($"Accepted");
        }

        public override void OnDisconnected(Connection connection, ConnectionInfo info)
        {
            log.Debug($"Connection: {connection.ToString()}, connectionInfo: {info.Address}");
            var result = connection.Close();
            log.Debug($"Disconnected and Closed");
        }

        public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            log.Debug($"Connection: {connection}, identity: {identity.Address}");
            log.Debug($"On Message, size: {size}, messageNum: {messageNum}");

            // TODO this should have a reasonable size
            if (size > 10 * 1024 * 1024) // 10kb
            {
                log.Warn("Discarding large packet");
                return;
            }

            var buffer = this.bufferPool.Rent(size);
            Marshal.Copy(data, buffer, 0, size);

            var gamePacket = GamePacket.Parser.ParseFrom(buffer, 0, size);
            switch (gamePacket.PacketCase)
            {
                case GamePacket.PacketOneofCase.None: // error
                    break;
                case GamePacket.PacketOneofCase.Handshake:
                    var handshakePacket = gamePacket.Handshake;
                    if (Parent.ServerInfo.HasPassword)
                    {
                        if (Parent.ServerInfo.Password != handshakePacket.Password)
                        {
                            // Invalid Password
                            Send(new BadPassword(), connection);
                            return;
                        }

                        // Password correct.
                        GameUser user = new GameUser()
                        {
                            Name = handshakePacket.Username,
                            Role = UserRole.Guest,
                            Id = handshakePacket.Id
                        };

                        Parent.UserManager.GetOrAddUser(user);

                        Send(new Handshake()
                        {
                            Server = true
                        }, connection);
                    }

                    break;
                case GamePacket.PacketOneofCase.ChatMessage:
                    break;
            }
        }

        public unsafe void Send<T>(T message, Connection connection) where T : IMessage<T>
        {
            //TODO implement a sending queue and a background task/thread
            log.Debug($"Sending message of type {typeof(T)}: {message}");
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

            log.Debug($"Message sent");

        }

        public unsafe void SendAll<T>(T message) where T : IMessage<T>
        {
            //TODO implement a sending queue and a background task/thread
            log.Debug($"Sending message of type {typeof(T)}: {message}");
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

            log.Debug($"Message sent");

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
                log.Error(ex);
            }
        }
    }
}
