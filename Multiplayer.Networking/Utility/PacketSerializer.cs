//#define PACKET_ID
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MessagePack;
using Multiplayer.Networking.Packet;

namespace Multiplayer.Networking.Utility
{
    public class PacketSerializer
#if PACKET_ID
        : IDisposable
#endif
    {
        private readonly MessagePackSerializerOptions options;

#if PACKET_ID
        private MemoryStream packetStream = new MemoryStream();
        /// <summary>
        /// This is the central place to register known packet types
        /// </summary>
        private static readonly IReadOnlyDictionary<byte, Type> PacketMapping = new Dictionary<byte, Type>()
        {
            { 1, typeof(TestPacket) },
        };

        private static readonly IReadOnlyDictionary<Type, byte> ReversePacketMapping;

        static PacketSerializer() {
            ReversePacketMapping = PacketMapping.ToDictionary(x => x.Value, x => x.Key);
        }
#endif
        public PacketSerializer()
        {
            // see https://github.com/neuecc/MessagePack-CSharp#security
            this.options = MessagePackSerializerOptions.Standard
                .WithSecurity(MessagePackSecurity.UntrustedData);
        }

        public byte[] SerializePacket<TPacket>(TPacket packet)
            where TPacket : IPacket
        {
            // TODO maybe we need locking here
#if PACKET_ID
            /*if (!ReversePacketMapping.TryGetValue(typeof(TPacket), out var packetId))
                return null;
            // this is a variant with a dedicated packetId
            this.packetStream.WriteByte(packetId);
            MessagePackSerializer.Serialize(this.packetStream, packet, this.options);

            // create a byte[] copy of the stream contents
            var serializedPacket = this.packetStream.ToArray();
            this.packetStream.SetLength(0L);

            return serializedPacket;*/
#else
            return MessagePackSerializer.Typeless.Serialize(packet, this.options);
#endif
        }

        public IPacket DeserializePacket(byte[] buffer)
        {
#if PACKET_ID
            var memoryBuffer = new Memory<byte>(buffer);
            var packetId = memoryBuffer.Slice(0, 1).Span[0];
            var packetData = memoryBuffer.Slice(1);

            if (!PacketMapping.TryGetValue(packetId, out var packetType))
                return null;

            return MessagePackSerializer.Deserialize(packetType, packetData, this.options) as IPacket;
#else
            return MessagePackSerializer.Typeless.Deserialize(buffer, this.options) as IPacket;
#endif
        }

#if PACKET_ID
        public void Dispose()
        {
            this.packetStream?.Dispose();
            this.packetStream = null;
        }

        ~PacketSerializer()
        {
            this.Dispose();
        }
#endif
    }

    
}

namespace Multiplayer.Networking.Packet
{
    public interface IPacket
    {
    }
    [MessagePackObject]
    public class Handshake : IPacket
    {

    }
}
