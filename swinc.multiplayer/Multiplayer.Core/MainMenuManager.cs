using Multiplayer.Debugging;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class MainMenuManager : ModBehaviour
    {
        public Button mainMenuButton { get; set; }
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
            Sprite texture = toCopy.gameObject.GetComponent<Button>().image.sprite;           
            mainMenuButton = WindowManager.SpawnButton();
            mainMenuButton.gameObject.GetComponentInChildren<Text>().text = "MainMenu_MP_Button".LocDef("Multiplayer");
            mainMenuButton.onClick.AddListener(MainMenuButtonClick);
            mainMenuButton.gameObject.name = "MainMenu_MP_Button";
            mainMenuButton.image.sprite = texture;
            mainMenuButton.gameObject.GetComponentInChildren<Text>().fontSize = toCopy.GetComponent<Button>().GetComponentInChildren<Text>().fontSize;
            LaunchBehaviour.ActiveObjects.Add(mainMenuButton.gameObject);
            WindowManager.AddElementToElement(mainMenuButton.gameObject, mainMenuPanel.gameObject, new Rect(0, 0, texture.rect.width, texture.rect.height), Rect.zero);                       
            Logging.Debug("Added menu button.");
            Logging.Debug(texture.name);
        }
        private void SceneChanged(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "MainMenu") return;
            CreateButton();
        }

        private void MainMenuButtonClick()
        {
            Sprite old = mainMenuButton.image.sprite;
            mainMenuButton.image.sprite = Resources.Load("grey_button13") as Sprite;
            WindowManager.SpawnDialog("CS_Dialog".LocDef("Coming soon."), false, DialogWindow.DialogType.Information);
            StartCoroutine(liftButton(old));           
        }
        IEnumerator liftButton(Sprite old)
        {
            yield return new WaitForSeconds(1);
            mainMenuButton.image.sprite = old;
        }

        public override void OnDeactivate()
        {
            mainMenuButton = null;
        }
    }
}
