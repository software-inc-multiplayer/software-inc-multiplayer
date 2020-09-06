using Multiplayer.Debugging;
using Multiplayer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Multiplayer.Networking
{
    public static partial class Client
    {
        public static string LogFilesPath { get
            {
                string path = "";
                foreach(ModController.DLLMod mod in ModController.Instance.Mods)
                {
                    if (mod.Meta.Name != "Software Inc Multiplayer") continue;
                    path = Path.Combine(mod.ModPath, "Logs");
                }
                return path;
            } }
        public static Text chatWindow { get; set; }
        public static List<string> chatMessages { get; set; }
        public static List<string> chatLogMessages { get; set; }
        private static void CreateChatLogFile()
        {
            string path = Path.Combine(LogFilesPath, $"chat-{DateTime.Now.ToString().MakeSafe()}.log");
            File.WriteAllText(path, string.Join("\n", chatLogMessages));
        }
        private static void OnServerChatRecieved(Helpers.TcpServerChat tcpServerChat)
        {
            if (chatMessages.Count == 18)
                chatMessages.RemoveAt(0);
            string color = "";
            switch((Helpers.TcpServerChatType) tcpServerChat.Data.GetValue("type"))
            {
                case Helpers.TcpServerChatType.Error:
                    color = "red";
                    break;
                case Helpers.TcpServerChatType.Info:
                    color = "blue";
                    break;
                case Helpers.TcpServerChatType.Warn:
                    color = "orange";
                    break;
            }
            chatMessages.Add($"<color={color}>{(string)tcpServerChat.Data.GetValue("message")}</color>");
            chatLogMessages.Add($"[Server] [{DateTime.Now.ToString()}] [{Enum.GetName(typeof(Helpers.TcpServerChatType), (Helpers.TcpServerChatType)tcpServerChat.Data.GetValue("type"))}] {(string)tcpServerChat.Data.GetValue("message")}");
            chatWindow.text = string.Join("\n", chatMessages);
        }
        static void OnChatReceived(Helpers.TcpChat chat)
        {
            Helpers.User sender = (Helpers.User)chat.Data.GetValue("sender");
            if (sender == null)
                sender = new Helpers.User() { Username = "Server" };
            Logging.Info($"[Message] {sender.Username}: {(string)chat.Data.GetValue("message")}");
            if (chatMessages.Count == 18)
                chatMessages.RemoveAt(0);
            chatMessages.Add($"{sender.Username}: {(string)chat.Data.GetValue("message")}");
            chatLogMessages.Add($"[{sender.Username}] [{DateTime.Now.ToString()}] {(string)chat.Data.GetValue("message")}");
            chatWindow.text = string.Join("\n", chatMessages);
        }
    }
}
