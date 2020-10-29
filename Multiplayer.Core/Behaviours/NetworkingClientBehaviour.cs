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
            if (!SteamManager.Initialized)
                return;

            var currentUserId = Steamworks.SteamUser.GetSteamID().m_SteamID;
            var currentUserName = Steamworks.SteamFriends.GetPersonaName();

            var currentUser = new GameUser()
            {
                Id = currentUserId,
                Name = currentUserName,
                Role = UserRole.Guest
            };

            this.UserManager = new UserManager();

            this.logger = new UnityLogger();
            this.client = new GameClient(
                this.logger,
                currentUser,
                new PacketSerializer(),
                this.UserManager
            );
        }

        public override void OnDeactivate()
        {
            
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
        private void Update()
        {
            // this is the games update loop
            this.client.HandleMessages();
        }
    }
}
