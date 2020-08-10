using Multiplayer.Debugging;
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
            RectTransform mainMenuPanel = WindowManager.FindElementPath("SPanel / Panel");
            Button mainMenuButton = WindowManager.SpawnButton();
            mainMenuButton.GetComponentInChildren<Text>().text = "Multiplayer";
            mainMenuButton.GetComponentInChildren<Text>().fontSize = 40;
            mainMenuButton.onClick.AddListener(MainMenuButtonClick);
            mainMenuButton.gameObject.name = "MainMenu_MP_Button";
            LaunchBehaviour.ActiveObjects.Add(mainMenuButton.gameObject);
            WindowManager.AddElementToElement(mainMenuButton.gameObject, mainMenuPanel.gameObject, new Rect(0, 0, 400, 250), Rect.zero);

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
