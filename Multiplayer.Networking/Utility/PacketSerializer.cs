//#define PACKET_ID

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using MessagePack;
using Multiplayer.Networking;
using Packets;

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
            /*var resolver = MessagePack.Resolvers.CompositeResolver.Create(
                new[] { MessagePack.Formatters.TypelessFormatter.Instance },
                new[] { MessagePack.Resolvers.StandardResolver.Instance });*/

            this.Options = MessagePackSerializer.Typeless.DefaultOptions//MessagePackSerializerOptions.Standard
                .WithOmitAssemblyVersion(true)
                //.WithResolver(resolver)
                //.WithCompression(MessagePackCompression.Lz4Block)
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

/// <summary>
/// Keep this namespace SHORT as it is used in serialization
/// </summary>
namespace Packets
{
    public interface IPacket
    {
        string Sender { get; }
    }

    [MessagePackObject]
    public class Handshake : IPacket
    {
        [Key(0)]
        public string Sender { get; }
        [Key(1)]
        public string UserName { get; set; }
        [Key(2)]
        public string Password { get; set; }

        public Handshake(string userId, string userName, string password = null)
        {
            this.Sender = userId;
            this.UserName = userName;
            this.Password = password;
        }

        // TODO in the future we need to use the steam networking library to verify the user identity via steam tokens
    }

    [MessagePackObject]
    public class WelcomeUser : IPacket
    {
        [Key(0)]
        public string Sender { get; }
        [Key(1)]
        public string UserName { get; set; }

        public WelcomeUser(string userId, string userName)
        {
            this.Sender = userId;
            this.UserName = userName;
        }
    }



    /// <summary>
    /// this enforces a clean disconnect
    /// </summary>
    [MessagePackObject]
    public class Disconnect : IPacket
    {
        [Key(0)]
        public string Sender { get; }
        [Key(1)]
        public string Target { get; }
        [Key(2)]
        public DisconnectReason Reason { get; }

        public Disconnect(string sender, DisconnectReason reason)
        {
            this.Sender = sender;
            this.Reason = reason;
        }
    }

    [MessagePackObject]
    public class ChatMessage : IPacket
    {
        [Key(0)]
        public string Sender { get; }
        [Key(1)]
        public string Message { get; }

        public ChatMessage(string sender, string message)
        {
            this.Sender = sender;
            this.Message = message;
        }
    }

    [MessagePackObject]
    public class PrivateChatMessage : IPacket
    {
        [Key(0)]
        public string Sender { get; }
        [Key(1)]
        public string Receiver { get; }
        [Key(2)]
        public string Message { get; }

        public PrivateChatMessage(string sender, string receiver, string message)
        {
            this.Sender = sender;
            this.Receiver = receiver;
            this.Message = message;
        }
    }
}
