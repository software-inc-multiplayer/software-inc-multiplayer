using System;
using System.Diagnostics.CodeAnalysis;
using Multiplayer.Debugging;
using UnityEngine;
using Facepunch.Steamworks;
using ILogger = Multiplayer.Shared.ILogger;

namespace Multiplayer.Core.Behaviours
{
    [DisallowMultipleComponent]
    public class SteamHelperBehaviour : ModBehaviour, IDisposable
    {
        private ILogger logger;
        public static bool Initialized { get; private set; } = false;
        private const int Appid = 362620;

        public void OnDisable()
        {
            //Proper place to shutdown SteamClient in order to avoid hanging of the game when closing it
            OnDeactivate();
        }

        public void OnEnable()
        {
            OnActivate();
        }



        public override void OnActivate()
        {
            if (Initialized)
                return;

            this.logger = Meta.Logger;
            this.logger.Info("[STEAM] booting");
            try
            {
                SteamClient.Init(Appid);
                Initialized = true;

                this.logger.Info("[STEAM] booted");

                this.logger.Debug($"[STEAM] Name: {SteamClient.Name}");
                this.logger.Debug($"[STEAM] AccountId: {SteamClient.SteamId.AccountId}");
                this.logger.Debug($"[STEAM] IsValid: {SteamClient.IsValid}");
                this.logger.Debug($"[STEAM] IsLoggedOn: {SteamClient.IsLoggedOn}");
            }
            catch (Exception ex)
            {
                this.logger.Error("[STEAM] boot failed", ex);
            }
        }

        public override void OnDeactivate()
        {
            if (!Initialized)
                return;

            this.logger.Info("[STEAM] shutdown");
            try
            {
                SteamClient.Shutdown();
                Initialized = false;

                this.logger.Info("[STEAM] closed");
            }
            catch (Exception ex)
            {
                this.logger.Error("[STEAM] shutdown failed", ex);
            }
        }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Unity")]
        private void Update()
        {
            if (!Initialized)
                return;
            SteamClient.RunCallbacks();
        }

        public void Dispose() => OnDeactivate();
    }
}
