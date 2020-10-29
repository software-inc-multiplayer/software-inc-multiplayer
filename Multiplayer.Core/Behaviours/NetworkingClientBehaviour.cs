using System.Diagnostics.CodeAnalysis;
using UnityEngine;

using Multiplayer.Debugging;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Utility;

namespace Multiplayer.Core
{

    [DisallowMultipleComponent]
    public class NetworkingClientBehaviour : ModBehaviour
    {
        private Shared.ILogger logger;
        private GameClient client;

        public IUserManager UserManager { get; private set; }

        public override void OnActivate()
        {
            this.logger = new UnityLogger();
            this.logger.Debug("booting client behavior");

            if (!SteamManager.Initialized)
                return;

            var currentUserId = Steamworks.SteamUser.GetSteamID().m_SteamID;
            var currentUserName = Steamworks.SteamFriends.GetPersonaName();
            this.logger.Debug("Got steam info", currentUserId, currentUserName);

            var currentUser = new GameUser()
            {
                Id = currentUserId,
                Name = currentUserName,
                Role = UserRole.Guest
            };

            this.UserManager = new UserManager();
            
            this.client = new GameClient(
                this.logger,
                currentUser,
                new PacketSerializer(),
                this.UserManager
            );

            this.logger.Debug("client booted");
        }

        public override void OnDeactivate()
        {
            this.logger.Debug("destroying client");
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
        private void Update()
        {
            // this is the games update loop
            this.client.HandleMessages();
        }
    }
}
