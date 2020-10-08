using Multiplayer.Debugging;
using Multiplayer.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class Meta : ModMeta
    {
        public static UnityLogger Logging { get; set; }
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
            Meta.Logging.Start();
            Logging = new UnityLogger();
            ThisMod = parentMod;
            Application.runInBackground = true;
            base.Initialize(parentMod);
        }
    }
}
