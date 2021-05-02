using Xunit;
using System;
using System.Threading;

using Multiplayer.Networking.Utility;
using Multiplayer.Networking.Server;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Client;

namespace Multiplayer.Networking.Test
{
    public class ClientSidedTests : IDisposable
    {
        private static int _serverPort = 1300;
        private readonly int serverPort;

        private readonly GameServer_old server;
        private readonly TestLogger logger;
        private readonly PacketSerializer packetSerializer;

        private static readonly ulong userId = 0123456789;
        private readonly GameClient_old client;
        private readonly GameUser testUser = new GameUser()
        {
            Id = userId,
            Name = "test-user",
            Role = UserRole.Host
        };

        public ClientSidedTests()
        {
            this.serverPort = Interlocked.Increment(ref _serverPort);
            this.logger = new TestLogger();
            this.packetSerializer = new PacketSerializer();
            this.server = new GameServer_old(this.logger, this.packetSerializer, new Shared.UserManager(), new Server.Managers.BanManager());

            /*this.server.ReceivedPacket += (sender, e) =>
            {
                // dummy server
                e.Handled = true;
            };*/

            this.server.Start(new ServerInfo()
            {
                Port = (ushort)serverPort,
                Name = "testserver",
                DefaultRole = Shared.UserRole.Host
            });

            this.client = new GameClient_old(this.logger, this.testUser, this.packetSerializer, new UserManager());
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

            client.ClientConnected += (sender, e) =>
            {
                clientConnectedFired = true;
                Assert.NotEqual(-1, e.ConnectionId);
                connectionId = e.ConnectionId;
            };
            client.ClientDisconnected += (sender, e) =>
            {
                clientDisconnectedFired = true;
                Assert.NotEqual(-1, connectionId);
                Assert.Equal(connectionId, e.ConnectionId);
            };

            //Assert.False(client.RawClient.Connecting);
            //Assert.False(client.RawClient.Connected);

            client.Connect("localhost", (ushort)serverPort);

            //Assert.True(client.RawClient.Connecting);
            //Assert.False(client.RawClient.Connected);

            //server.SafeHandleMessages();
            client.SafeHandleMessages();
            //server.SafeHandleMessages();

            //Assert.False(client.RawClient.Connecting);
            //Assert.True(client.RawClient.Connected);

            client.Disconnect();

            server.SafeHandleMessages();

            //Assert.False(client.RawClient.Connecting);
            //Assert.False(client.RawClient.Connected);

            client.SafeHandleMessages();

            Assert.True(clientConnectedFired);
            Assert.True(clientDisconnectedFired);
        }
    }
}
