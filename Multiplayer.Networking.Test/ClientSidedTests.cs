using System;
using Xunit;
using Multiplayer.Networking;
using Newtonsoft.Json.Serialization;
using System.Threading;
using Multiplayer.Networking.Utility;

namespace Multiplayer.Networking.Test
{
    public class ClientSidedTests
    {
        private static readonly int serverPort = 1337;
        private readonly Server server;
        private readonly TestLogger logger;
        private readonly PacketSerializer packetSerializer;

        public ClientSidedTests()
        {
            this.logger = new TestLogger();
            this.packetSerializer = new PacketSerializer();
            this.server = new Server(this.logger, this.packetSerializer);
            this.server.Start(serverPort);
        }


        [Fact]
        public void Bootup()
        {
            var client = new Client(this.logger, this.packetSerializer);
            Assert.NotNull(client);
        }

        [Fact]
        public void ConnectAndDisconnectServer()
        {
            var server = new Server(this.logger, this.packetSerializer);
            server.Start(serverPort);

            var clientConnectedFired = false;
            var clientDisconnectedFired = false;

            var client = new Client(this.logger, this.packetSerializer);
            client.ClientConnected += (sender, e) => clientConnectedFired = true;
            client.ClientDisconnected += (sender, e) => clientDisconnectedFired = true;

            client.Connect("localhost", serverPort);

            Assert.True(client.RawClient.Connecting);
            Assert.False(client.RawClient.Connected);

            //server.SafeHandleMessages();
            client.SafeHandleMessages();
            //server.SafeHandleMessages();

            Assert.False(client.RawClient.Connecting);
            Assert.True(client.RawClient.Connected);

            client.Disconnect();

            Assert.False(client.RawClient.Connecting);
            Assert.False(client.RawClient.Connected);

            client.SafeHandleMessages();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }
    }
}
