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
using Multiplayer.Packets;

namespace Multiplayer.Networking.Server
{
    public class GameServerSocket : SocketManager, IDisposable
    {
        
        public GameServer Parent { get; set; }
        
        private ILogger log;


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
            log.Debug($"Connection: {connection.ToString()}, identity: {identity.Address}");
            log.Debug($"On Message, size: {size}, messageNum: {messageNum}");
            
            byte[] managedArray = new byte[size];
            Marshal.Copy(data, managedArray, 0, size);

            Handshake handshakePacket = Handshake.Parser.ParseFrom(managedArray);
            if(handshakePacket != null)
            {
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
            }
        }
        
        public unsafe void Send<T>(T message, Connection connection) where T : IMessage<T>
        {
            //maybe we should avoid this lock
            log.Debug($"Sending message of type {typeof(T)}: {message.ToString()}");
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
            //maybe we should avoid this lock
            log.Debug($"Sending message of type {typeof(T)}: {message.ToString()}");
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
