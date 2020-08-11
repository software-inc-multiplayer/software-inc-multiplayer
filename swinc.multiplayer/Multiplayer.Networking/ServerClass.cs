using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Multiplayer.Debugging;
using WatsonTcp;
using System.Configuration;

namespace Multiplayer.Networking
{

    public class ServerClass : IDisposable
    {
        public static ServerClass Instance;
        private ushort Port = 52512;
        public ushort MaxPlayers = 10;
        private string Password = "";
        WatsonTcpServer server;
        private bool disposedValue;
        public List<Helpers.User> clients = new List<Helpers.User>();

        public ServerClass()
        {
            Instance = this;
        }

        public void Start(ushort port = 52512, string password = "", ushort maxplayers = 10)
        {
            Password = password;
            MaxPlayers = maxplayers;
            Port = port;
            server = new WatsonTcpServer(null, Port);
            server.ClientConnected += ClientConnected;
            server.ClientDisconnected += ClientDisconnected;
            server.MessageReceived += MessageReceived;
            server.SyncRequestReceived = SyncRequestReceived;
            server.Start();
            Logging.Info($"[Server] Server started.");
        }
        public void Stop()
        {
            server.Dispose();
            Logging.Info($"[Server] Server disposed.");
        }

        public ushort GetUserID(string username)
        {
            ushort uid = ushort.MaxValue;
            Helpers.User u = clients.Find(x => x.Username == username);
            if (u != null)
                uid = u.ID;
            return uid;
        }

        public string GetUsername(string ipport)
        {
            Helpers.User u = clients.Find(x => x.IpPort == ipport);
            return u.Username;
        }

        public Helpers.User GetUser(string username)
        {
            return clients.Find(x => x.Username == username);
        }

        void ClientConnected(object sender, ClientConnectedEventArgs args)
        {
            Logging.Info("[Server] Client connected: " + args.IpPort + "\nWaiting for client login...");
        }

        void ClientDisconnected(object sender, ClientDisconnectedEventArgs args)
        {
            Logging.Info("[Server] Client disconnected: " + args.IpPort + ": " + args.Reason.ToString());
            Helpers.User usr = clients.Find(x => x.IpPort == args.IpPort);
            if (usr == null)
            {
                Logging.Warn("[Server] ServerClass.ClientDisconnected: usr is null!");
                return;
            }
            if (usr.Role == Helpers.UserRole.Host)
            {
                //TODO: Do something if the host left the game
                Logging.Warn("[Server] Host did leave the server!");
            }
            clients.Remove(usr);
        }

        void MessageReceived(object sender, MessageReceivedFromClientEventArgs args)
        {
            Logging.Info("[Server] Message received from " + args.IpPort + ": " + Encoding.UTF8.GetString(args.Data));
            string datastr = Encoding.UTF8.GetString(args.Data);
            if (datastr == "login")
                LoginMessageReceived(args);
            else if (datastr == "chat")
                ChatMessageReceived(args);
        }

        async void ChatMessageReceived(MessageReceivedFromClientEventArgs args)
        {
            Helpers.User receiver = GetUser((string)args.Metadata["receiver"]);
            Helpers.User sender = clients.Find(x => x.IpPort == args.IpPort);
            string message = (string)args.Metadata["message"];

            if (receiver == null && (string)args.Metadata["receiver"] != "")
                Logging.Warn("[Server] Couldn't find user " + args.Metadata["receiver"] + " on the server and therefor couldn't send the chat message!");

            Helpers.ChatMessage cm = new Helpers.ChatMessage(sender.Username, message);

            if ((string)args.Metadata["receiver"] == "")
            {
                foreach (Helpers.User u in clients)
                    await server.SendAsync(u.IpPort, cm.Meta, cm.Data);

                Logging.Info($"[Server] Sent chat message from '{sender.Username}' to all clients on the server");
            }
            else
            {
                await server.SendAsync(receiver.IpPort, cm.Meta, cm.Data);
                Logging.Info($"[Server] Sent chat message from '{sender.Username}' to '{receiver.Username}'");
            }
        }

        void LoginMessageReceived(MessageReceivedFromClientEventArgs args)
        {
            string un = (string)args.Metadata["username"];
            string pw = (string)args.Metadata["password"];
            if (un == "")
                un = "Player_" + (clients.Count + 1);
            Logging.Info($"[Server] Login request from {args.IpPort} with username '{un}' and password '{pw}'");

            if (clients.Count >= MaxPlayers)
            {
                Logging.Info($"[Server] Max player count reached, will disconnect client!");
                Helpers.ServerMessage sm = new Helpers.ServerMessage(args.IpPort, "max_players");
                server.Send(args.IpPort, sm.Meta, "login_response");
                server.DisconnectClient(args.IpPort);
                return;
            }

            if (pw != Password)
            {
                Logging.Info($"[Server] Wrong password used '{pw}'");
                Helpers.ServerMessage sm = new Helpers.ServerMessage(args.IpPort, "wrong_password");
                server.Send(args.IpPort, sm.Meta, "login_response");
                server.DisconnectClient(args.IpPort);
                return;
            }
            Helpers.User newusr = new Helpers.User();
            newusr.IpPort = args.IpPort;
            if (clients.Count < 1)
                newusr.Role = Helpers.UserRole.Host;
            else
                newusr.Role = Helpers.UserRole.Client;
            newusr.Usercompany = null;
            newusr.ID = (ushort)(clients.Count + 1);
            newusr.Username = un;
            clients.Add(newusr);
            Logging.Info("[Server] Debug: " + JsonConvert.SerializeObject(newusr));
            Helpers.ServerMessage loginmsg = new Helpers.ServerMessage(args.IpPort, "ok");
            server.Send(args.IpPort, loginmsg.Meta, "login_response");

        }

        SyncResponse SyncRequestReceived(SyncRequest req)
        {
            return new SyncResponse(req, "Hello back at you!");
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    server.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
