//#define PACKET_ID
#define TEST
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
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
        public MessagePackSerializerOptions Options { get; }

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
            var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                new[] { MessagePack.Formatters.TypelessFormatter.Instance },
                new[] { MessagePack.Resolvers.StandardResolver.Instance });
            this.Options = MessagePackSerializerOptions.Standard
                .WithResolver(resolver)
                .WithSecurity(MessagePackSecurity.UntrustedData);


            /*MessagePack.Formatters.TypelessFormatter.Instance = typeName =>
            {
                if (typeName.StartsWith("SomeNamespace"))
                {
                    typeName = typeName.Replace("SomeNamespace", "AnotherNamespace");
                }

                return Type.GetType(typeName, false);
            };*/
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
            return MessagePackSerializer.Typeless.Serialize(packet, this.Options);
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
            return MessagePackSerializer.Typeless.Deserialize(buffer, this.Options) as IPacket;
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
    public class TestPacket : IPacket
    {
        [Key(0)]
        public string TestString { get; set; }
    }

    [MessagePackObject]
    public class Handshake : IPacket
    {
        [Key(0)]
        public User User { get; set; }
        [Key(1)]
        public string Password { get; set; }

        public Handshake(User userId, string password = null)
        {
            this.User = userId;
            this.Password = password;
        }
    }
    /// <summary>
    /// A chat message.
    /// </summary>
    [MessagePackObject]
    public class ChatMessage : IPacket
    {
        [Key(0)]
        public User Sender { get; }

        [Key(1)]
        public string Message { get; }
        [Key(3)]
        public DateTime CreatedDate { get; }
        public ChatMessage(User sender, string message)
        {
            this.Sender = sender; this.Message = message; this.CreatedDate = DateTime.Now;
        }
    }

    /// <summary>
    /// this enforces a clean disconnect
    /// </summary>
    [MessagePackObject]
    public class Disconnect : IPacket
    {
        [Key(0)]
        public Constants.DisconnectReason Reason { get; }

        public Disconnect(Constants.DisconnectReason reason)
        {
            this.Reason = reason;
        }
    }
}
