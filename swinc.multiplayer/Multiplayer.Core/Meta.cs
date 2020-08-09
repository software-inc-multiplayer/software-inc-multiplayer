using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Multiplayer.Debugging;
using Multiplayer.Networking;

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
            WindowManager.AddElementToElement(e.gameObject, parent.gameObject, new Rect(0, 15, 64, 64), Rect.zero);
        }

        private void ErrorTest()
        {
            Logging.Error("Test");
        }

        public override void Initialize(ModController.DLLMod parentMod)
        {
            AppDomain.CurrentDomain.UnhandledException += ErrorCatcher;
            AppDomain.CurrentDomain.AssemblyResolve += (x, y) => Assembly.LoadFrom(Path.Combine(parentMod.FolderPath(), "References\\" + y.Name.Substring(0, y.Name.IndexOf(",")) + ".dll"));
            base.Initialize(parentMod);
        }
        private void ErrorCatcher(object sender, UnhandledExceptionEventArgs e)
        {
            Exception t = (Exception)e.ExceptionObject;
            Logging.Error(t.ToString());
        }
    }
}
