using System;
using System.Diagnostics;
using System.Threading;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Server;
using Multiplayer.Networking.Shared;
using Multiplayer.Packets;
using Xunit;

namespace Multiplayer.Networking.Test
{
    public class ServerHandlerTest : IDisposable
    {
        private static int _serverPort = 1500;
        private readonly int serverPort;
        private readonly ServerInfo serverInfo;

        private readonly TestLogger logger;
        private GameServer server;

        private static readonly ulong userId1 = 0123456789;
        private static readonly ulong userId2 = 9876543210;

        private GameClient client1;
        private GameClient client2;
        
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
            this.server = new GameServer(logger);
            this.client1 = new GameClient(logger, testUser1);
            this.client2 = new GameClient(logger, testUser2);
        }

        [DebuggerStepThrough]
        private void SetupServerAndClient()
        {
            this.server = new GameServer(logger);
            this.server.Start(this.serverInfo);

            this.client1.Connect("localhost", (ushort)serverPort);
            this.client2.Connect("localhost", (ushort)serverPort);
        }

        public void Dispose()
        {
            this.server.Dispose();
        }

        [Fact]
        public void ServerWithChatHandler()
        {
            SetupServerAndClient();

            const string testMessage = "Hello World!";
            this.client1.Send(new ChatMessage()
            {
                Username = "Client 1",
                Contents = testMessage
            });
        }
    }
}
