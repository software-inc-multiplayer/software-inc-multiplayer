﻿using System;
using System.Diagnostics.CodeAnalysis;
using Multiplayer.Debugging;
using Multiplayer.Networking;
using Multiplayer.Networking.Server;
using Multiplayer.Networking.Server.Handlers;
using Multiplayer.Networking.Server.Managers;
using Multiplayer.Networking.Shared;
using UnityEngine;

namespace Multiplayer.Core.Behaviours
{

    [DisallowMultipleComponent]
    public class NetworkingServerBehaviour : ModBehaviour, IDisposable
    {
        private Shared.ILogger logger;
        //public GameServer_old Server { get; private set; }
        public GameServer Server { get; private set; }

        public IUserManager UserManager { get; private set; }
        public BanManager BanManager { get; private set; }
        public ChatHandler ChatHandler { get; private set; }

        public override void OnActivate()
        {
            this.logger = new UnityLogger();
            this.logger.Debug("server behaviour booting");

            this.UserManager = new UserManager();
            this.BanManager = new BanManager();
            
            this.Server = new GameServer(logger);

            /*this.Server = new GameServer_old(
                this.logger,
                new PacketSerializer(),
                this.UserManager,
                this.BanManager
            );

            this.RegisterHandlers();*/

            this.logger.Debug("server behaviour booted");
        }

        private void RegisterHandlers()
        {
            //this.ChatHandler = new ChatHandler(this.Server);
            //this.Server.RegisterPacketHandler(this.ChatHandler);
        }

        public override void OnDeactivate()
        {
            this.logger.Debug("destroying server behaviour");
            Dispose();
        }

        public void Host(string name, string description, ushort port)
        {
            var serverInfo = new ServerInfo()
            {
                Name = name,
                Description = description,
                Port = port,
                DefaultRole = UserRole.Guest
            };
            this.logger.Debug("[server] starting");
            try
            {
                this.Server.Start(serverInfo);
            }
            catch (Exception ex)
            {
                this.logger.Error("[server] not started", ex);
            }
            this.logger.Debug("[server] started");
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
        private void Update()
        {
             // this is the games update loop
            // this.Server.HandleMessages();
        }

        public void Dispose()
        {
            Server?.Dispose();
            Server = null;
        }
    }
}
