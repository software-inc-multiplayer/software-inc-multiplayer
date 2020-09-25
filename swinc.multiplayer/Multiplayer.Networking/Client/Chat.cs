using Multiplayer.Debugging;
using Multiplayer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

namespace Multiplayer.Networking
{
    public static partial class Client
    {
        public static string LogFilesPath
        {
            get
            {
                string path = "";
                foreach (ModController.DLLMod mod in ModController.Instance.Mods)
                {
                    if (mod.Meta.Name != "Software Inc Multiplayer") continue;
                    path = Path.Combine(mod.ModPath, "Logs");
                }
                return path;
            }
        }
        public static Text ChatWindow { get; set; }
        public static List<string> ChatMessages { get; set; }
        public static List<string> ChatLogMessages { get; set; }
        private static void CreateChatLogFile()
        {
            string path = Path.Combine(LogFilesPath, $"chat-{DateTime.Now.ToString().MakeSafe()}.log");
            File.WriteAllText(path, string.Join("\n", ChatLogMessages));
        }       
        public static void Send(Helpers.TcpChat chatmsg)
        {
            if (string.IsNullOrEmpty((string)chatmsg.Data.GetValue("message")))
            {
                Logging.Warn("[Message] Your chat message can't be empty!");
                WindowManager.SpawnDialog("Your chat message can't be empty!", true, DialogWindow.DialogType.Warning);
                return;
            }
            Logging.Info($"[Message] {((Helpers.User)chatmsg.Data.GetValue("sender")).Username}: " + (string)chatmsg.Data.GetValue("message"));
            client.Send(chatmsg.Serialize());
            if ((Helpers.User)chatmsg.Data.GetValue("reciever") != null)
            {
                OnServerChatRecieved(new Helpers.TcpServerChat($"Sent private message to {((Helpers.User)chatmsg.Data.GetValue("reciever")).Username}: {(string)chatmsg.Data.GetValue("message")}", Helpers.TcpServerChatType.Info));
                return;
            }
            if (ChatMessages.Count == 18)
                ChatMessages.RemoveAt(0);
            ChatMessages.Add($"{((Helpers.User)chatmsg.Data.GetValue("sender")).Username}: {(string)chatmsg.Data.GetValue("message")}");
            ChatWindow.text = string.Join("\n", ChatMessages);
        }
        public static void OnServerChatRecieved(Helpers.TcpServerChat tcpServerChat)
        {
            if (ChatMessages.Count == 18)
                ChatMessages.RemoveAt(0);
            string color = "";
            switch ((Helpers.TcpServerChatType)tcpServerChat.Data.GetValue("type"))
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

            ChatMessages.Add($"<color={color}>{(string)tcpServerChat.Data.GetValue("message")}</color>");
            ChatLogMessages.Add($"[Server] [{DateTime.Now.ToString()}] [{Enum.GetName(typeof(Helpers.TcpServerChatType), (Helpers.TcpServerChatType)tcpServerChat.Data.GetValue("type"))}] {(string)tcpServerChat.Data.GetValue("message")}");
            ChatWindow.text = string.Join("\n", ChatMessages);
        }
        private static void OnChatReceived(Helpers.TcpChat chat)
        {
            Helpers.User sender = (Helpers.User)chat.Data.GetValue("sender");
            if (sender == null)
            {
                sender = new Helpers.User() { Username = "Unknown User" };
            }

            Logging.Info($"[Message] {sender.Username}: {(string)chat.Data.GetValue("message")}");
            if (ChatMessages.Count == 18)
                ChatMessages.RemoveAt(0);
            ChatMessages.Add($"{sender.Username}: {(string)chat.Data.GetValue("message")}");
            ChatLogMessages.Add($"[{sender.Username}] [{DateTime.Now.ToString()}] {(string)chat.Data.GetValue("message")}");
            ChatWindow.text = string.Join("\n", ChatMessages);
        }
    }
}
