using Multiplayer.Debugging;
using Multiplayer.Extensions;
using Multiplayer.Networking;
using System;
using System.Collections.Generic;
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
        public GUIWindow MPWindow { get; set; }
        public override string Name => "Software Inc Multiplayer";
        public override void ConstructOptionsScreen(RectTransform parent, bool inGame)
        {
            Button bthost = WindowManager.SpawnButton();
            bthost.onClick.AddListener(CreateBaseMultiplayerWindow);
            bthost.SetText("Start");
            WindowManager.AddElementToElement(bthost.gameObject, parent.gameObject, new Rect(15, 15, 192, 64), Rect.zero);
        }

        private void CreateBaseMultiplayerWindow()
        {
            Logging.Info("Opened multiplayer window.");
            if (MPWindow != null)
            {
                MPWindow.Show();
                return;
            }
            MPWindow = WindowManager.SpawnWindow();
            MPWindow.SetTitle("MultiplayerButton".LocDef("Multiplayer"));
            MPWindow.ShowCentered = true;
            MPWindow.MinSize = new Vector2(730, 500);
            MPWindow.SizeButton.SetActive(false);


            MPWindow.Show();
        }

        public override void Initialize(ModController.DLLMod parentMod)
        {
            ThisMod = parentMod;
            Application.runInBackground = true;
            base.Initialize(parentMod);
        }
    }
}
