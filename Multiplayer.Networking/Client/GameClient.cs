using System;
using System.IO;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Google.Protobuf;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Shared.Managers;
using Multiplayer.Packets;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Client
{
    public class GameClient : IDisposable
    {
        private readonly GameUser? virtualUser;
        public GameClientSocket Socket { get; private set; }
        private ILogger log { get; set; }

        public GameClient(ILogger log, GameUser? virtualUser)
        {
            this.log = log;
            this.virtualUser = virtualUser;
            RegisterManager.LoadInstances(this, null);
        }

        public void Connect(string ip, ushort port, string password = "")
        {
            this.Socket = SteamNetworkingSockets.ConnectNormal<GameClientSocket>(NetAddress.From(ip, port));

            this.Socket.Parent = this;

            if (virtualUser != null)
            {
                Send(new Handshake
                {
                    Username = virtualUser.Name,
                    Password = password,
                    Id = virtualUser.Id
                });
                return;
            }
            
            var handshake = new Handshake
            {
                Username = SteamClient.Name,
                Password = password,
                Id = SteamClient.SteamId.Value
            };
            this.Send(handshake);
        }

        public void Disconnect()
        {
            Socket.Connection.Close();
        }

        public unsafe void Send<T>(T message) where T : IMessage<T>
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
                    this.Socket.Connection.SendMessage((IntPtr)p, messageSize);

                }
                serializationStream.Position = 0;
            }

            log.Debug($"Message sent");

        }

        public void Dispose()
        {
            Socket?.Connection.Close();
            Socket?.Dispose();
            Socket = null;
        }
    }
}
