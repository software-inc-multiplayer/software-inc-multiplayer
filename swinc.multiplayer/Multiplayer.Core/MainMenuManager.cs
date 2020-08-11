using Multiplayer.Debugging;
using System.Collections;
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
            if (scene.name != "MainMenu") return;
            CreateButton();
        }

        private void MainMenuButtonClick()
        {
            WindowManager.SpawnDialog("CS_Dialog".LocDef("Coming soon."), false, DialogWindow.DialogType.Information);          
        }

        public override void OnDeactivate()
        {

        }
    }
}
