using System;
using Multiplayer.Core.Behaviours;
using Multiplayer.Debugging;
using UnityEngine;
using ILogger = Multiplayer.Shared.ILogger;
//using Multiplayer.Extensions;

namespace Multiplayer.Core
{
    public class Meta : ModMeta, IDisposable
    {
        public static ILogger Logger { get; set; }
        public static ModController.DLLMod ThisMod { get; set; }
        
        public static bool GiveMeFreedom = true;

        public static NetworkingClientBehaviour NetworkingClient;
        public static NetworkingServerBehaviour NetworkingServer;
        public static SteamHelperBehaviour SteamHelper;
        public static MultiplayerMenuBehaviour MpWindow;

        public override string Name => "Software Inc Multiplayer";
        public override void ConstructOptionsScreen(RectTransform parent, bool inGame)
        {
            // Button bthost = WindowManager.SpawnButton();
            // bthost.onClick.AddListener(CreateBaseMultiplayerWindow);

            //bthost.SetText("Start");
            // WindowManager.AddElementToElement(bthost.gameObject, parent.gameObject, new Rect(15, 15, 192, 64), Rect.zero);
            var label = WindowManager.SpawnLabel();
            label.text = "Multiplayer Mod - Development";
            WindowManager.AddElementToElement(label.gameObject, parent.gameObject, new Rect(15, 15, 192, 32), Rect.zero);
        }

        public override void Initialize(ModController.DLLMod parentMod)
        {
            Logger = new UnityLogger();
            ThisMod = parentMod;
            
            Application.runInBackground = true;

            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                Logger.Error("Error!" + ((Exception)args.ExceptionObject).StackTrace + "\n\n" + ((Exception)args.ExceptionObject).Message);
            };
            
            base.Initialize(parentMod);
        }



        public void Dispose()
        {
            NetworkingClient?.Dispose();
            NetworkingServer?.Dispose();
            SteamHelper?.Dispose();
        }
    }
}
