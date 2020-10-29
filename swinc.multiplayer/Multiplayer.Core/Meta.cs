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

        #region Multiplayer Window Controls
        Utils.Controls.Window.UILabel hosttestlabel;
        Utils.Controls.Window.UILabel connecttestlabel;
		#endregion

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
            MPWindow.Modal = true;
            MPWindow.SetTitle("MultiplayerButton".LocDef("Multiplayer"));
            MPWindow.ShowCentered = true;
            MPWindow.MinSize = new Vector2(730, 500);
            MPWindow.SizeButton.SetActive(false);

            Utils.Controls.Window.UIButton tabHost = new Utils.Controls.Window.UIButton("CreateServerTab".LocDef("Create Server"), new Rect(0,0,150,25),CreateServerClicked, MPWindow,"tab_create");
            Utils.Controls.Window.UIButton tabConnect = new Utils.Controls.Window.UIButton("ConnectServerTab".LocDef("Connect to Server"),new Rect(150,0,150,25),ConnectServerClicked,MPWindow,"tab_connect");

            //Create the controls inside the tabs
            connecttestlabel = new Utils.Controls.Window.UILabel("Connect Test", new Rect(25, 50, 100, 50), MPWindow);
            hosttestlabel = new Utils.Controls.Window.UILabel("Host Test", new Rect(25, 50, 100, 50), MPWindow);
            connecttestlabel.obj.gameObject.SetActive(false);
            hosttestlabel.obj.gameObject.SetActive(false);
            MPWindow.Show();
            ConnectServerClicked();
        }

		private void ConnectServerClicked()
        {
            connecttestlabel.obj.gameObject.SetActive(true);
            hosttestlabel.obj.gameObject.SetActive(false);
        }

		private void CreateServerClicked()
		{
            connecttestlabel.obj.gameObject.SetActive(false);
            hosttestlabel.obj.gameObject.SetActive(true);
        }

		public override void Initialize(ModController.DLLMod parentMod)
        {
            ThisMod = parentMod;
            Application.runInBackground = true;
            base.Initialize(parentMod);
        }
    }
}
