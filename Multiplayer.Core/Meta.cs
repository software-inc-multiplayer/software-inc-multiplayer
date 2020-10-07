using Multiplayer.Debugging;
using Multiplayer.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class Meta : ModMeta
    {
        public static ModController.DLLMod ThisMod { get; set; }
        public static bool GiveMeFreedom = true;
        public override string Name => "Software Inc Multiplayer";
        public override void ConstructOptionsScreen(RectTransform parent, bool inGame)
        {
            Button e = WindowManager.SpawnButton();
            e.onClick.AddListener(ErrorTest);
            e.SetText("Test Error");
            WindowManager.AddElementToElement(e.gameObject, parent.gameObject, new Rect(0, 15, 192, 64), Rect.zero);
        }

        private void ErrorTest()
        {
            Logging.Error("Test");
        }

        public override void Initialize(ModController.DLLMod parentMod)
        {
            bool hasInternet = InternetTools.CheckInternetConnection();
            if(!hasInternet)
            {
                WindowManager.SpawnDialog("NoInternetDialog".Loc("You aren't connected to the internet or your internet is not fast enough to use the Multiplayer Mod!\n\nThe multiplayer mod will now unload to save peformance."), true, DialogWindow.DialogType.Error);
                ModController.Instance.UnloadMod(parentMod, false);
                return;
            }
            ThisMod = parentMod;
            Application.runInBackground = true;
            base.Initialize(parentMod);
        }
    }
}
