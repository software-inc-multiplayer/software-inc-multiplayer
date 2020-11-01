using System.Diagnostics.CodeAnalysis;
using UnityEngine;

using Multiplayer.Debugging;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Utility;
using Multiplayer.Networking.Client.Handlers;

namespace Multiplayer.Core
{

    [DisallowMultipleComponent]
    public class NetworkingClientBehaviour : ModBehaviour
    {
        private Shared.ILogger logger;
        public GameClient Client { get; private set; }
        public ChatHandler ChatHandler { get; private set; }
        public IUserManager UserManager { get; private set; }

        public override void OnActivate()
        {
            this.logger = new UnityLogger();
            this.logger.Debug("client behavior booting");

            if (!SteamManager.Initialized)
                return;

            var currentUserId = Steamworks.SteamUser.GetSteamID().m_SteamID;
            var currentUserName = Steamworks.SteamFriends.GetPersonaName();
            this.logger.Debug("got steam info", currentUserId, currentUserName);

            var currentUser = new GameUser()
            {
                Id = currentUserId,
                Name = currentUserName,
                Role = UserRole.Guest
            };

            this.UserManager = new UserManager();
            
            this.Client = new GameClient(
                this.logger,
                currentUser,
                new PacketSerializer(),
                this.UserManager
            );

            this.RegisterPacketHandler();

            this.logger.Debug("client behaviour booted");
        }

        private void RegisterPacketHandler()
        {
            this.ChatHandler = new ChatHandler(this.Client);
            this.Client.RegisterPacketHandler(this.ChatHandler);
        }

        public override void OnDeactivate()
        {
            this.logger.Debug("destroying client behaviour");
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
        private void Update()
        {
            // this is the games update loop
            this.Client.HandleMessages();
        }

        public void Connect(string host, int port)
        {
            this.logger.Debug("client connecting");
            this.Client.Connect(host, port);
            this.logger.Debug("client connected");
        }
    }
}
