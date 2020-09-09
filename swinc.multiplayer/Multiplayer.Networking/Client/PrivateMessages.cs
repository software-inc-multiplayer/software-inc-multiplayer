using Multiplayer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Networking
{
    public static partial class Client
    {
        public static List<string> PrivateMessages { get; set; }
        private static void CreatePChatLogFile()
        {
            string path = Path.Combine(LogFilesPath, $"private-chat-{DateTime.Now.ToString().MakeSafe()}.log");
            File.WriteAllText(path, string.Join("\n", ChatLogMessages));
        }
        private static void OnPrivateChatRecieved(Helpers.TcpChat chat)
        {
            Helpers.User sender = (Helpers.User)chat.Data.GetValue("sender");
            string msg = (string)chat.Data.GetValue("message");
            PrivateMessages.Add($"[{sender.Username}] [{DateTime.Now.ToString()}] {(string)chat.Data.GetValue("message")}");
            WindowManager.SpawnDialog($"{sender.Username} sent you a message:\n\n<b>{msg}</b>", true, DialogWindow.DialogType.Information);
        }
    }
}
