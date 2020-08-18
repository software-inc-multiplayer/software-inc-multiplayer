using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Multiplayer.Debugging;

namespace Multiplayer.Core
{
    public class GameBehaviour : ModBehaviour
    {
        public Button MPButton { get; set; }
        public bool isEnabled { get; set; }
        public override void OnActivate()
        {
            isEnabled = true;
            SceneManager.sceneLoaded += OnScene;
            if (SceneManager.GetActiveScene().name == "MainScene")
            {
                CreateButton();
            }
        }

        private void OnScene(Scene arg0, LoadSceneMode arg1)
        {
            if (!isEnabled) return;
            if (arg0.name == "MainScene")
            {
                CreateButton();
            }
        }

        private void CreateButton()
        {
            GameObject FanPanel = WindowManager.FindElementPath("MainPanel/Holder/FanPanel").gameObject;
            MPButton = WindowManager.SpawnButton();
            MPButton.onClick.AddListener(CreateMultiplayerWindow);
            MPButton.GetComponentInChildren<Text>().text = "MultiplayerButton".LocDef("Multiplayer");         
            WindowManager.AddElementToElement(MPButton.gameObject, FanPanel, new Rect(264, 0, 100, 32), Rect.zero);
            Logging.Info("Initalized multiplayer button in MainScene");
        }

        private void CreateMultiplayerWindow()
        {
            Logging.Info("Opened multiplayer window.");
        }

        public override void OnDeactivate()
        {
            MPButton.gameObject.SetActive(false);
            Logging.Info("Destroyed multiplayer button in MainScene");
            isEnabled = false;           
        }
    }
}
