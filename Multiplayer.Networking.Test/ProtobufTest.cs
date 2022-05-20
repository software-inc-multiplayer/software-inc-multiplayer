using System;
using Google.Protobuf.WellKnownTypes;
using Multiplayer.Packets;
using Xunit;
using Xunit.Abstractions;

namespace Multiplayer.Networking.Test
{
    public class ProtobufTest
    {
        private readonly ITestOutputHelper logger;

        public ProtobufTest(ITestOutputHelper logger) {
            this.logger = logger;
        }

        [Fact]
        public void TypeExperiments()
        {
            logger.WriteLine(Handshake.Descriptor.Name);
            logger.WriteLine(Handshake.Descriptor.FullName);

            var x = new Handshake();
            var anyMessage = Any.Pack(x);

            ReadOnlySpan<char> typeString = anyMessage.TypeUrl;
            var split = typeString.IndexOf('/') + 1;
            var type = typeString[split..];

            Assert.Equal(Handshake.Descriptor.FullName, type.ToString());
        }
    }
}