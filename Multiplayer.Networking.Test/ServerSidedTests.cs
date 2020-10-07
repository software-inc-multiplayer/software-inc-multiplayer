using System;
using Xunit;
using Multiplayer.Networking;
using System.Threading;
using Multiplayer.Networking.Utility;
using System.Diagnostics;
using Xunit.Abstractions;

namespace Multiplayer.Networking.Test
{
    public class ServerSidedTests : IDisposable
    {
        private static int _serverPort = 1400;
        private readonly TestLogger logger;
        private readonly PacketSerializer packetSerializer;
        private readonly Server server;
        private readonly int serverPort;
        private readonly ITestOutputHelper output;

        public ServerSidedTests(ITestOutputHelper output)
        {
            this.logger = new TestLogger();
            this.packetSerializer = new PacketSerializer();
            this.server = new Server(this.logger, this.packetSerializer);
            this.serverPort = Interlocked.Increment(ref _serverPort);
            output.WriteLine($"Port: {serverPort}");
            this.output = output;
        }

        public void Dispose()
        {
            this.server.Dispose();
        }

        [Fact]
        public void Bootup()
        {
            // maybe we can create these upfront?
            Assert.NotNull(server.RawServer);
            Assert.Null(server.ServerInfomation);
        }

        [Fact]
        public void StartStop()
        {
            var startedEventFired = false;
            var stoppedEventFired = false;

            server.ServerStarted += (sender, e) => { startedEventFired = true; };
            server.ServerStopped += (sender, e) => { stoppedEventFired = true; };

            server.Start(serverPort);

            Assert.NotNull(server.RawServer);
            Assert.True(server.RawServer.Active);

            Assert.NotNull(server.ServerInfomation);
            
            Assert.Equal(serverPort, server.ServerInfomation.Port);
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

            var connectionId = 0;

            server.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                connectionId = e.ConnectionId;
                Assert.False(e.Cancel);
            };
            server.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                Assert.NotEqual(0, connectionId);
                Assert.Equal(connectionId, e.ConnectionId);
            };

            server.Start(serverPort);

            var client = new Client(this.logger, this.packetSerializer);
            client.Connect("localhost", serverPort);

            server.SafeHandleMessages();

            Assert.True(clientConnectedFired);
            Assert.NotEqual(0, connectionId);
            Assert.Contains(connectionId, server.ConnectedClients);
            Assert.Single(server.ConnectedClients);

            Assert.True(client.RawClient.Connected);

            client.Disconnect();

            server.SafeHandleMessages();
            server.Stop();

            Assert.True(clientDisconnectedFired);
        }

        [Fact]
        public void DenyClient()
        {
            var clientConnectedFired = false;
            var clientDisconnectedFired = false;

            var server = new Server(this.logger, this.packetSerializer);
            server.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                output.WriteLine($"CC-Port: {serverPort}");
                e.Cancel = true;
            };
            
            server.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                output.WriteLine($"CD-Port: {serverPort}");
                //Assert.Equal(1, e.ConnectionId);
            };

            server.Start(serverPort);

            var client = new Client(this.logger, this.packetSerializer);
            client.Connect("localhost", serverPort);

            server.SafeHandleMessages();

            Assert.Empty(server.ConnectedClients);

            Assert.False(client.RawClient.Connected);

            server.SafeHandleMessages();

            server.Stop();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }

        [Fact]
        public void ClientHandshake()
        {
            var clientConnectedFired = false;
            var clientDisconnectedFired = false;

            var server = new Server(this.logger, this.packetSerializer);
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

            var client = new Client(this.logger, this.packetSerializer);
            client.Connect("localhost", serverPort);

            server.SafeHandleMessages(); // finish server connected
            client.SafeHandleMessages(); // trigger handshake
            server.SafeHandleMessages(); // handle client handshake

            Assert.Contains(1, server.ConnectedClients);

            Assert.True(client.RawClient.Connected);

            client.Disconnect();

            server.SafeHandleMessages();
            server.Stop();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }
    }
}