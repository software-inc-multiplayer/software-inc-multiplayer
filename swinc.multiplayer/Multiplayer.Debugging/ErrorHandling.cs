using Multiplayer.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer.Debugging
{
    public class ErrorHandling
    {
        public static void Handle(Error error)
        {
            GUIWindow errorWindow = WindowManager.SpawnWindow();
            errorWindow.Show();
            errorWindow.Title = "Error!";
            var panel = errorWindow.MainPanel;
            Text errorInfo = WindowManager.SpawnLabel();
            errorInfo.supportRichText = true;
            errorInfo.text = $"We found an error:\n\n<color=red><b>{error.ToString()}</b></color>\n\n";
            errorWindow.MinSize = new Vector2(Screen.width / 2.5f, Screen.height / 2.5f);
            
            PopupManager.NotificationSound.Issue.Play();
            WindowManager.AddElementToElement(errorInfo.gameObject, panel.gameObject, new Rect(0, 15, 450, 50), Rect.zero);
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
