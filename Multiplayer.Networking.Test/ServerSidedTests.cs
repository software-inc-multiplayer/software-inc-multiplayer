using Xunit;
using System;
using System.Threading;

using Multiplayer.Networking.Server;
using Multiplayer.Networking.Server.Managers;
using Multiplayer.Networking.Utility;
using Multiplayer.Networking.Shared;
using Packets;
using Multiplayer.Networking.Server.Handlers;
using System.Collections.Generic;
using Multiplayer.Networking.Client;

namespace Multiplayer.Networking.Test
{
    public class ServerSidedTests : IDisposable
    {
        private static int _serverPort = 1400;
        private readonly int serverPort;
        private readonly ServerInfo serverInfo;

        private readonly TestLogger logger;
        private readonly PacketSerializer packetSerializer;
        private readonly GameServer_old server;
        private readonly GameUser testUser = new GameUser()
        {
            Id = 0123456789,
            Name = "test-user",
            Role = UserRole.Host
        };

        public ServerSidedTests()
        {
            this.serverPort = Interlocked.Increment(ref _serverPort);
            this.serverInfo = new ServerInfo() { Port = (ushort)this.serverPort, Name = "testserver", DefaultRole = UserRole.Host };
            this.logger = new TestLogger();
            this.packetSerializer = new PacketSerializer();
            this.server = new GameServer_old(this.logger, this.packetSerializer, new UserManager(), new BanManager());
        }

        private GameClient_old CreateClient()
        {
            var client = new GameClient_old(this.logger, this.testUser, this.packetSerializer, new UserManager());
            client.Connect("localhost", (ushort)serverPort);
            return client;
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
            Assert.Null(server.ServerInfo);
        }

        [Fact]
        public void StartStop()
        {
            var startedEventFired = false;
            var stoppedEventFired = false;

            server.ServerStarted += (sender, e) => { startedEventFired = true; };
            server.ServerStopped += (sender, e) => { stoppedEventFired = true; };

            server.Start(this.serverInfo);

            Assert.NotNull(server.RawServer);
            //Assert.True(server.RawServer.Active);

            Assert.NotNull(server.ServerInfo);
            
            Assert.Equal(serverPort, server.ServerInfo.Port);
            Assert.Equal("", server.ServerInfo.Password);

            server.Stop();

            //Assert.False(server.RawServer.Active);

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
                //Assert.NotEqual(-1, e.ConnectionId);
                //connectionId = e.ConnectionId;
                //Assert.False(e.Cancel);
            };
            server.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                Assert.NotEqual(-1, connectionId);
                //Assert.Equal(connectionId, e.ConnectionId);
            };

            server.Start(this.serverInfo);

            using var client = this.CreateClient();
            client.Connect("127.0.0.1", serverInfo.Port);

            server.SafeHandleMessages();

            Assert.True(clientConnectedFired);
            Assert.NotEqual(-1, connectionId);
            //Assert.Contains(connectionId, server.ConnectedClients);
            Assert.Single(server.ConnectedClients);

            //Assert.True(client.RawClient.Connected);

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
                //Assert.NotEqual(-1, e.ConnectionId);
                //connectionId = e.ConnectionId;
                //e.Cancel = true;
            };
            
            server.ClientDisconnected += (sender, e) => {
                clientDisconnectedFired = true;
                Assert.NotEqual(-1, connectionId);
                //Assert.Equal(connectionId, e.ConnectionId);
            };

            server.Start(this.serverInfo);

            using var client = this.CreateClient();

            server.SafeHandleMessages();

            Assert.Empty(server.ConnectedClients);

            //Assert.False(client.RawClient.Connected);

            server.SafeHandleMessages();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }

        [Fact]
        public void ClientHandshake()
        {
            var clientConnectedFired = false;
            var handshakeReceived = false;

            var connectionId = -1;

            server.ClientConnected += (sender, e) => {
                clientConnectedFired = true;
                //Assert.NotEqual(-1, e.ConnectionId);
                //connectionId = e.ConnectionId;
                //Assert.False(e.Cancel);
            };
            /*server.ReceivedPacket += (sender, e) =>
            {
                if (e.Handled)
                    return;
                handshakeReceived = e.Handled = e.Packet is Handshake;
            };*/
            //server.RegisterPacketHandler();

            server.Start(this.serverInfo);

            using var client = this.CreateClient();

            server.SafeHandleMessages(); // finish server connected
            client.SafeHandleMessages(); // trigger handshake
            server.SafeHandleMessages(); // handle client handshake

            Assert.True(handshakeReceived);

            //Assert.True(client.RawClient.Connected);

            client.Disconnect();

            Assert.True(clientConnectedFired);
        }

        [Fact(Skip = "It's not ready yet")]
        public void ClientDoubleHandshake()
        {
            /*server.ReceivedPacket += (sender, e) =>
            {
                if (e.Handled)
                    return;
                e.Handled |= e.Packet is Handshake;
            };*/
            server.Start(this.serverInfo);

            using var client = this.CreateClient();

            server.SafeHandleMessages(); // finish server connected
            client.SafeHandleMessages(); // trigger handshake
            server.SafeHandleMessages(); // handle client handshake

            var handshake = new Handshake(0123456789, "placeholder");
            //client.RawClient.Send(this.packetSerializer.SerializePacket(handshake));
            server.SafeHandleMessages();
            client.SafeHandleMessages();

            // TODO dont know what to assert here as this is undefined behaviour so far
        }

        [Fact]
        public void ClientHandshakeDisconnect()
        {
            var disconnectReceived = false;
            /*server.ReceivedPacket += (sender, e) =>
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
            };*/

            server.Start(this.serverInfo);

            using var client = this.CreateClient();

            server.SafeHandleMessages(); // finish server connected
            client.SafeHandleMessages(); // trigger handshake
            server.SafeHandleMessages(); // handle client handshake

            client.Disconnect();

            server.SafeHandleMessages();
            client.SafeHandleMessages();
            //server.SafeHandleMessages();

            Assert.True(disconnectReceived);
        }
    
        [Fact]
        public void ServerWithChatHandler()
        {
            var chatHandler = new ChatHandler(this.server);
            this.server.RegisterPacketHandler(chatHandler);
            
            this.server.Start(this.serverInfo);

            using var client = this.CreateClient();

            server.SafeHandleMessages(); // finish server connected
            client.SafeHandleMessages(); // trigger handshake
            server.SafeHandleMessages(); // handle client handshake

            client.Disconnect();

            server.SafeHandleMessages();
            client.SafeHandleMessages();

            //Assert.Equal(new Dictionary<Type, List<IPacketHandler>>() { chatHandler } ,this.server.PacketHandlers);
        }
    }
}
