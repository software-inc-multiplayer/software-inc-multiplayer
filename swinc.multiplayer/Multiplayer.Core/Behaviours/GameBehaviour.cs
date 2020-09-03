using Multiplayer.Debugging;
using Multiplayer.Extensions;
using Multiplayer.Networking;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class GameBehaviour : ModBehaviour
    {
        public Button MPButton { get; set; }
        public int PreviousCount { get; set; }
        public GUIWindow MPWindow { get; set; }
        public Text ChatWindow { get; set; }
        public override void OnActivate()
        {
            SceneManager.sceneLoaded += OnScene;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                CreateButton();
            }
        }

        private void OnScene(Scene arg0, LoadSceneMode arg1)
        {
            if (!isActiveAndEnabled) return;
            if (arg0.name == "MainScene")
            {
                //HUD.Instance.AddPopupMessage("Thanks for using the Multiplayer mod.\nJoin the discord: discord.io/multiplayer-mod", "Smiley", PopupManager.NotificationSound.Good, 1);
                CreateButton();
            }
        }

        private void CreateButton()
        {
            GameObject FanPanel = WindowManager.FindElementPath("MainPanel/Holder/FanPanel").gameObject;
            MPButton = WindowManager.SpawnButton();
            MPButton.onClick.AddListener(CreateBaseMultiplayerWindow);
            MPButton.SetText("MultiplayerButton".LocDef("Multiplayer"));
            WindowManager.AddElementToElement(MPButton.gameObject, FanPanel, new Rect(274, 0, 100, 32), Rect.zero);
            Logging.Info("Initalized multiplayer button in MainScene");
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

            new Utils.Controls.Element.UIButton("ConnectButtonText".LocDef("Connect"), new Rect(30, 30, 159, 25), () =>
            {
                #region Connect Window
                MPWindow.gameObject.SetActive(false);

                /**
                 * Box 200, 200, 750, 200 "Multiplayer - Connect"
                 * Button 230, 275, 159, 25 "Go Back"
                  Label 230, 230, 700, 45 "Connect to a multiplayer server via IP and Port, specify a password if the server has a password set."
                  Input 230, 310, 159, 25 "IP Address"
                  Input 399, 310, 159, 25 "Port"
                  Input 230, 345, 159, 25 "Password"
                  Button 588, 310, 159, 25 "Connect"
                **/

                GUIWindow connectWindow = WindowManager.SpawnWindow();
                connectWindow.SetTitle("MultiplayerButtonConnect".LocDef("Multiplayer - Connect"));
                connectWindow.ShowCentered = true;
                connectWindow.MinSize = new Vector2(750, 200);
                connectWindow.SizeButton.SetActive(false);

                Utils.Controls.Element.UIButton goBackButton = new Utils.Controls.Element.UIButton("GoBackButton".LocDef("Go Back"), new Rect(30, 75, 159, 25), () =>
                {
                    connectWindow.gameObject.SetActive(false);
                    MPWindow.gameObject.SetActive(true);
                }, connectWindow.MainPanel, "GoBackButton", "GoBackTooltip".LocDef("Go back to the main multiplayer window."));

                new Utils.Controls.Element.UILabel(
                    "CreateLabel".LocDef("Create a multiplayer server on the specified port, leave password blank to disable password verification."),
                    new Rect(30, 30, 700, 45),
                    connectWindow.MainPanel);

                Utils.Controls.Element.UITextbox PortTextBox = new Utils.Controls.Element.UITextbox(new Rect(30, 110, 159, 25), connectWindow.MainPanel, "PortInput".LocDef("Port"), "", null, 12);
                Utils.Controls.Element.UITextbox PasswordTextBox = new Utils.Controls.Element.UITextbox(new Rect(35 + 159, 110, 159, 25), connectWindow.MainPanel, "PasswordInput".LocDef("Password"), "", null, 12, true);
                Utils.Controls.Element.UIButton ConnectButton = new Utils.Controls.Element.UIButton("StartButtonText".LocDef("Start"), new Rect(488, 110, 159, 25), () =>
                {

                    if (string.IsNullOrWhiteSpace(PortTextBox.obj.text) || !ushort.TryParse(PortTextBox.obj.text, out ushort nedfro))
                    {
                        WindowManager.SpawnDialog("NoPortText".LocDef("Please enter a valid Port into the text box labeled \"Port\""), true, DialogWindow.DialogType.Error);
                        return;
                    }
                    else
                    {
                        if(Client.Connected)
                        {
                            Client.Disconnect();
                        }
                        if (Networking.Server.isRunning)
                        {
                            // If user is already connected to a server.
                            DialogWindow diag = WindowManager.SpawnDialog();
                            KeyValuePair<string, Action>[] actions = new KeyValuePair<string, Action>[]
                            {
                                new KeyValuePair<string, Action>("StopButtonText".LocDef("Stop Server"), delegate {
                                    Networking.Server.Stop();
                                    diag.Window.Close();
                                    try
                                    {
                                        Networking.Server.Start(ushort.Parse(PortTextBox.obj.text));                                        
                                    }
                                    catch (Exception e)
                                    {
                                        WindowManager.SpawnDialog($"There was an error trying to create a server at port {PortTextBox.obj.text}, see console for error.", true, DialogWindow.DialogType.Error);
                                        Logging.Error(e);
                                        return;
                                    }
                                    WindowManager.SpawnDialog("SuccessfullyCreated".LocDef("Successfully created server!"), true, DialogWindow.DialogType.Information);
                                }),
                                new KeyValuePair<string, Action>("CancelButton".LocDef("Cancel"), delegate {
                                    diag.Window.Close();
                                }),
                            };
                            diag.Show("AlreadyServer".LocDef("You already have a server started, would you like to stop it?"), !true, DialogWindow.DialogType.Warning, actions);
                            return;
                        }
                        Networking.Server.Start(ushort.Parse(PortTextBox.obj.text));
                    }
                    connectWindow.gameObject.SetActive(false);
                }, connectWindow.MainPanel);
                connectWindow.Show();
                #endregion
            }, MPWindow.MainPanel);
            new Utils.Controls.Element.UIButton("ServerButtonText".LocDef("Manage/Create Server"), new Rect(199, 30, 159, 25), () =>
            {

                MPWindow.gameObject.SetActive(false);

                GUIWindow ServerManageWindow = WindowManager.SpawnWindow();
                ServerManageWindow.SetTitle("MultiplayerButtonConnect".LocDef("Multiplayer - Connect"));
                ServerManageWindow.ShowCentered = true;
                ServerManageWindow.MinSize = new Vector2(750, 200);
                ServerManageWindow.SizeButton.SetActive(false);

                Utils.Controls.Element.UIButton goBackButton = new Utils.Controls.Element.UIButton("GoBackButton".LocDef("Go Back"), new Rect(30, 75, 159, 25), () =>
                {
                    ServerManageWindow.gameObject.SetActive(false);
                    MPWindow.gameObject.SetActive(true);
                }, ServerManageWindow.MainPanel, "GoBackButton", "GoBackTooltip".LocDef("Go back to the main multiplayer window."));

                new Utils.Controls.Element.UILabel(
                    "ConnectLabel".LocDef("Create a multiplayer server at ease."),
                    new Rect(30, 30, 700, 45),
                    ServerManageWindow.MainPanel);

                Utils.Controls.Element.UITextbox PortTextBox = new Utils.Controls.Element.UITextbox(new Rect(30, 110, 159, 25), ServerManageWindow.MainPanel, "Port", "", null, 12);
                Utils.Controls.Element.UITextbox PasswordTextBox = new Utils.Controls.Element.UITextbox(new Rect(35 + 159, 110, 159, 25), ServerManageWindow.MainPanel, "Password".LocDef("Password"), "", null, 12);
                Utils.Controls.Element.UIButton ConnectButton = new Utils.Controls.Element.UIButton("ConnectButtonText".LocDef("Start"), new Rect(488, 110, 159, 25), () =>
                {
                    if (String.IsNullOrEmpty(PortTextBox.obj.text))
                    {

                    }
                }, ServerManageWindow.MainPanel);
                ServerManageWindow.Show();
            }, MPWindow.MainPanel);
            new Utils.Controls.Element.UIButton("GameplayButtonText".LocDef("Gameplay"), new Rect(369, 30, 159, 25), () =>
            {
                WindowManager.SpawnDialog("ComingSoon".LocDef("Coming soon!"), true, DialogWindow.DialogType.Error);
            }, MPWindow.MainPanel);

            Client.chatWindow = WindowManager.SpawnLabel();
            Client.chatWindow.text = "NoMessages".LocDef("Its pretty quiet in here, seems to be no sign of chat messages anywhere!");
            MPWindow.AddElement(Client.chatWindow.gameObject, new Rect(30, 75, 670, 255), Rect.zero);
            Utils.Controls.Element.UITextbox chatBox = new Utils.Controls.Element.UITextbox(new Rect(30, 390, 471, 45), MPWindow.MainPanel, "TypeToChat".LocDef("Type here to chat..."), "chatBox", null, 15, false);
            Utils.Controls.Element.UIButton sendButton = new Utils.Controls.Element.UIButton("Send", new Rect(541, 390, 159, 45), () =>
            {
                if (!Client.Connected)
                {
                    WindowManager.SpawnDialog("NotConnectedToServer".LocDef("You aren't connected to a server!"), true, DialogWindow.DialogType.Error);
                    return;
                }
                var tmpUser = new Helpers.User();
                tmpUser.Username = Client.Username;
                Helpers.TcpChat chatClass = new Helpers.TcpChat(chatBox.obj.text, tmpUser);
                chatBox.obj.text = "";
                Client.Send(chatClass);
            }, MPWindow.MainPanel);
            MPWindow.Show();
        }

        public override void OnDeactivate()
        {
            SceneManager.sceneLoaded -= OnScene;
            if (MPButton != null)
                MPButton.gameObject.SetActive(false);
            Logging.Info("Destroyed multiplayer button in MainScene");
        }
    }
}
