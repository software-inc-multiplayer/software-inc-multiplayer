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

namespace Multiplayer.Networking.Client
{
    public class GameClient : IDisposable
    {
        public GameClientSocket Socket { get; private set; }

        private MemoryStream serializationStream = new MemoryStream();

        public GameClient()
        {
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

        public unsafe void Send<T>(T message) where T : IMessage<T>
        {
            //maybe we should avoid this lock
            lock (serializationStream)
            {
                message.WriteTo(serializationStream);
                var buffer = serializationStream.GetBuffer();
                var messageSize = message.CalculateSize();
                fixed (byte *p = buffer)
                {
                    this.Socket.Connection.SendMessage((IntPtr)p, messageSize);
                }
                serializationStream.Position = 0;
            }
        }

        public void Dispose()
        {
            this.serializationStream?.Dispose();
            this.serializationStream = null;
        }
    }
}
