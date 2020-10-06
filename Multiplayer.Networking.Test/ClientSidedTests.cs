using System;
using Xunit;
using Multiplayer.Networking;
using Newtonsoft.Json.Serialization;
using System.Threading;

namespace Multiplayer.Networking.Test
{
    public class ClientSidedTests
    {
        private readonly Server server;
        private static readonly int serverPort = 1337;
        private static readonly int networkDelay = 10;
        private readonly TestLogger logger;

        public ClientSidedTests()
        {
            this.logger = new TestLogger();
            this.server = new Server(this.logger);
            this.server.Start(serverPort);
        }


        [Fact]
        public void Bootup()
        {
            var client = new Client(this.logger);
            Assert.NotNull(client);
        }

        [Fact]
        public void ConnectAndDisconnectServer()
        {
            var server = new Server(this.logger);
            server.Start(serverPort);
            server.HandleMessages();

            var clientConnectedFired = false;
            var clientDisconnectedFired = false;

            var client = new Client(this.logger);
            client.ClientConnected += (sender, e) => clientConnectedFired = true;
            client.ClientDisconnected += (sender, e) => clientDisconnectedFired = true;

            client.Connect("localhost", serverPort);

            Assert.True(client.RawClient.Connecting);
            Assert.False(client.RawClient.Connected);

            Thread.Sleep(networkDelay);

            client.HandleMessages();

            Assert.False(client.RawClient.Connecting);
            Assert.True(client.RawClient.Connected);

            client.Disconnect();

            Assert.False(client.RawClient.Connecting);
            Assert.False(client.RawClient.Connected);

            client.HandleMessages();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }
    }
}
