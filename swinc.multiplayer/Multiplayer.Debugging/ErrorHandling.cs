using UnityEngine;
using UnityEngine.UI;
using Multiplayer.Extensions;

namespace Multiplayer.Debugging
{
    public class ErrorHandling
    {
        public static void Handle(Error error, bool DebugMode)
        {
            if (DebugMode)
            {
                GUIWindow errorWindow = WindowManager.SpawnWindow();
                var title = WindowManager.SpawnLabel();
                title.text = "Error!";
                errorWindow.TitleText = title;
                var panel = errorWindow.MainPanel;
                Text errorInfo = WindowManager.SpawnLabel();
                errorInfo.text = $"We found an error:\n\n<color=red><b>{error.ToString()}</b></color>\n\n";
                errorWindow.MinSize = new Vector2(Screen.width / 2.5f, Screen.height / 2.5f);
                errorWindow.Show();
                PopupManager.NotificationSound.Issue.Play();
                WindowManager.AddElementToWindow(errorInfo.gameObject, errorWindow, new Rect(0, 15, 450, 50), Rect.zero);
            }
            if (error.Message == "This is a test error. Please ignore.") return;
        }
        public class Error
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public string Source { get; set; }
            public string TargetSite { get; set; }
            public Error()
            {

            }
            public Error(string code)
            {
                Code = code;
            }
            public Error(string code, string message)
            {
                Code = code;
                Message = message;
            }
            public Error(string code, string message, string stackTrace)
            {
                Code = code;
                Message = message;
                StackTrace = stackTrace;
            }
            public Error(string code, string message, string stackTrace, string source)
            {
                Code = code;
                Message = message;
                StackTrace = stackTrace;
                Source = source;
            }
            public Error(string code, string message, string stackTrace, string source, string targetSite)
            {
                Code = code;
                Message = message;
                StackTrace = stackTrace;
                Source = source;
                TargetSite = targetSite;
            }
        }
    }
}
