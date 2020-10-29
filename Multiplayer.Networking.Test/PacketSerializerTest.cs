using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Packets;
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

            //var wrongObjectData = MessagePackSerializer.Typeless.Serialize(randomObject, this.packetSerializer.Options);
            //Assert.NotNull(wrongObjectData);

            //var wrongPacket = this.packetSerializer.DeserializePacket(wrongObjectData);
            //Assert.Null(wrongPacket);
        }

        [Fact()]
        public void HandshakePacket()
        {
            var packet = new Handshake(1234567890, "placeholder");

            var serializedPacket = this.packetSerializer.SerializePacket(packet);

            Assert.NotNull(serializedPacket);
            Assert.NotEmpty(serializedPacket);

            //output.WriteLine(MessagePackSerializer.ConvertToJson(serializedPacket));

            Assert.NotNull(serializedPacket);
            Assert.NotEmpty(serializedPacket);

            var deserializedPacket = this.packetSerializer.DeserializePacket(serializedPacket);
            Assert.NotNull(deserializedPacket);
            var deserializedTypedPacket = deserializedPacket as Handshake;
            Assert.NotNull(deserializedTypedPacket);

            //Assert.Equal(packet.User.UniqueID, deserializedTypedPacket.User.UniqueID);
        }
    }
}
