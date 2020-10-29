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
        Utils.Controls.Window.UITextbox hostport;
        Utils.Controls.Window.UIButton hostbutton;
        Utils.Controls.Window.UITextbox connectremoteip;
        Utils.Controls.Window.UITextbox connectremoteport;
        Utils.Controls.Window.UIButton connectbutton;
        Utils.Controls.Window.UILabel serverlistplaceholder;
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

            Utils.Controls.Window.UIButton tabHost = new Utils.Controls.Window.UIButton("CreateServerTab".LocDef("Create Server"), new Rect(0,0,145,25),CreateServerClicked, MPWindow,"mp_tab_create");
            Utils.Controls.Window.UIButton tabConnect = new Utils.Controls.Window.UIButton("ConnectServerTab".LocDef("Connect to Server"),new Rect(150,0,145,25),ConnectServerClicked,MPWindow,"mp_tab_connect");
            Utils.Controls.Window.UIButton tabServerlist = new Utils.Controls.Window.UIButton("ServerListTab".LocDef("Server List"), new Rect(300, 0, 145, 25), ServerlistClicked, MPWindow, "mp_tab_serverlist");

            //Create the controls inside the tabs
            hostport = new Utils.Controls.Window.UITextbox(new Rect(25, 50, 150, 30), MPWindow, "Port", "mp_hostport");
            hostbutton = new Utils.Controls.Window.UIButton("Host Server", new Rect(25, 90, 150, 50), null, MPWindow, "mp_hostbutton");
            connectremoteip = new Utils.Controls.Window.UITextbox(new Rect(25, 50, 150, 30), MPWindow, "Remote IP", "mp_remoteip");
            connectremoteport = new Utils.Controls.Window.UITextbox(new Rect(25, 90, 150, 30), MPWindow, "Remote Port", "mp_remoteport");
            connectbutton = new Utils.Controls.Window.UIButton("Connect", new Rect(25, 130, 150, 50), null, MPWindow, "mp_remotebutton");
            serverlistplaceholder = new Utils.Controls.Window.UILabel("Coming soon!", new Rect(25, 50, 150, 25), MPWindow, "mp_serverlistplaceholder",true);
            hostport.obj.gameObject.SetActive(false);
            connectremoteip.obj.gameObject.SetActive(false);
            connectremoteport.obj.gameObject.SetActive(false);
            connectbutton.obj.gameObject.SetActive(false);
            serverlistplaceholder.obj.gameObject.SetActive(false);

            MPWindow.Show();
            ConnectServerClicked();
        }

		private void ServerlistClicked()
		{
            connectremoteip.obj.gameObject.SetActive(false);
            connectremoteport.obj.gameObject.SetActive(false);
            connectbutton.obj.gameObject.SetActive(false);
            hostport.obj.gameObject.SetActive(false);
            hostbutton.obj.gameObject.SetActive(false);
            serverlistplaceholder.obj.gameObject.SetActive(true);
        }

		private void ConnectServerClicked()
        {
            connectremoteip.obj.gameObject.SetActive(true);
            connectremoteport.obj.gameObject.SetActive(true);
            connectbutton.obj.gameObject.SetActive(true);
            hostport.obj.gameObject.SetActive(false);
            hostbutton.obj.gameObject.SetActive(false);
            serverlistplaceholder.obj.gameObject.SetActive(false);
        }

		private void CreateServerClicked()
        {
            connectremoteip.obj.gameObject.SetActive(false);
            connectremoteport.obj.gameObject.SetActive(false);
            connectbutton.obj.gameObject.SetActive(false);
            hostport.obj.gameObject.SetActive(true);
            hostbutton.obj.gameObject.SetActive(true);
            serverlistplaceholder.obj.gameObject.SetActive(false);
        }

		public override void Initialize(ModController.DLLMod parentMod)
        {
            ThisMod = parentMod;
            Application.runInBackground = true;
            base.Initialize(parentMod);
        }
    }
}
