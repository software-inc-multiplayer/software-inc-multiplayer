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
using Google.Protobuf.WellKnownTypes;
using System.Buffers;

namespace Multiplayer.Networking.Client
{
    public class GameClient : IDisposable
    {
        public GameClientSocket Socket { get; private set; }
        private readonly ArrayPool<byte> bufferPool = ArrayPool<byte>.Create();

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
            var anyMessage = Any.Pack(message);
            var messageSize = anyMessage.CalculateSize();

            var buffer = bufferPool.Rent(messageSize);

            anyMessage.WriteTo(buffer);
            fixed (byte *p = buffer)
            {
                this.Socket.Connection.SendMessage((IntPtr)p, messageSize);
            }

            bufferPool.Return(buffer);
        }

        public void Dispose()
        {
        }
    }
}
