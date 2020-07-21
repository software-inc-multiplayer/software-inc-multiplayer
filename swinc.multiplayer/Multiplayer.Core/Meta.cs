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

namespace Multiplayer.Core
{
    public class Meta : ModMeta
    {
        public static bool GiveMeFreedom = true;
        public override string Name => "Software Inc Multiplayer";
        public override void ConstructOptionsScreen(RectTransform parent, bool inGame)
        {
            Toggle DebugCheckBox = WindowManager.SpawnCheckbox();
            DebugCheckBox.onValueChanged.AddListener(DebugSetter);
        }
        public override void Initialize(ModController.DLLMod parentMod)
        {
            AppDomain.CurrentDomain.FirstChanceException += ErrorCatcher;
            AppDomain.CurrentDomain.AssemblyResolve += (x, y) => Assembly.LoadFrom(Path.Combine(parentMod.FolderPath(), "References\\" + y.Name.Substring(0, y.Name.IndexOf(",")) + ".dll"));
            base.Initialize(parentMod);
        }
        private void DebugSetter(bool arg)
        {

        }
        private void ErrorCatcher(object sender, FirstChanceExceptionEventArgs e)
        {
            Logging.Error(e.Exception.ToString());
            ErrorHandling.Error error = new ErrorHandling.Error(e.Exception.GetHashCode().ToString(), e.Exception.Message, e.Exception.StackTrace, e.Exception.Source, e.Exception.TargetSite.Name);
        }
    }
}
