using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

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

        [Serializable]
        private class RandomObject
        {
            public string Field1 { get; set; }
        }

        [Fact()]
        public void InvalidType()
        {
            var randomObject = new RandomObject() { Field1 = "test" };

            using var ms = new MemoryStream();
            var formatter = new BinaryFormatter();
            formatter.Serialize(ms, randomObject);
            var wrongObjectData = ms.ToArray();

            //var wrongObjectData = MessagePackSerializer.Typeless.Serialize(randomObject, this.packetSerializer.Options);
            Assert.NotNull(wrongObjectData);

            var wrongPacket = this.packetSerializer.DeserializePacket(wrongObjectData);
            Assert.Null(wrongPacket);
        }

        [Fact()]
        public void HandshakePacket()
        {
            var userId = 1234567890UL;
            var userName = "<placeholder>";
            var password = "<test123>";
            var packet = new Handshake(userId, userName, password);

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

            Assert.Equal(userId, deserializedTypedPacket.Sender);
            Assert.Equal(userName, deserializedTypedPacket.UserName);
            Assert.Equal(password, deserializedTypedPacket.Password);
        }
    }
}
