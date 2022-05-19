using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Packets;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Google.Protobuf;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Client
{
    public class GameClient : IDisposable
    {
        public GameClientSocket Socket { get; private set; }
        private ILogger log { get; set; }

        public GameClient(ILogger log)
        {
            this.log = log;
        }

        public void Connect(string ip, ushort port)
        {
            this.Socket = SteamNetworkingSockets.ConnectNormal<GameClientSocket>(NetAddress.From(ip, port));

            var handshake = new Handshake
            {
                UserName = SteamClient.Name
            };
            this.Send(handshake);
        }



        public void Disconnect()
        {
            //TODO improve disconnection, this is pretty rude
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
