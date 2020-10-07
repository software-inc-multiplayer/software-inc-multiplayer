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
                    if (mod.Meta.Name != "Software Inc Multiplayer")
                    {
                        continue;
                    }

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
        public static void Send(TcpChat chatmsg)
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
                OnServerChatRecieved(new TcpServerChat($"Sent private message to {((Helpers.User)chatmsg.Data.GetValue("reciever")).Username}: {(string)chatmsg.Data.GetValue("message")}", TcpServerChatType.Info));
                return;
            }
            if (ChatMessages.Count == 18)
            {
                ChatMessages.RemoveAt(0);
            }

            ChatMessages.Add($"{((Helpers.User)chatmsg.Data.GetValue("sender")).Username}: {(string)chatmsg.Data.GetValue("message")}");
            ChatWindow.text = string.Join("\n", ChatMessages);
        }
        public static void OnServerChatRecieved(TcpServerChat tcpServerChat)
        {
            if (ChatMessages.Count == 18)
            {
                ChatMessages.RemoveAt(0);
            }

            string color = "";
            switch ((TcpServerChatType)tcpServerChat.Data.GetValue("type"))
            {
                case TcpServerChatType.Error:
                    color = "red";
                    break;
                case TcpServerChatType.Info:
                    color = "blue";
                    break;
                case TcpServerChatType.Warn:
                    color = "orange";
                    break;
            }

            ChatMessages.Add($"<color={color}>{(string)tcpServerChat.Data.GetValue("message")}</color>");
            ChatLogMessages.Add($"[Server] [{DateTime.Now}] [{Enum.GetName(typeof(TcpServerChatType), (TcpServerChatType)tcpServerChat.Data.GetValue("type"))}] {(string)tcpServerChat.Data.GetValue("message")}");
            ChatWindow.text = string.Join("\n", ChatMessages);
        }
        private static void OnChatReceived(TcpChat chat)
        {
            Helpers.User sender = (Helpers.User)chat.Data.GetValue("sender");
            if (sender == null)
            {
                sender = new Helpers.User() { Username = "Unknown User" };
            }

            Logging.Info($"[Message] {sender.Username}: {(string)chat.Data.GetValue("message")}");
            if (ChatMessages.Count == 18)
            {
                ChatMessages.RemoveAt(0);
            }

            ChatMessages.Add($"{sender.Username}: {(string)chat.Data.GetValue("message")}");
            ChatLogMessages.Add($"[{sender.Username}] [{DateTime.Now}] {(string)chat.Data.GetValue("message")}");
            ChatWindow.text = string.Join("\n", ChatMessages);
        }
    }
}
