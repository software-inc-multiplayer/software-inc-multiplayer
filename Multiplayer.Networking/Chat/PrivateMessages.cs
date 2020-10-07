using Multiplayer.Debugging;
using Multiplayer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Multiplayer.Networking
{
    public partial class Server
    {
        public static void Send(int id, TcpPrivateChat chat)
        {
            server.Send(id, chat.Serialize());
        }
        private static void OnPrivateChat(TcpPrivateChat chat)
        {
            Helpers.User reciever = GetUser((string)chat.Data.GetValue("reciever"));
            Send(reciever.ID, chat);
        }
    }
    public static partial class Client
    {
        public static List<string> PrivateMessages { get; set; }
        public static void Send(TcpPrivateChat pm)
        {
            Logging.Info("Sent private message.");
            client.Send(pm.Serialize());
        }
        private static void CreatePChatLogFile()
        {
            string path = Path.Combine(LogFilesPath, $"private-chat-{DateTime.Now.ToString().MakeSafe()}.log");
            File.WriteAllText(path, string.Join("\n", ChatLogMessages));
        }
        private static void OnPrivateChatRecieved(TcpPrivateChat chat)
        {
            Helpers.User sender = (Helpers.User)chat.Data.GetValue("sender");
            string msg = (string)chat.Data.GetValue("message");
            PrivateMessages.Add($"[{sender.Username}] [{DateTime.Now}] {(string)chat.Data.GetValue("message")}");
            WindowManager.SpawnDialog($"{sender.Username} sent you a message:\n\n<b>{msg}</b>", true, DialogWindow.DialogType.Information);
        }
    }
}
