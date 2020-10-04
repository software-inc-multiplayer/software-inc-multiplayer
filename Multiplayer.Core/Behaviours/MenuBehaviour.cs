using Multiplayer.Debugging;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class MenuBehaviour : ModBehaviour
    {
        public string OldText { get; set; }
        public override void OnActivate()
        {
            SceneManager.sceneLoaded += OnScene;
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                ModifyText();
            }
        }

        private void OnScene(Scene arg0, LoadSceneMode arg1)
        {
            if (!isActiveAndEnabled) return;
            if (arg0.name == "MainMenu")
            {
                ModifyText();
            }
        }

        private void ModifyText()
        {
            OldText = Versioning.SimpleVersionString;
            string newText = "Multiplayer Mod v0.3.2-closed - " + OldText.Replace(",", "");
            WindowManager.FindElementPath("MainPanel/Text[2]").GetComponent<Text>().text = newText;
        }

        public override void OnDeactivate()
        {
            SceneManager.sceneLoaded -= OnScene;
            if (SceneManager.GetActiveScene().name == "MainMenu")
            {
                WindowManager.FindElementPath("MainPanel/Text[2]").GetComponent<Text>().text = OldText;
            }
            Logging.OnDisable();
        }
    }
}
