using System.Diagnostics.CodeAnalysis;
using UnityEngine;

using Multiplayer.Debugging;
using Multiplayer.Networking.Server;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Utility;
using Multiplayer.Networking.Server.Managers;

namespace Multiplayer.Core
{

    [DisallowMultipleComponent]
    public class NetworkingServerBehaviour : ModBehaviour
    {
        private Shared.ILogger logger;
        private GameServer server;

        public IUserManager UserManager { get; private set; }
        public BanManager BanManager { get; private set; }

        public override void OnActivate()
        {
            this.UserManager = new UserManager();
            this.BanManager = new BanManager();

            this.logger = new UnityLogger();
            this.server = new GameServer(
                this.logger,
                new PacketSerializer(),
                this.UserManager,
                this.BanManager
            );
        }

        public override void OnDeactivate()
        {
            
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
        private void Update()
        {
            // this is the games update loop
            this.server.HandleMessages();
        }
    }
}
