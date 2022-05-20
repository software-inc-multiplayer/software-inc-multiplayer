using System;
using System.Buffers;
using System.IO;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Server
{
    public class GameServerSocket : SocketManager
    {
        private readonly ArrayPool<byte> bufferPool = ArrayPool<byte>.Create();
        public override void OnConnected(Connection connection, ConnectionInfo info)
        {
            this.Connected?.Invoke(connection, info);
        }

        public event Action<Connection, ConnectionInfo> Connected;

        public override void OnConnecting(Connection connection, ConnectionInfo info)
        {
            var result = connection.Accept();
        }

        public override void OnDisconnected(Connection connection, ConnectionInfo info)
        {

        }

        public unsafe override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            var buffer = this.bufferPool.Rent(size);
            Marshal.Copy(data, buffer, 0, size);
            var anyMessage = Any.Parser.ParseFrom(buffer, 0, size);
            this.bufferPool.Return(buffer);

            ReadOnlySpan<char> typeString = anyMessage.TypeUrl;
            var split = typeString.IndexOf('/') + 1;
            var messageType = typeString[split..].ToString();

            this.MessageReceived?.Invoke(connection, anyMessage, messageType);
        }

        public event Action<Connection, Any, string> MessageReceived;
    }
}
