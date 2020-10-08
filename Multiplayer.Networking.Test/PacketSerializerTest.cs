using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Multiplayer.Networking.Packet;
using Multiplayer.Networking.Utility;
using Xunit;
using Xunit.Abstractions;

namespace Multiplayer.Networking.Test
{
    public class PacketSerializerTest
    {
        private readonly PacketSerializer packetSerializer;
        private readonly ITestOutputHelper output;

        public PacketSerializerTest(ITestOutputHelper output)
        {
            this.packetSerializer = new PacketSerializer();
            this.output = output;
        }

        [Fact()]
        public void InvalidType()
        {
            var randomObject = new { Field1 = "test" };

            var wrongObjectData = MessagePackSerializer.Typeless.Serialize(randomObject, this.packetSerializer.Options);
            Assert.NotNull(wrongObjectData);

            var wrongPacket = this.packetSerializer.DeserializePacket(wrongObjectData);
            Assert.Null(wrongPacket);
        }

        [Fact()]
        public void HandshakePacket()
        {
            var packet = new Handshake(new User());

            var serializedPacket = this.packetSerializer.SerializePacket(packet);
            Assert.NotNull(serializedPacket);
            Assert.NotEmpty(serializedPacket);

            var deserializedPacket = this.packetSerializer.DeserializePacket(serializedPacket);
            var deserializedTypedPacket = deserializedPacket as Handshake;
            Assert.NotNull(deserializedPacket);
            Assert.NotNull(deserializedTypedPacket);

            Assert.Equal(packet.User.UniqueID, deserializedTypedPacket.User.UniqueID);
        }

        [Fact()]
        public void TestPacket()
        {
            var packet = new TestPacket() { TestString = "test" };

            var serializedPacket = this.packetSerializer.SerializePacket(packet);
            output.WriteLine(MessagePackSerializer.ConvertToJson(serializedPacket));
            Assert.NotNull(serializedPacket);
            Assert.NotEmpty(serializedPacket);

            var deserializedPacket = this.packetSerializer.DeserializePacket(serializedPacket);
            var deserializedTypedPacket = deserializedPacket as TestPacket;
            Assert.NotNull(deserializedPacket);
            Assert.NotNull(deserializedTypedPacket);

            Assert.Equal(packet.TestString, deserializedTypedPacket.TestString);
        }
    }
}
