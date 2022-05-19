using System;
using System.Linq;
using System.Reflection;
using Multiplayer.Core.Behaviours;
using Multiplayer.Debugging;
//using Multiplayer.Extensions;
using UnityEngine;
using UnityEngine.UI;
using ILogger = Multiplayer.Shared.ILogger;

namespace Multiplayer.Core
{
    public class Meta : ModMeta, IDisposable
    {
        public static ILogger Logger { get; set; }
        public static ModController.DLLMod ThisMod { get; set; }
        
        public static bool GiveMeFreedom = true;
        
        public static NetworkingClientBehaviour NetworkingClient { get; private set; }
        public static NetworkingServerBehaviour NetworkingServer { get; private set; }
        public static SteamHelperBehaviour SteamHelper { get; private set; }
        public static MultiplayerMenuBehaviour MpWindow { get; set; }

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
            
            NetworkingClient = parentMod.Behaviors.Single(x => x is NetworkingClientBehaviour) as NetworkingClientBehaviour;
            NetworkingServer = parentMod.Behaviors.Single(x => x is NetworkingServerBehaviour) as NetworkingServerBehaviour;
            SteamHelper = parentMod.Behaviors.Single(x => x is SteamHelperBehaviour) as SteamHelperBehaviour;
            
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
