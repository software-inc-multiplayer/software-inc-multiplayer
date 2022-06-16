using System;
using System.Collections.Generic;
using Facepunch.Steamworks;
using Multiplayer.Extensions;
using Multiplayer.Packets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Multiplayer.Core.Behaviours
{
    public class MultiplayerMenuBehaviour : ModBehaviour
    {
        public Button MpButton { get; set; }
        public GUIWindow MpWindow { get; set; }
        public static Text ChatWindow { get; set; }
        
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
            MpButton = WindowManager.SpawnButton();
            MpButton.onClick.AddListener(CreateBaseMultiplayerWindow);
            MpButton.SetText("MultiplayerButton".LocDef("Multiplayer"));
            WindowManager.AddElementToElement(MpButton.gameObject, FanPanel, new Rect(274, 0, 100, 32), Rect.zero);
            Meta.Logger.Info("Initalized multiplayer button in MainScene");
        }

        private void CreateBaseMultiplayerWindow()
        {
            Meta.Logger.Info("Opened multiplayer window.");
            if (MpWindow != null)
            {
                MpWindow.Show();
                return;
            }
            MpWindow = WindowManager.SpawnWindow();
            MpWindow.SetTitle("MultiplayerButton".LocDef("Multiplayer"));
            MpWindow.ShowCentered = true;
            MpWindow.MinSize = new Vector2(730, 500);
            MpWindow.SizeButton.SetActive(false);

            new Utils.Controls.Element.UIButton("ConnectButtonText".LocDef("Connect"), new Rect(30, 30, 159, 25), () =>
            {
                #region Connect Window
                MpWindow.gameObject.SetActive(false);

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
                    MpWindow.gameObject.SetActive(true);
                }, connectWindow.MainPanel, "GoBackButton", "GoBackTooltip".LocDef("Go back to the main multiplayer window."));

                new Utils.Controls.Element.UILabel(
                    "ConnectLabel".LocDef("Connect to a multiplayer server via IP and Port, specify a password if the server has a password set."),
                    new Rect(30, 30, 700, 45),
                    connectWindow.MainPanel);

                Utils.Controls.Element.UITextbox IpTextBox = new Utils.Controls.Element.UITextbox(new Rect(30, 110, 159, 25), connectWindow.MainPanel, "IP", "", null, 12);
                Utils.Controls.Element.UITextbox PortTextBox = new Utils.Controls.Element.UITextbox(new Rect(35 + 159, 110, 159, 25), connectWindow.MainPanel, "PortInput".LocDef("Port"), "", null, 12);
                Utils.Controls.Element.UITextbox PasswordTextBox = new Utils.Controls.Element.UITextbox(new Rect(30, 145, 159, 25), connectWindow.MainPanel, "PasswordInput".LocDef("Password"), "", null, 12, true);
                new Utils.Controls.Element.UIButton("ConnectButtonText".LocDef("Connect"), new Rect(488, 110, 159, 25), () =>
                {
                    if (String.IsNullOrWhiteSpace(IpTextBox.obj.text))
                    {
                        WindowManager.SpawnDialog("NoIPText".LocDef("Please enter a IP into the text box labeled \"Server IP\""), true, DialogWindow.DialogType.Error);
                        return;
                    }
                    else if (String.IsNullOrWhiteSpace(PortTextBox.obj.text))
                    {
                        WindowManager.SpawnDialog("NoPortText".LocDef("Please enter a Port into the text box labeled \"Server Port\""), true, DialogWindow.DialogType.Error);
                        return;
                    }
                    else
                    {
                        if (Meta.NetworkingClient.IsConnected)
                        {
                            // If user is already connected to a server.
                            GameObject diagObj = UnityEngine.Object.Instantiate(WindowManager.Instance.DialogPrefab, WindowManager.Instance.Canvas.transform, false);
                            DialogWindow diag = gameObject.GetComponent<DialogWindow>();
                            KeyValuePair<string, Action>[] actions = new KeyValuePair<string, Action>[]
                            {
                                new KeyValuePair<string, Action>("DisconnnectButton".LocDef("Disconnect"), delegate {
                                    Meta.NetworkingClient.Disconnect();
                                }),
                                new KeyValuePair<string, Action>("CancelButton".LocDef("Cancel"), delegate {
                                    diag.Window.Close();
                                    return;
                                }),
                            };
                            diag.Show("AlreadyConnectedToServer".LocDef("You are already connected to a server, would you like to disconnect?"), !true, DialogWindow.DialogType.Warning, actions);
                        }
                        try
                        {
                            Meta.NetworkingClient.Connect(IpTextBox.obj.text, ushort.Parse(PortTextBox.obj.text));
                            WindowManager.SpawnDialog("SuccessfullyConnected".LocDef("Successfully connected to the server!"), true, DialogWindow.DialogType.Error);
                        }
                        catch (Exception e)
                        {
                            WindowManager.SpawnDialog($"There was an error trying to connect to {IpTextBox.obj.text}:{PortTextBox.obj.text}, see console for error.", true, DialogWindow.DialogType.Error);
                            Meta.Logger.Error(e);
                            return;
                        }
                    }
                }, connectWindow.MainPanel);
                #region Window Show management.
                connectWindow.Show();
                #endregion
                #endregion
            }, MpWindow.MainPanel);
            new Utils.Controls.Element.UIButton("ServerButtonText".LocDef("Create Server"), new Rect(199, 30, 159, 25), () =>
            {
                #region Create Server Window
                MpWindow.gameObject.SetActive(false);

                GUIWindow connectWindow = WindowManager.SpawnWindow();
                connectWindow.SetTitle("MultiplayerButtonSS".LocDef("Multiplayer - Create Server"));
                connectWindow.ShowCentered = true;
                connectWindow.MinSize = new Vector2(750, 200);
                connectWindow.SizeButton.SetActive(false);

                Utils.Controls.Element.UIButton goBackButton = new Utils.Controls.Element.UIButton("GoBackButton".LocDef("Go Back"), new Rect(30, 75, 159, 25), () =>
                {
                    connectWindow.gameObject.SetActive(false);
                    MpWindow.gameObject.SetActive(true);
                }, connectWindow.MainPanel, "GoBackButton", "GoBackTooltip".LocDef("Go back to the main multiplayer window."));

                new Utils.Controls.Element.UILabel(
                    "CreateLabel".LocDef("Create a multiplayer server on the specified port, leave password blank to disable password verification."),
                    new Rect(30, 30, 700, 45),
                    connectWindow.MainPanel);

                Utils.Controls.Element.UITextbox PortTextBox = new Utils.Controls.Element.UITextbox(new Rect(30, 110, 159, 25), connectWindow.MainPanel, "PortInput".LocDef("Port"), "", null, 12);
                Utils.Controls.Element.UITextbox PasswordTextBox = new Utils.Controls.Element.UITextbox(new Rect(35 + 159, 110, 159, 25), connectWindow.MainPanel, "PasswordInput".LocDef("Password"), "", null, 12, true);
                Utils.Controls.Element.UIButton ConnectButton = new Utils.Controls.Element.UIButton("StartButtonText".LocDef("Start"), new Rect(488, 110, 159, 25), () =>
                {
                    DialogWindow dia2g = WindowManager.SpawnDialog();
                    if (string.IsNullOrWhiteSpace(PortTextBox.obj.text) || !ushort.TryParse(PortTextBox.obj.text, out ushort nedfro))
                    {
                        WindowManager.SpawnDialog("NoPortText".LocDef("Please enter a valid Port into the text box labeled \"Port\""), true, DialogWindow.DialogType.Error);
                        return;
                    }
                    else
                    {
                        KeyValuePair<string, Action>[] action2s = new KeyValuePair<string, Action>[]
                        {
                                new KeyValuePair<string, Action>("Yes".LocDef("Yes"), delegate {
                                    if(Meta.NetworkingClient.IsConnected) Meta.NetworkingClient.Disconnect();
                                    dia2g.Window.Close();
                                    try
                                    {
                                        Meta.NetworkingClient.Connect("127.0.0.1", ushort.Parse(PortTextBox.obj.text));
                                    }
                                    catch (Exception e)
                                    {
                                        WindowManager.SpawnDialog($"There was an error trying to connect to the server. See console for error.", true, DialogWindow.DialogType.Error);
                                        Meta.Logger.Error(e);
                                        return;
                                    }
                                    WindowManager.SpawnDialog("SuccessfullyCreated".LocDef("Successfully created server!"), true, DialogWindow.DialogType.Information);
                                }),
                                new KeyValuePair<string, Action>("No".LocDef("No"), delegate {
                                    dia2g.Window.Close();
                                }),
                        };
                        if (Meta.NetworkingClient.IsConnected)
                        {
                            Meta.NetworkingClient.Disconnect();
                        }
                        if (Meta.NetworkingServer.Server != null)
                        {
                            // If user is already connected to a server.
                            DialogWindow diag = WindowManager.SpawnDialog();
                            KeyValuePair<string, Action>[] actions = new KeyValuePair<string, Action>[]
                            {
                                new KeyValuePair<string, Action>("StopButtonText".LocDef("Stop Server"), delegate {
                                    Meta.NetworkingServer.Dispose();
                                    diag.Window.Close();
                                    try
                                    {
                                        Meta.NetworkingServer.Host("", "", ushort.Parse(PortTextBox.obj.text));
                                    }
                                    catch (Exception e)
                                    {
                                        WindowManager.SpawnDialog($"There was an error trying to create a server at port {PortTextBox.obj.text}, see console for error.", true, DialogWindow.DialogType.Error);
                                        Meta.Logger.Error(e);
                                        return;
                                    }
                                    dia2g.Show("LikeToConnect".LocDef("Would you like to connect to the server you have created?"), !true, DialogWindow.DialogType.Question, action2s);
                                }),
                                new KeyValuePair<string, Action>("CancelButton".LocDef("Cancel"), delegate {
                                    diag.Window.Close();
                                }),
                            };
                            diag.Show("AlreadyServer".LocDef("You already have a server started, would you like to stop it?"), !true, DialogWindow.DialogType.Warning, actions);
                            return;
                        }
                        Meta.NetworkingServer.Host("Test", "Test Server", ushort.Parse(PortTextBox.obj.text));
                        dia2g.Show("LikeToConnect".LocDef("Would you like to connect to the server you have created?"), !true, DialogWindow.DialogType.Question, action2s);
                    }
                    connectWindow.gameObject.SetActive(false);
                }, connectWindow.MainPanel);
                connectWindow.Show();
                #endregion
            }, MpWindow.MainPanel);
            new Utils.Controls.Element.UIButton("GameplayButtonText".LocDef("Gameplay"), new Rect(369, 30, 159, 25), () =>
            {
                WindowManager.SpawnDialog("ComingSoon".LocDef("Coming soon!"), true, DialogWindow.DialogType.Error);
            }, MpWindow.MainPanel);

            ChatWindow = WindowManager.SpawnLabel();
            ChatWindow.text = "NoMessages".LocDef("No messages... yet.");
            MpWindow.AddElement(ChatWindow.gameObject, new Rect(30, 75, 670, 255), Rect.zero);
            Utils.Controls.Element.UITextbox chatBox = new Utils.Controls.Element.UITextbox(new Rect(30, 390, 471, 45), MpWindow.MainPanel, "TypeToChat".LocDef("Type here to chat..."), "chatBox", null, 15, false);
            Utils.Controls.Element.UIButton sendButton = new Utils.Controls.Element.UIButton("Send", new Rect(541, 390, 159, 45), () =>
            {
                if (!Meta.NetworkingClient.IsConnected)
                {
                    WindowManager.SpawnDialog("NotConnectedToServer".LocDef("You aren't connected to a server!"), true, DialogWindow.DialogType.Error);
                    return;
                }
                var chatClass = new ChatMessage()
                {
                    Username = SteamClient.Name,
                    Contents = chatBox.obj.text
                };
                chatBox.obj.text = "";
                Meta.NetworkingClient.Client.Send(chatClass);
            }, MpWindow.MainPanel);
            MpWindow.Show();
        }

        public override void OnDeactivate()
        {
            SceneManager.sceneLoaded -= OnScene;
            if (MpButton != null)
                MpButton.gameObject.SetActive(false);
            Meta.Logger.Info("Destroyed multiplayer button in MainScene");
        }
    }
}