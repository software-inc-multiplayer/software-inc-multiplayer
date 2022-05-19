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
        private ILogger Logger;
        public static bool Initialized { get; private set; } = false;
        
        private const int Appid = 362620;

        public void OnDisable()
        {
            //Proper place to shutdown SteamClient in order to avoid hanging of the game when closing it
            OnDeactivate();
        }

        public void OnEnable()
        {
            Logger = new UnityLogger();
            OnActivate();
        }

        public override void OnActivate()
        {
            if (Initialized)
                return;
            
            this.Logger.Info("[STEAM] booting");
            try
            {
                SteamClient.Init(Appid);
                Initialized = true;

                this.Logger.Info("[STEAM] booted");

                this.Logger.Debug($"[STEAM] Name: {SteamClient.Name}");
                this.Logger.Debug($"[STEAM] AccountId: {SteamClient.SteamId.AccountId}");
                this.Logger.Debug($"[STEAM] IsValid: {SteamClient.IsValid}");
                this.Logger.Debug($"[STEAM] IsLoggedOn: {SteamClient.IsLoggedOn}");
            }
            catch (Exception ex)
            {
                this.Logger.Error("[STEAM] boot failed", ex);
            }
        }

        public override void OnDeactivate()
        {
            if (!Initialized)
                return;

            this.Logger.Info("[STEAM] shutdown");
            try
            {
                SteamClient.Shutdown();
                Initialized = false;

                this.Logger.Info("[STEAM] closed");
            }
            catch (Exception ex)
            {
                this.Logger.Error("[STEAM] shutdown failed", ex);
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
