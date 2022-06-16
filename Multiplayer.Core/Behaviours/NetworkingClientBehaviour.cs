using System;
using System.Diagnostics.CodeAnalysis;
using Multiplayer.Debugging;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Client.Handlers;
using Multiplayer.Networking.Shared;
using UnityEngine;

namespace Multiplayer.Core.Behaviours
{

    [DisallowMultipleComponent]
    public class NetworkingClientBehaviour : ModBehaviour, IDisposable

    {
        private Shared.ILogger log;

        //public GameClient_old Client { get; private set; }
        public GameClient Client { get; private set; }
        public ChatHandler ChatHandler { get; private set; }
        public IUserManager UserManager { get; private set; }
        public bool IsConnected => Client.Socket.Connected;


        public override void OnActivate()
        {
            this.log = new UnityLogger();
            this.log.Debug("client behavior booting");

            //if (!SteamManager.Initialized)
            //    return;

            //var currentUserId = Steamworks.SteamUser.GetSteamID().m_SteamID;
            //var currentUserName = Steamworks.SteamFriends.GetPersonaName();
            //this.log.Debug("got steam info", currentUserId, currentUserName);

            /*var currentUser = new GameUser()
            {
                Id = currentUserId,
                Name = currentUserName,
                Role = UserRole.Guest
            };

            this.UserManager = new UserManager();

            this.Client = new GameClient_old(
                this.log,
                currentUser,
                new PacketSerializer(),
                this.UserManager
            );

            this.RegisterPacketHandler();*/

            this.Client = new GameClient(log, null);
            this.log.Debug("client behaviour booted");
        }

        private void RegisterPacketHandler()
        {
            //this.ChatHandler = new ChatHandler(this.Client);
            //this.Client.RegisterPacketHandler(this.ChatHandler);
        }

        public override void OnDeactivate()
        {
            this.log.Debug("destroying client behaviour");
            Dispose();
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
        private void Update()
        {

            // this is the games update loop
            //this.Client.HandleMessages();
        }

        public void Connect(string host, int port)
        {
            this.log.Debug($"[client] connecting to {host}:{port}");
            try
            {

                this.Client.Connect(host, (ushort)port);
            }
            catch (Exception ex)
            {
                this.log.Error("[client] not connected", ex);
            }

            this.log.Debug("[client] connected");
        }

        public void Disconnect()
        {
            log.Debug("[client] disconnecting..");

            try
            {
                Client.Disconnect();
            }
            catch (Exception e)
            {
                this.log.Error("[client] not connected", e);
            }
        }

        public void Dispose()
        {
            try
            {
                Client?.Dispose();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }


    }
}
