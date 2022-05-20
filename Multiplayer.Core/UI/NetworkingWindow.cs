#if REWRITE_UI
// This is not compiled by default.
using System;
using System.Collections.Generic;
using Multiplayer.Core.Utils.Controls.Element;
using Multiplayer.Extensions;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// ReSharper disable ObjectCreationAsStatement

namespace Multiplayer.Core.UI
{
    public class NetworkingWindow : ModBehaviour
    {
        public int SelectedTab { get; set; }
        
        public static GUIWindow InternalWindow { get; set; }
        
        private RectTransform ConnectionTab { get; set; }

        public void ChangeTab(int tab)
        {
            SelectedTab = tab;
            switch (tab)
            {
                case 0:
                    ConnectionTab.gameObject.FlipActive();
                    break;
            }
        }

        public void ConstructWindow()
        {
            InternalWindow = WindowManager.SpawnWindow();
            InternalWindow.StartHidden = true;
            InternalWindow.MinSize = new Vector2(750, 235);

            ConnectionTab = WindowManager.SpawnPanel();

            new UIButton("ConnectButtonText".LocDef("Connect"), new Rect(30, 30, 700, 45), (() => ChangeTab(0)), ConnectionTab.gameObject);

            #region Connection Tab

            new UILabel(
                    "ConnectLabel".LocDef("Connect to a multiplayer server via IP and Port, specify a password if the server has a password set."),
                    new Rect(60, 30, 700, 45),
                    ConnectionTab.gameObject);

                UITextbox ipTextBox = new UITextbox(new Rect(35, 110, 159, 25), ConnectionTab.gameObject, "IP", "", null, 12);
                UITextbox portTextBox = new UITextbox(new Rect(40 + 159, 110, 159, 25), ConnectionTab.gameObject, "PortInput".LocDef("Port"), "", null, 12);
                UITextbox passwordTextBox = new UITextbox(new Rect(40, 145, 159, 25), ConnectionTab.gameObject, "PasswordInput".LocDef("Password"), "", null, 12, true);
                new UIButton("ConnectButtonText".LocDef("Connect"), new Rect(498, 110, 159, 25), () =>
                {
                    if (String.IsNullOrWhiteSpace(ipTextBox.obj.text))
                    {
                        WindowManager.SpawnDialog("NoIPText".LocDef("Please enter a IP into the text box labeled \"Server IP\""), true, DialogWindow.DialogType.Error);
                    }
                    else if (String.IsNullOrWhiteSpace(portTextBox.obj.text))
                    {
                        WindowManager.SpawnDialog("NoPortText".LocDef("Please enter a Port into the text box labeled \"Server Port\""), true, DialogWindow.DialogType.Error);
                    }
                    else
                    {
                        if (Meta.NetworkingClient.IsConnected)
                        {
                            // If user is already connected to a server.
                            GameObject diagObj = Instantiate(WindowManager.Instance.DialogPrefab, WindowManager.Instance.Canvas.transform, false);
                            DialogWindow diag = diagObj.GetComponent<DialogWindow>();
                            KeyValuePair<string, Action>[] actions = {
                                new KeyValuePair<string, Action>("DisconnnectButton".LocDef("Disconnect"), delegate {
                                    Meta.NetworkingClient.Disconnect();
                                }),
                                new KeyValuePair<string, Action>("CancelButton".LocDef("Cancel"), delegate {
                                    diag.Window.Close();
                                }),
                            };
                            diag.Show("AlreadyConnectedToServer".LocDef("You are already connected to a server, would you like to disconnect?"), !true, DialogWindow.DialogType.Warning, actions);
                        }
                        try
                        {
                            Meta.NetworkingClient.Connect(ipTextBox.obj.text, ushort.Parse(portTextBox.obj.text));
                            WindowManager.SpawnDialog("SuccessfullyConnected".LocDef("Successfully connected to the server!"), true, DialogWindow.DialogType.Error);
                        }
                        catch (Exception e)
                        {
                            WindowManager.SpawnDialog($"There was an error trying to connect to {ipTextBox.obj.text}:{portTextBox.obj.text}, see console for error.", true, DialogWindow.DialogType.Error);
                            Meta.Logger.Error(e);
                        }
                    }
                }, ConnectionTab.gameObject);
            
            WindowManager.AddElementToWindow(ConnectionTab.gameObject, InternalWindow, new Rect(0, 35, 750, 200), Rect.zero);
            
            #endregion

        }

        public string GetLoc() => "MultiplayerButton".LocDef("Multiplayer");

        public Rect GetButtonLocation() => new Rect(274, 0, 100, 32);
        
        public override void OnActivate()
        {
            SceneManager.activeSceneChanged += SceneChange;
        }

        private void SceneChange(Scene scene, Scene prev)
        {
            if (!isActiveAndEnabled) return;
            if (scene.name == "MainScene")
            {
                var gameButton = WindowManager.SpawnButton();
                gameButton.onClick.AddListener(ConstructWindow);
                gameButton.SetText(GetLoc());

                GameObject fanPanel = WindowManager.FindElementPath("MainPanel/Holder/FanPanel").gameObject;
                WindowManager.AddElementToElement(gameButton.gameObject, fanPanel, GetButtonLocation(), Rect.zero);
                
                Meta.Logger.Info("Initalized multiplayer button in MainScene");
            }
        }
        
        public override void OnDeactivate()
        {
            SceneManager.activeSceneChanged -= SceneChange;
        }
    }
}
#endif