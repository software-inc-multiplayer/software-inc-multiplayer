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

        public NetworkingClientBehaviour NetworkingClient { get; private set; }
        public NetworkingServerBehaviour NetworkingServer { get; private set; }
        public SteamHelperBehaviour SteamHelper { get; private set; }
        public GUIWindow MpWindow { get; set; }

        #region Multiplayer Window Controls
        Utils.Controls.Window.UITextbox hostport;
        Utils.Controls.Window.UITextbox hostpassword;
        Utils.Controls.Window.UIButton hostbutton;
        Utils.Controls.Window.UITextbox connectremoteip;
        Utils.Controls.Window.UITextbox connectremoteport;
        Utils.Controls.Window.UITextbox connectremotepassword;
        Utils.Controls.Window.UIButton connectbutton;
        Utils.Controls.Window.UIButton disconnectButton;
        Utils.Controls.Window.UILabel serverlistplaceholder;
        #endregion



        public override string Name => "Software Inc Multiplayer";
        public override void ConstructOptionsScreen(RectTransform parent, bool inGame)
        {
            Button bthost = WindowManager.SpawnButton();
            bthost.onClick.AddListener(CreateBaseMultiplayerWindow);

            //bthost.SetText("Start");
            WindowManager.AddElementToElement(bthost.gameObject, parent.gameObject, new Rect(15, 15, 192, 64), Rect.zero);
        }

        private void CreateBaseMultiplayerWindow()
        {
            Logger.Info("Opened multiplayer window.");
            if (MpWindow != null)
            {
                MpWindow.Show();
                return;
            }
            MpWindow = WindowManager.SpawnWindow();
            MpWindow.Modal = true;
            //MPWindow.SetTitle("MultiplayerButton".LocDef("Multiplayer"));
            MpWindow.InitialTitle = "MultiplayerButton".LocDef("Multiplayer");
            MpWindow.ShowCentered = true;
            MpWindow.MinSize = new Vector2(730, 500);
            MpWindow.SizeButton.SetActive(false);

            Utils.Controls.Window.UIButton tabHost = new Utils.Controls.Window.UIButton("CreateServerTab".LocDef("Create Server"), new Rect(3, 3, 240, 25),CreateServerClicked, MPWindow,"mp_tab_create");
            Utils.Controls.Window.UIButton tabConnect = new Utils.Controls.Window.UIButton("ConnectServerTab".LocDef("Connect to Server"),new Rect(245, 3, 240, 25),ConnectServerClicked,MPWindow,"mp_tab_connect");
            Utils.Controls.Window.UIButton tabServerlist = new Utils.Controls.Window.UIButton("ServerListTab".LocDef("Server List"), new Rect(487, 3, 240, 25), ServerlistClicked, MPWindow, "mp_tab_serverlist");

            //Create the controls inside the tabs
            hostport = new Utils.Controls.Window.UITextbox(new Rect(25, 50, 150, 30), MPWindow, "Port", "mp_hostport");
            hostpassword = new Utils.Controls.Window.UITextbox(new Rect(25, 90, 150, 30), MPWindow, "Password", "mp_hostpassword");
            hostbutton = new Utils.Controls.Window.UIButton("Host Server", new Rect(25, 130, 150, 50), HostClicked, MPWindow, "mp_hostbutton");
            connectremoteip = new Utils.Controls.Window.UITextbox(new Rect(25, 50, 150, 30), MPWindow, "Remote IP", "mp_remoteip");
            connectremoteport = new Utils.Controls.Window.UITextbox(new Rect(25, 90, 150, 30), MPWindow, "Remote Port", "mp_remoteport");
            connectremotepassword = new Utils.Controls.Window.UITextbox(new Rect(25, 130, 150, 30), MPWindow, "Password", "mp_remotepassword");
            connectbutton = new Utils.Controls.Window.UIButton("Connect", new Rect(25, 170, 150, 50), ConnectClicked, MPWindow, "mp_remotebutton");
            serverlistplaceholder = new Utils.Controls.Window.UILabel("Coming soon!", new Rect(25, 50, 150, 25), MPWindow, "mp_serverlistplaceholder",true);
            Utils.Controls.Window.UIButton tabHost = new Utils.Controls.Window.UIButton("CreateServerTab".LocDef("Create Server"), new Rect(0, 0, 145, 25), CreateServerClicked, MpWindow, "mp_tab_create");
            Utils.Controls.Window.UIButton tabConnect = new Utils.Controls.Window.UIButton("ConnectServerTab".LocDef("Connect to Server"), new Rect(150, 0, 145, 25), ConnectServerClicked, MpWindow, "mp_tab_connect");
            Utils.Controls.Window.UIButton tabServerList = new Utils.Controls.Window.UIButton("ServerListTab".LocDef("Server List"), new Rect(300, 0, 145, 25), ServerlistClicked, MpWindow, "mp_tab_serverlist");

            //Create the controls inside the tabs
            hostport = new Utils.Controls.Window.UITextbox(new Rect(25, 50, 150, 30), MpWindow, "Port", "mp_hostport")
            {
                obj = { text = "1234", characterValidation = InputField.CharacterValidation.Integer }
            };
            hostbutton = new Utils.Controls.Window.UIButton("Host Server", new Rect(25, 90, 150, 50), HostGameClicked, MpWindow, "mp_hostbutton");
            connectremoteip =
                new Utils.Controls.Window.UITextbox(new Rect(25, 50, 150, 30), MpWindow, "Remote IP", "mp_remoteip")
                {
                    obj = { text = "127.0.0.1" }
                };
            connectremoteport =
                new Utils.Controls.Window.UITextbox(new Rect(25, 90, 150, 30), MpWindow, "Remote Port", "mp_remoteport")
                {
                    obj = { text = "1234", characterValidation = InputField.CharacterValidation.Integer }
                };
            connectbutton = new Utils.Controls.Window.UIButton("Connect", new Rect(25, 130, 150, 50), ConnectGameClicked, MpWindow, "mp_remotebutton");
            disconnectButton = new Utils.Controls.Window.UIButton("Disconnect", new Rect(25, 190, 150, 50),
                DisconnectGameClicked, MpWindow, "mp_disconnect_from_server");
            serverlistplaceholder = new Utils.Controls.Window.UILabel("Coming soon!", new Rect(25, 50, 150, 25), MpWindow, "mp_serverlistplaceholder", true);
            
            hostport.obj.gameObject.SetActive(false);
            hostpassword.obj.gameObject.SetActive(false);
            hostbutton.obj.gameObject.SetActive(false);
            connectremoteip.obj.gameObject.SetActive(false);
            connectremoteport.obj.gameObject.SetActive(false);
            connectremotepassword.obj.gameObject.SetActive(false);
            connectbutton.obj.gameObject.SetActive(false);
            serverlistplaceholder.obj.gameObject.SetActive(false);

            MpWindow.Show();
            ConnectServerClicked();
        }


        private void ServerlistClicked()
        {
            connectremoteip.obj.gameObject.SetActive(false);
            connectremoteport.obj.gameObject.SetActive(false);
            connectremotepassword.obj.gameObject.SetActive(false);
            connectbutton.obj.gameObject.SetActive(false);
            hostport.obj.gameObject.SetActive(false);
            hostpassword.obj.gameObject.SetActive(false);
            hostbutton.obj.gameObject.SetActive(false);
            serverlistplaceholder.obj.gameObject.SetActive(true);
        }

        private void ConnectServerClicked()
        {
            connectremoteip.obj.gameObject.SetActive(true);
            connectremoteport.obj.gameObject.SetActive(true);
            connectremotepassword.obj.gameObject.SetActive(true);
            connectbutton.obj.gameObject.SetActive(true);
            hostport.obj.gameObject.SetActive(false);
            hostpassword.obj.gameObject.SetActive(false);
            hostbutton.obj.gameObject.SetActive(false);
            serverlistplaceholder.obj.gameObject.SetActive(false);
        }

        private void CreateServerClicked()
        {
            connectremoteip.obj.gameObject.SetActive(false);
            connectremoteport.obj.gameObject.SetActive(false);
            connectremotepassword.obj.gameObject.SetActive(false);
            connectbutton.obj.gameObject.SetActive(false);
            hostport.obj.gameObject.SetActive(true);
            hostpassword.obj.gameObject.SetActive(true);
            hostbutton.obj.gameObject.SetActive(true);
            serverlistplaceholder.obj.gameObject.SetActive(false);
        }

        private void HostGameClicked()
        {
            var strPort = this.hostport.obj.text;
            ushort port = 1337;
            if (!ushort.TryParse(strPort, out port))
            {
                // show some error to the user?
            }
            this.NetworkingServer.Host("<placeholder>", "<placeholder>", port);
        }


        private void DisconnectGameClicked()
        {
            if (NetworkingClient.IsConnected)
            {
                NetworkingClient.Disconnect();
            }
        }


        private void ConnectGameClicked()
        {
            var strPort = this.connectremoteport.obj.text;
            var port = 1337;
            if (!int.TryParse(strPort, out port))
            {
                // show some error to the user?
            }
            this.NetworkingClient.Connect(this.connectremoteip.obj.text, port);
        }

        public override void Initialize(ModController.DLLMod parentMod)
        {
            Logger = new FileLogger();
            ThisMod = parentMod;
            Application.runInBackground = true;
            this.NetworkingClient = parentMod.Behaviors.Single(x => x is NetworkingClientBehaviour) as NetworkingClientBehaviour;
            this.NetworkingServer = parentMod.Behaviors.Single(x => x is NetworkingServerBehaviour) as NetworkingServerBehaviour;
            this.SteamHelper = parentMod.Behaviors.Single(x => x is SteamHelperBehaviour) as SteamHelperBehaviour;
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
