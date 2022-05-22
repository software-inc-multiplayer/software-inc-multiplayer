using System;
using System.Collections.Generic;
using Facepunch.Steamworks;
using Multiplayer.Extensions;
using Multiplayer.Networking.Shared;
using Multiplayer.Packets;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;




namespace Multiplayer.Core.Behaviours
{
    public class MainMenuBehaviour : ModBehaviour
    {
        public Button MpButton { get; set; }
        public GUIWindow MpWindow { get; set; }
        public static Text ChatWindow { get; set; }
        public override void OnActivate()
        {
            SceneManager.sceneLoaded += OnScene;
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                Meta.Logger.Info("OnActivate Called MainMenu is the scene");
                CreateButton();
            }
        }
        private void OnScene(Scene arg0, LoadSceneMode arg1)
        {
            if (!isActiveAndEnabled) return;
            if (arg0.name == "MainMenu")
            {
                Meta.Logger.Info("OnScene function called scene is MainMenu");
                CreateButton();
            }
        }
        private void CreateButton()
        {
            Meta.Logger.Info("CreateButton Called");
            GameObject FanPanel = WindowManager.FindElementPath("MainPanel/Panel/Button 2").gameObject;
            MpButton = WindowManager.SpawnButton();
            /*MpButton.onClick.AddListener(CreateBaseMultiplayerWindow);*/
            MpButton.SetText("MultiplayerButton".LocDef("Multiplayer"));
            WindowManager.AddElementToElement(MpButton.gameObject, FanPanel, new Rect(274, 0, 100, 32), Rect.zero);
            Meta.Logger.Info("Initalized multiplayer button in MenuScene");
        }

        public override void OnDeactivate()
        {
            SceneManager.sceneLoaded -= OnScene;
            Meta.Logger.Info("Destroyed multiplayer button in MenuScene");
        }
    }
}
