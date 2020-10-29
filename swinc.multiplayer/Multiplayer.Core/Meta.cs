using Multiplayer.Debugging;
using Multiplayer.Extensions;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class Meta : ModMeta
    {
        public static ModController.DLLMod ThisMod { get; set; }
        public static bool GiveMeFreedom = true;
        public override string Name => "Software Inc Multiplayer";
        public override void ConstructOptionsScreen(RectTransform parent, bool inGame)
        {
            Button bthost = WindowManager.SpawnButton();
            bthost.onClick.AddListener(ShowMultiplayerHost);
            bthost.SetText("Host Server");
            WindowManager.AddElementToElement(bthost.gameObject, parent.gameObject, new Rect(15, 15, 192, 64), Rect.zero);
            
            Button btconnect = WindowManager.SpawnButton();
            btconnect.onClick.AddListener(ShowMultiplayerConnect);
            btconnect.SetText("Connect to Server");
            WindowManager.AddElementToElement(btconnect.gameObject, parent.gameObject, new Rect(15, 100, 192, 64), Rect.zero);



        }

		private void ShowMultiplayerConnect()
		{

		}

		private void ShowMultiplayerHost()
		{

		}

        public override void Initialize(ModController.DLLMod parentMod)
        {
            ThisMod = parentMod;
            Application.runInBackground = true;
            base.Initialize(parentMod);
        }
    }
}
