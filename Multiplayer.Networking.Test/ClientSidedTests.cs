using Xunit;
using System.Threading;

using Multiplayer.Networking;
using Multiplayer.Networking.Utility;
using System;

namespace Multiplayer.Networking.Test
{
    public class ClientSidedTests : IDisposable
    {
        private static int _serverPort = 1300;
        private readonly int serverPort;

        private readonly Server server;
        private readonly TestLogger logger;
        private readonly PacketSerializer packetSerializer;

        private readonly Client client;

        public ClientSidedTests()
        {
            this.serverPort = Interlocked.Increment(ref _serverPort);
            this.logger = new TestLogger();
            this.packetSerializer = new PacketSerializer();
            this.server = new Server(this.logger, this.packetSerializer);

            this.server.ReceivedPacket += (sender, e) =>
            {
                // dummy server
                e.Handled = true;
            };

            this.server.Start(serverPort);

            this.client = new Client(this.logger, this.packetSerializer);
        }

        public void Dispose()
        {
            this.server.Dispose();
            this.client.Dispose();
        }


        [Fact]
        public void Bootup()
        {
            Assert.NotNull(client); // maybe expand on this :)
        }

        [Fact]
        public void ConnectAndDisconnectServer()
        {
            var clientConnectedFired = false;
            var clientDisconnectedFired = false;
            var connectionId = -1;

            client.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                Assert.NotEqual(-1, e.ConnectionId);
                connectionId = e.ConnectionId;
            };
            client.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                Assert.NotEqual(-1, connectionId);
                Assert.Equal(connectionId, e.ConnectionId);
            };

            Assert.False(client.RawClient.Connecting);
            Assert.False(client.RawClient.Connected);

            client.Connect("localhost", serverPort);

            Assert.True(client.RawClient.Connecting);
            Assert.False(client.RawClient.Connected);

            //server.SafeHandleMessages();
            client.SafeHandleMessages();
            //server.SafeHandleMessages();

            Assert.False(client.RawClient.Connecting);
            Assert.True(client.RawClient.Connected);

            client.Disconnect();

            server.SafeHandleMessages();

            Assert.False(client.RawClient.Connecting);
            Assert.False(client.RawClient.Connected);

            client.SafeHandleMessages();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }
    }
}
