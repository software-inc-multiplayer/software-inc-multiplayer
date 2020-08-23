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
        public GUIWindow MPWindow { get; set; }
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
                CreateButton();
            }
        }

        private void CreateButton()
        {
            GameObject FanPanel = WindowManager.FindElementPath("MainPanel/Holder/FanPanel").gameObject;
            MPButton = WindowManager.SpawnButton();
            MPButton.onClick.AddListener(CreateBaseMultiplayerWindow);
            MPButton.SetText("MultiplayerButton".LocDef("Multiplayer"));
            WindowManager.AddElementToElement(MPButton.gameObject, FanPanel, new Rect(264, 0, 100, 32), Rect.zero);
            Logging.Info("Initalized multiplayer button in MainScene");
        }

        private void CreateBaseMultiplayerWindow()
        {
            Logging.Info("Opened multiplayer window.");
            MPWindow = WindowManager.SpawnWindow();
            MPWindow.SetTitle("MultiplayerButton".LocDef("Multiplayer"));
            MPWindow.ShowCentered = true;
            MPWindow.MinSize = new Vector2(640, 480);
            MPWindow.SizeButton.SetActive(false);

            Button connectButton = WindowManager.SpawnButton();
            connectButton.SetText("ConnectButtonText".LocDef("Connect"));
            connectButton.onClick.AddListener(() =>
            {              
                Button goBackButt = WindowManager.SpawnButton();
                goBackButt.SetText("GoBackButton".LocDef("Go back"));
                MPWindow.gameObject.SetActive(false);
                GUIWindow connectWindow = WindowManager.SpawnWindow();
                connectWindow.SetTitle("MultiplayerButtonConnect".LocDef("Multiplayer - Connect"));
                connectWindow.ShowCentered = true;
                connectWindow.MinSize = new Vector2(640, 480);
                connectWindow.SizeButton.SetActive(false);
                connectWindow.AddElement(goBackButt.gameObject, new Rect(640 - 5, 5, 96, 64), Rect.zero);
                if (!Client.Connected || Client.Connected == null)
                {
                    GameObject diagObj = UnityEngine.Object.Instantiate(WindowManager.Instance.DialogPrefab);
                    diagObj.transform.SetParent(WindowManager.Instance.Canvas.transform, worldPositionStays: false);
                    DialogWindow diag = gameObject.GetComponent<DialogWindow>();
                    KeyValuePair<string, Action>[] actions = new KeyValuePair<string, Action>[]
                    {
                        new KeyValuePair<string, Action>("DisconnnectButton".LocDef("Disconnect"), delegate {
                            Client.Disconnect();
                            diag.Window.Close();
                            connectWindow.Show();
                            goBackButt.onClick.AddListener(() =>
                            {
                                connectWindow.gameObject.SetActive(false);
                                MPWindow.gameObject.SetActive(true);
                            });
                        }),
                        new KeyValuePair<string, Action>("CancelButton".LocDef("Cancel"), delegate {
                            diag.Window.Close();
                        }),
                    };
                    diag.Show("AlreadyConnectedToServer".LocDef("You are already connected to a server, would you like to disconnect?"), !true, DialogWindow.DialogType.Warning, actions);
                } else
                {
                    connectWindow.Show();
                    goBackButt.onClick.AddListener(() =>
                    {
                        connectWindow.gameObject.SetActive(false);
                        MPWindow.gameObject.SetActive(true);
                    });
                }            
            });

            Button startServerButton = WindowManager.SpawnButton();
            startServerButton.SetText("StartServerButtonText".LocDef("Create Server"));

            MPWindow.Show();

            MPWindow.AddElement(connectButton.gameObject, new Rect(5, 5, 96, 64), Rect.zero);
            MPWindow.AddElement(startServerButton.gameObject, new Rect(96 + 10, 5, 96, 64), Rect.zero);
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
