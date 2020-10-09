using Xunit;
using System;
using System.Threading;

using Multiplayer.Networking;
using Multiplayer.Networking.Utility;
using Packets;

namespace Multiplayer.Networking.Test
{
    public class ServerSidedTests : IDisposable
    {
        private static int _serverPort = 1400;
        private readonly int serverPort;

        private readonly TestLogger logger;
        private readonly PacketSerializer packetSerializer;
        private readonly Server server;

        public ServerSidedTests()
        {
            this.serverPort = Interlocked.Increment(ref _serverPort);
            this.logger = new TestLogger();
            this.packetSerializer = new PacketSerializer();
            this.server = new Server(this.logger, this.packetSerializer);
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

            var connectionId = -1;

            server.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                Assert.NotEqual(-1, e.ConnectionId);
                connectionId = e.ConnectionId;
                Assert.False(e.Cancel);
            };
            server.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                Assert.NotEqual(-1, connectionId);
                Assert.Equal(connectionId, e.ConnectionId);
            };
            server.ReceivedPacket += (sender, e) =>
            {
                if (e.Packet is Disconnect)
                    e.Handled = true;
            };

            server.Start(serverPort);

            var client = new Client(this.logger, this.packetSerializer);
            client.Connect("localhost", serverPort);

            server.SafeHandleMessages();

            Assert.True(clientConnectedFired);
            Assert.NotEqual(-1, connectionId);
            Assert.Contains(connectionId, server.ConnectedClients);
            Assert.Single(server.ConnectedClients);

            Assert.True(client.RawClient.Connected);

            client.Disconnect();

            server.SafeHandleMessages();
            client.SafeHandleMessages();
            server.SafeHandleMessages();

            Assert.True(clientDisconnectedFired);
        }

        [Fact]
        public void DenyClient()
        {
            var clientConnectedFired = false;
            var clientDisconnectedFired = false;

            var connectionId = -1;

            server.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                Assert.NotEqual(-1, e.ConnectionId);
                connectionId = e.ConnectionId;
                e.Cancel = true;
            };
            
            server.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                Assert.NotEqual(-1, connectionId);
                Assert.Equal(connectionId, e.ConnectionId);
            };

            server.Start(serverPort);

            var client = new Client(this.logger, this.packetSerializer);
            client.Connect("localhost", serverPort);

            server.SafeHandleMessages();

            Assert.Empty(server.ConnectedClients);

            Assert.False(client.RawClient.Connected);

            server.SafeHandleMessages();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }

        [Fact]
        public void ClientHandshake()
        {
            var clientConnectedFired = false;
            var handshakeReceived = true;

            var connectionId = -1;

            server.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                Assert.NotEqual(-1, e.ConnectionId);
                connectionId = e.ConnectionId;
                Assert.False(e.Cancel);
            };
            server.ReceivedPacket += (sender, e) =>
            {
                if (e.Handled)
                    return;
                handshakeReceived = e.Handled = e.Packet is Handshake;
            };

            server.Start(serverPort);

            var client = new Client(this.logger, this.packetSerializer);
            client.Connect("localhost", serverPort);

            server.SafeHandleMessages(); // finish server connected
            client.SafeHandleMessages(); // trigger handshake
            server.SafeHandleMessages(); // handle client handshake

            Assert.True(handshakeReceived);

            Assert.True(client.RawClient.Connected);

            client.Disconnect();

            Assert.True(clientConnectedFired);
        }

        [Fact(Skip = "It's not ready yet")]
        public void ClientDoubleHandshake()
        {
            server.ReceivedPacket += (sender, e) =>
            {
                if (e.Handled)
                    return;
                e.Handled |= e.Packet is Handshake;
            };
            server.Start(serverPort);

            var client = new Client(this.logger, this.packetSerializer);
            client.Connect("localhost", serverPort);

            server.SafeHandleMessages(); // finish server connected
            client.SafeHandleMessages(); // trigger handshake
            server.SafeHandleMessages(); // handle client handshake

            var handshake = new Handshake(new User(true));
            client.RawClient.Send(this.packetSerializer.SerializePacket(handshake));
            server.SafeHandleMessages();
            client.SafeHandleMessages();

            // TODO dont know what to assert here as this is undefined behaviour so far
        }

        [Fact]
        public void ClientHandshakeDisconnect()
        {
            var disconnectReceived = false;
            server.ReceivedPacket += (sender, e) =>
            {
                if (e.Handled)
                    return;
                e.Handled |= e.Packet is Handshake;
            };
            server.ReceivedPacket += (sender, e) =>
            {
                if (e.Handled)
                    return;
                if(e.Packet is Disconnect dc)
                {
                    disconnectReceived = true;
                    Assert.Equal(DisconnectReason.Leaving, dc.Reason);
                }
                e.Handled |= disconnectReceived;
            };

            server.Start(serverPort);

            var client = new Client(this.logger, this.packetSerializer);
            client.Connect("localhost", serverPort);

            server.SafeHandleMessages(); // finish server connected
            client.SafeHandleMessages(); // trigger handshake
            server.SafeHandleMessages(); // handle client handshake

            client.Disconnect();

            server.SafeHandleMessages();
            client.SafeHandleMessages();
            //server.SafeHandleMessages();

            Assert.True(disconnectReceived);
        }
    }
}
