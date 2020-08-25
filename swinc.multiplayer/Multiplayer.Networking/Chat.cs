using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer.Debugging;
using System.Threading.Tasks;
using System.IO;

namespace Multiplayer.Networking
{
    public class ChatMessage
    {
        public ulong ID { get; }
        public string Message { get; }
        public Helpers.User Author { get; }
        public DateTime SentDate { get; }
        public ChatMessage(string message, Helpers.User author)
        {
            SentDate = DateTime.UtcNow;
            ID = (ulong)new Random().Next();
            Message = message;
            Author = author;
        }
    }
    public class Chat
    {
        public static Chat instance;
        public Dictionary<ulong, ChatMessage> Messages = new Dictionary<ulong, ChatMessage>();
        public static void RecieveMessage(Helpers.TcpChat raw)
        {
            try
            {
                Helpers.User author = (Helpers.User)raw.Data.GetValue("sender");
                if (author == null)
                    author = new Helpers.User() { Username = "Server" };
                ChatMessage msg = new ChatMessage((string)raw.Data.GetValue("message"), author);
                Logging.Info($"[Chat] {msg.Author.Username}: ${msg.Message}");
                Logging.Info(instance.Messages.Count);
                instance.Messages.Add(msg.ID, msg);
                Logging.Info(instance.Messages.Count);
            } catch(Exception e)
            {
                Logging.Warn("[Chat] Error parsing chat: " + e);
                return;
            }        
        }
        /// <summary>
        /// Usually called on Client Disconnect.
        /// </summary>
        public static void ClearHistory(bool forced = false)
        {
            try
            {
                if (forced)
                {
                    if (instance.Messages.Count == 0)
                    {
                        Logging.Warn($"[Chat] No messages to clear.");
                        return;
                    }
                    Logging.Warn($"[Chat] Clearing {instance.Messages.Count} chat messages.");
                    instance.Messages.Clear();
                    return;
                }
                if (instance.Messages.Count == 0)
                {
                    Logging.Warn($"[Chat] No messages needed to clear/log.");
                    return;
                }
                string logFolderPath = Path.Combine(ModController.ModFolder, "Multiplayer", "Logs");
                if (!Directory.Exists(logFolderPath)) Directory.CreateDirectory(logFolderPath);
                string arr = "";
                foreach (KeyValuePair<ulong, ChatMessage> keyValuePair in instance.Messages)
                {
                    ChatMessage msg = keyValuePair.Value;
                    arr += $"{msg.SentDate.ToShortDateString()} - {msg.Author.Username} said {msg.Message}\n";
                }
                File.WriteAllText(Path.Combine(logFolderPath, $"chat-{DateTime.Now.ToShortDateString()}.log"), arr);
                Logging.Info($"[Chat] Wrote chat logs to {Path.Combine(logFolderPath, $"chat-{DateTime.Now.ToShortDateString()}.log")}");
                Logging.Info($"[Chat] Cleared Chat History.");
                instance.Messages.Clear();
            } catch (Exception e)
            {
                Logging.Error("There was an error clearing chat:" + e.ToString());
            }            
        }
    }
}
