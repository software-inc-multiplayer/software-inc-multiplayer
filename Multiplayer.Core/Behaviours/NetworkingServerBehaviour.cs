using System.Diagnostics.CodeAnalysis;
using UnityEngine;

using Multiplayer.Debugging;
using Multiplayer.Networking.Server;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Utility;
using Multiplayer.Networking.Server.Managers;
using Multiplayer.Networking;

namespace Multiplayer.Core
{

    [DisallowMultipleComponent]
    public class NetworkingServerBehaviour : ModBehaviour
    {
        private Shared.ILogger logger;
        public GameServer Server { get; private set; }

        public IUserManager UserManager { get; private set; }
        public BanManager BanManager { get; private set; }

        public override void OnActivate()
        {
            this.logger = new UnityLogger();
            this.logger.Debug("server behaviour booting");

            this.UserManager = new UserManager();
            this.BanManager = new BanManager();

            this.Server = new GameServer(
                this.logger,
                new PacketSerializer(),
                this.UserManager,
                this.BanManager
            );
            this.logger.Debug("server behaviour booted");
        }

        public override void OnDeactivate()
        {
            this.logger.Debug("destroying server behaviour");
        }

        public void Host(string name, string description, int port)
        {
            var serverInfo = new ServerInfo()
            {
                Name = name,
                Description = description,
                Port = port,
                DefaultRole = UserRole.Guest
            };
            this.logger.Debug("server starting");
            this.Server.Start(serverInfo);
            this.logger.Debug("server started");
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
        private void Update()
        {
            // this is the games update loop
            this.Server.HandleMessages();
        }
    }
}
