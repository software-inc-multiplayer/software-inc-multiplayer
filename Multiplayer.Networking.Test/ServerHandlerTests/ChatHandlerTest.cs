using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Client.Handlers;
using Multiplayer.Networking.Server;
using Multiplayer.Networking.Server.Handlers;
using Multiplayer.Networking.Server.Managers;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Utility;
using Packets;
using Xunit;

namespace Multiplayer.Networking.Test.ServerHandlerTests
{
    public class ServerHandlerTest : IDisposable
    {
        private static int _serverPort = 1500;
        private readonly int serverPort;
        private readonly ServerInfo serverInfo;

        private readonly TestLogger logger;
        private readonly PacketSerializer packetSerializer;
        private readonly GameServer_old server;

        private static readonly ulong userId1 = 0123456789;
        private static readonly ulong userId2 = 9876543210;

        private readonly GameClient_old client1;
        private readonly GameClient_old client2;
        private readonly GameUser testUser1 = new GameUser() {
            Id = userId1,
            Name = "test-user-1",
            Role = UserRole.Host
        };
        private readonly GameUser testUser2 = new GameUser()
        {
            Id = userId2,
            Name = "test-user-2",
            Role = UserRole.Player
        };

        public ServerHandlerTest()
        {
            this.serverPort = Interlocked.Increment(ref _serverPort);
            this.serverInfo = new ServerInfo() { Port = (ushort)this.serverPort, Name = "testserver", DefaultRole = UserRole.Host };
            this.logger = new TestLogger();
            this.packetSerializer = new PacketSerializer();
            this.server = new GameServer_old(this.logger, this.packetSerializer, new UserManager(), new BanManager());

            this.client1 = new GameClient_old(this.logger, this.testUser1, this.packetSerializer, new UserManager());
            this.client2 = new GameClient_old(this.logger, this.testUser2, this.packetSerializer, new UserManager());
        }

        [DebuggerStepThrough]
        private void SetupServerAndClient(IPacketHandler[] packetHandlers)
        {
            foreach (var packetHandler in packetHandlers)
            {
                this.server.RegisterPacketHandler(packetHandler);
            }
            this.server.Start(this.serverInfo);

            this.client1.Connect("localhost", (ushort)serverPort);
            this.client2.Connect("localhost", (ushort)serverPort);

            this.server.SafeHandleMessages(); // finish server connected
            this.client1.SafeHandleMessages(); // trigger handshake
            this.client2.SafeHandleMessages(); // trigger handshake
            this.server.SafeHandleMessages(); // handle client handshake
        }

        public void Dispose()
        {
            this.server.Dispose();
        }

        [Fact]
        public void ServerWithChatHandler()
        {
            // TODO moq this server and check if chat handler does its stuff
            var chatHandler = new Server.Handlers.ChatHandler(this.server);
            SetupServerAndClient(new IPacketHandler[] { chatHandler });

            var testMessage = "test-message";
            this.client1.Send(new ChatMessage(this.testUser1.Id, testMessage));

            this.server.SafeHandleMessages();
        }
    }
}
