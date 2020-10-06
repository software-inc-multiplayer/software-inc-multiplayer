using System;
using Xunit;
using Multiplayer.Networking;
using System.Threading;

namespace Multiplayer.Networking.Test
{
    public class ServerSidedTests
    {
        private static readonly int serverPort = 1338;
        private static readonly int networkDelay = 10;
        private TestLogger logger;

        public ServerSidedTests()
        {
            this.logger = new TestLogger();
        }

        [Fact]
        public void Bootup()
        {
            var server = new Server(this.logger);
            // maybe we can create these upfront?
            Assert.Null(server.RawServer);
            Assert.Null(server.ServerInfomation);
        }

        [Fact]
        public void StartStop()
        {
            var startedEventFired = false;
            var stoppedEventFired = false;

            var server = new Server(this.logger);
            server.ServerStarted += (sender, e) => { startedEventFired = true; };
            server.ServerStopped += (sender, e) => { stoppedEventFired = true; };

            server.Start(serverPort);

            Assert.NotNull(server.RawServer);
            Assert.True(server.RawServer.Active);

            Assert.NotNull(server.ServerInfomation);
            
            Assert.Equal(1337, server.ServerInfomation.Port);
            Assert.Equal("", server.ServerInfomation.Password);

            server.Stop();

            Assert.False(server.RawServer.Active);

            Assert.True(startedEventFired);
            Assert.True(stoppedEventFired);
        }

        [Fact]
        public void ClientConnected()
        {
            var clientConnectedFired = false;
            var clientDisconnectedFired = false;

            var server = new Server(this.logger);
            server.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                Assert.Equal(1, e.ConnectionId);
                Assert.False(e.Cancel);
            };
            server.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                Assert.Equal(1, e.ConnectionId);
            };

            server.Start(serverPort);

            var client = new Client(logger);
            client.Connect("localhost", serverPort);

            Thread.Sleep(networkDelay);

            server.HandleMessages();

            Assert.Contains(1, server.ConnectedClients);

            Assert.True(client.RawClient.Connected);

            client.Disconnect();

            server.HandleMessages();
            server.Stop();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }

        [Fact]
        public void DenyClient()
        {
            var clientConnectedFired = false;
            var clientDisconnectedFired = false;

            var server = new Server(this.logger);
            server.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                e.Cancel = true;
            };
            
            server.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                Assert.Equal(1, e.ConnectionId);
            };

            server.Start(serverPort);

            var client = new Client(logger);
            client.Connect("localhost", serverPort);

            Thread.Sleep(networkDelay);

            server.HandleMessages();

            Assert.DoesNotContain(1, server.ConnectedClients);

            Assert.False(client.RawClient.Connected);

            server.Stop();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }
    }
}