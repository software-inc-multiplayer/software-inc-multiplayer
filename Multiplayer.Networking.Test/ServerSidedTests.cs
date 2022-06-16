using System;
using System.Threading;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Server;
using Multiplayer.Networking.Shared;
using Xunit;

namespace Multiplayer.Networking.Test
{
    public class ServerSidedTests : IDisposable
    {
        private static int _serverPort = 1400;
        private readonly int serverPort;
        private readonly ServerInfo serverInfo = new() { Port = (ushort)_serverPort, Name = "testserver", DefaultRole = UserRole.Host };

        private readonly TestLogger logger;
        private readonly GameServer server;
        private readonly GameUser testUser = new GameUser()
        {
            Id = 0123456789,
            Name = "test-user",
            Role = UserRole.Host
        };

        public ServerSidedTests()
        {
            this.serverPort = Interlocked.Increment(ref _serverPort);
            this.logger = new TestLogger();
            this.server = new GameServer(this.logger);
        }

        private GameClient CreateClient()
        {
            var client = new GameClient(this.logger, this.testUser);
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
            Assert.NotNull(server.SocketManager);
            Assert.Null(server.ServerInfo);
        }

        [Fact]
        public void StartStop()
        {
            var startedEventFired = false;
            var stoppedEventFired = false;

            server.ServerStarted += (sender, e) => { startedEventFired = true; };
            server.ServerStopped += (sender, e) => { stoppedEventFired = true; };

            server.Start(serverInfo);

            Assert.NotNull(server.SocketManager);
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
                // Assert.NotEqual(-1, connectionId);
                //Assert.Equal(connectionId, e.ConnectionId);
            };

            server.Start(this.serverInfo);

            using var client = this.CreateClient();
            client.Connect("127.0.0.1", serverInfo.Port);

            Assert.True(clientConnectedFired);
            Assert.NotEqual(-1, connectionId);
            //Assert.Contains(connectionId, server.ConnectedClients);
            Assert.Equal(1, server.UserManager.Count());

            //Assert.True(client.RawClient.Connected);

            client.Disconnect();
            
            Assert.Equal(0, server.UserManager.Count());

            // Assert.True(clientDisconnectedFired);
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
                //Assert.Equal(connectionId, e.ConnectionId);
            };

            server.Start(this.serverInfo);

            using var client = this.CreateClient();
            
            Assert.Equal(0, server.UserManager.Count());
        }

        // [Fact]
        // public void ClientHandshake()
        // {
        //     var clientConnectedFired = false;
        //     var handshakeReceived = false;
        //
        //     var connectionId = -1;
        //
        //     server.ClientConnected += (sender, e) => {
        //         clientConnectedFired = true;
        //         //Assert.NotEqual(-1, e.ConnectionId);
        //         //connectionId = e.ConnectionId;
        //         //Assert.False(e.Cancel);
        //     };
        //     /*server.ReceivedPacket += (sender, e) =>
        //     {
        //         if (e.Handled)
        //             return;
        //         handshakeReceived = e.Handled = e.Packet is Handshake;
        //     };*/
        //     //server.RegisterPacketHandler();
        //
        //     server.Start(this.serverInfo);
        //
        //     using var client = this.CreateClient();
        //
        //     Assert.True(handshakeReceived);
        //
        //     //Assert.True(client.RawClient.Connected);
        //
        //     client.Disconnect();
        //
        //     Assert.True(clientConnectedFired);
        // }
        //
        // [Fact(Skip = "It's not ready yet")]
        // public void ClientDoubleHandshake()
        // {
        //     /*server.ReceivedPacket += (sender, e) =>
        //     {
        //         if (e.Handled)
        //             return;
        //         e.Handled |= e.Packet is Handshake;
        //     };*/
        //     server.Start(this.serverInfo);
        //
        //     using var client = this.CreateClient();
        //
        //     server.SafeHandleMessages(); // finish server connected
        //     client.SafeHandleMessages(); // trigger handshake
        //     server.SafeHandleMessages(); // handle client handshake
        //
        //     var handshake = new Handshake(0123456789, "placeholder");
        //     //client.RawClient.Send(this.packetSerializer.SerializePacket(handshake));
        //     server.SafeHandleMessages();
        //     client.SafeHandleMessages();
        //
        //     // TODO dont know what to assert here as this is undefined behaviour so far
        // }
        //
        // [Fact]
        // public void ClientHandshakeDisconnect()
        // {
        //     var disconnectReceived = false;
        //     /*server.ReceivedPacket += (sender, e) =>
        //     {
        //         if (e.Handled)
        //             return;
        //         e.Handled |= e.Packet is Handshake;
        //     };
        //     server.ReceivedPacket += (sender, e) =>
        //     {
        //         if (e.Handled)
        //             return;
        //         if(e.Packet is Disconnect dc)
        //         {
        //             disconnectReceived = true;
        //             Assert.Equal(DisconnectReason.Leaving, dc.Reason);
        //         }
        //         e.Handled |= disconnectReceived;
        //     };*/
        //
        //     server.Start(this.serverInfo);
        //
        //     using var client = this.CreateClient();
        //
        //     server.SafeHandleMessages(); // finish server connected
        //     client.SafeHandleMessages(); // trigger handshake
        //     server.SafeHandleMessages(); // handle client handshake
        //
        //     client.Disconnect();
        //
        //     server.SafeHandleMessages();
        //     client.SafeHandleMessages();
        //     //server.SafeHandleMessages();
        //
        //     Assert.True(disconnectReceived);
        // }
        //
        // [Fact]
        // public void ServerWithChatHandler()
        // {
        //     var chatHandler = new ChatHandler(this.server);
        //     this.server.RegisterPacketHandler(chatHandler);
        //     
        //     this.server.Start(this.serverInfo);
        //
        //     using var client = this.CreateClient();
        //
        //     server.SafeHandleMessages(); // finish server connected
        //     client.SafeHandleMessages(); // trigger handshake
        //     server.SafeHandleMessages(); // handle client handshake
        //
        //     client.Disconnect();
        //
        //     server.SafeHandleMessages();
        //     client.SafeHandleMessages();
        //
        //     //Assert.Equal(new Dictionary<Type, List<IPacketHandler>>() { chatHandler } ,this.server.PacketHandlers);
        // }
    }
}
