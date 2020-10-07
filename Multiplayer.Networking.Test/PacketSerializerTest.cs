using System;
using System.Collections.Generic;
using System.Text;
using MessagePack;
using Multiplayer.Networking.Utility;
using Xunit;

namespace Multiplayer.Networking.Test
{
    public class PacketSerializerTest
    {
        [Fact()]
        public void InvalidType()
        {
            var serializer = new PacketSerializer();
            var randomObject = new { Field1 = "test" };

            var wrongObjectData = MessagePackSerializer.Typeless.Serialize(randomObject);
            Assert.NotNull(wrongObjectData);

            var wrongPacket = serializer.DeserializePacket(wrongObjectData);
            Assert.Null(wrongPacket);
        }
    }
}
