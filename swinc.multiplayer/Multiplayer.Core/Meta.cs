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

namespace Multiplayer.Extensions
{
    public class Meta : ModMeta
    {
        public static bool DebugMode { get; set; }
        public static ModController.DLLMod ThisMod { get; set; }
        public static bool GiveMeFreedom = true;
        public override string Name => "Software Inc Multiplayer";
        public override void ConstructOptionsScreen(RectTransform parent, bool inGame)
        {          
            Toggle DebugCheckBox = WindowManager.SpawnCheckbox();
            DebugCheckBox.isOn = DebugMode;
            DebugCheckBox.onValueChanged.AddListener(DebugSetter);
            Button e = WindowManager.SpawnButton();
            e.onClick.AddListener(ErrorTest);
            WindowManager.AddElementToElement(DebugCheckBox.gameObject, parent.gameObject, new Rect(0, 15, 64, 64), Rect.zero);
            WindowManager.AddElementToElement(e.gameObject, parent.gameObject, new Rect(0, 60, 128, 64), Rect.zero);
        }

        private void ErrorTest()
        {
            FirstChanceExceptionEventArgs e = new FirstChanceExceptionEventArgs(new Exception("This is a test error. Please ignore."));
            ErrorCatcher(null, e);
        }

        private void DebugSetter(bool arg0)
        {
            ThisMod.SaveSetting("debug", arg0.ToString());
        }

        public override void Initialize(ModController.DLLMod parentMod)
        {
            ThisMod = parentMod;
            ThisMod.Settings.TryGetValue("debug", out string e);
            if (bool.Parse(e)) DebugMode = true;
            else DebugMode = false;
            AppDomain.CurrentDomain.FirstChanceException += ErrorCatcher;
            AppDomain.CurrentDomain.AssemblyResolve += (x, y) => Assembly.LoadFrom(Path.Combine(parentMod.FolderPath(), "References\\" + y.Name.Substring(0, y.Name.IndexOf(",")) + ".dll"));
            base.Initialize(parentMod);
        }
        private void ErrorCatcher(object sender, FirstChanceExceptionEventArgs e)
        {
            Logging.Error(e.Exception.ToString());
            ErrorHandling.Error error = new ErrorHandling.Error(e.Exception.GetHashCode().ToString(), e.Exception.Message, e.Exception.StackTrace, e.Exception.Source, e.Exception.TargetSite.Name);
            ErrorHandling.Handle(error, DebugMode);
        }
    }
    public class Behaviour : ModBehaviour
    {
        public override void OnActivate()
        {
            
        }

        public override void OnDeactivate()
        {
            
        }
    }
}
