using Multiplayer.Debugging;
using Multiplayer.Extensions;
using Multiplayer.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class MainMenuManager : ModBehaviour
    {
        public override void OnActivate()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == "MainMenu")
            {
                CreateButton();
            }
            SceneManager.sceneLoaded += SceneChanged;
        }
        private void CreateButton()
        {
            RectTransform mainMenuPanel = WindowManager.FindElementPath("MainPanel/Panel");
            RectTransform toCopy = WindowManager.FindElementPath("MainPanel/Panel/Button 5");
            Button butt = Instantiate(toCopy).GetComponent<Button>();
            butt.onClick.RemoveAllListeners();
            butt.onClick = new Button.ButtonClickedEvent();
            butt.onClick.AddListener(MainMenuButtonClick);
            butt.GetComponentInChildren<Text>().text = "Multiplayer";
            Sprite texture = butt.image.sprite;           
            LaunchBehaviour.ActiveObjects.Add(butt.gameObject);
            WindowManager.AddElementToElement(butt.gameObject, mainMenuPanel.gameObject, new Rect(0, 0, texture.rect.width, texture.rect.height), Rect.zero);
            butt.transform.SetSiblingIndex(1);
            Logging.Debug("Added menu button.");
        }
        private void SceneChanged(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "MainMenu")
            {
                isWindowUp = false;
                return;
            }
            CreateButton();
        }
        private bool isWindowUp = false;
        private void MainMenuButtonClick()
        {
            if (isWindowUp)
            {
                PopupManager.NotificationSound.Warning.Play();
                return;
            }
            isWindowUp = true;
            GUIWindow window = WindowManager.SpawnWindow();

            Button OptionsButton = WindowManager.SpawnButton();
            OptionsButton.gameObject.GetComponentInChildren<Text>().text = "MPOptionsWindow_OptionsButton".LocDef("Options");


            Button ServerIButton = WindowManager.SpawnButton();
            ServerIButton.gameObject.GetComponentInChildren<Text>().text = "MPOptionsWindow_ServerIButton".LocDef("Server Info");
          
            window.NonLocTitle = "Manage Multiplayer";
            window.Title = "MPOptionsWindowTitle".LocDef("Manage Multiplayer");
            window.MinSize = new Vector2(128f + 1f + 128f, Screen.height / 2f);
            window.ShowCentered = true;            
            window.Show();
            window.SizeButton.SetActive(false);
            WindowManager.AddElementToElement(OptionsButton.gameObject, window.MainPanel, new Rect(0, 0, 128, 32), Rect.zero);
            WindowManager.AddElementToElement(ServerIButton.gameObject, window.MainPanel, new Rect(129, 0, 128, 32), Rect.zero);

            GameObject OptionsPanel = new GameObject();

            #region Options Panel

            #endregion

            GameObject ServerIPanel = new GameObject();

            #region Server Info Panel
            Text SHeader = WindowManager.SpawnLabel();
            SHeader.text = "MPOptionsWindow_SHeader".LocDef("Your server:");
            SHeader.fontSize = 16;
            
            Text SConnectIP = WindowManager.SpawnLabel();
            SConnectIP.text = "MPOptionsWindow_SConnectIP".LocDef("Server IP:") + $" <color=blue>{IPUtils.GetIP()}:{ServerClass.GetServerPort()}</color>";


            SConnectIP.gameObject.transform.SetParent(ServerIPanel.transform);
            SHeader.gameObject.transform.SetParent(ServerIPanel.transform);
            #endregion

            OptionsButton.onClick.AddListener(() => {
                ServerIPanel.SetActive(false);
                OptionsPanel.SetActive(true);
            });
            ServerIButton.onClick.AddListener(() => {
                OptionsPanel.SetActive(false);
                ServerIPanel.SetActive(true);
            });
            ServerIPanel.SetActive(false);
            OptionsPanel.SetActive(true);
            WindowManager.AddElementToElement(SHeader.gameObject, window.MainPanel, new Rect(0, 38, 192, 32), Rect.zero);
            WindowManager.AddElementToElement(SConnectIP.gameObject, window.MainPanel, new Rect(0, 55, 192, 42), Rect.zero);
            window.OnClose += () =>
            {
                isWindowUp = false;
            };
        }

        public override void OnDeactivate()
        {

        }
    }
}
