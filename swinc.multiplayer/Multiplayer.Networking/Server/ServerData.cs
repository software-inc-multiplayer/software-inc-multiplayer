using System;
using System.Collections.Generic;
using System.IO;
using Multiplayer.Debugging;

namespace Multiplayer.Networking
{
    [Serializable]
    public class ServerData
    {
        public string ServerID;
        public List<Helpers.User> Clients = new List<Helpers.User>();
        public string ServerName;
        public string Password;
        public ushort MaxPlayers;
        public int Difficulty;
        public GameWorld.Server Gameworld;
        public GameTime Gametime;
        string serverpath;

        /// <summary>
        /// Creates a new ServerData based on the ServerClass.Instance
        /// </summary>
        /// <param name="fname">The filename to the saved Server, if empty it will load from the ServerClass.Instance</param>
        public ServerData(string fname = "")
        {
            Server.OnSavingServer += SaveData;
            serverpath = Path.Combine(ModController.ModFolder, "Multiplayer", "Servers");
            Directory.CreateDirectory(serverpath); //Create path if not exists
            if (string.IsNullOrEmpty(fname) || !File.Exists(Path.Combine(serverpath, fname + ".json")))
            {
                Logging.Info("[ServerHandler] ServerData does not exist, will create new one");
                Gameworld = new GameWorld.Server();
                Gametime = new GameTime(new SDateTime(1, 70), 0);
                ServerID = DateTime.Now.Ticks + "";
                UpdateData();
            }
            else
            {
                Logging.Info($"[ServerHandler] Trying to load ServerData from '{fname}'");
                ServerData data = Helpers.Deserialize<ServerData>(File.ReadAllBytes(Path.Combine(serverpath, fname + ".server")));
                data.Gametime.Speed = 0; //Pause the game at startup
                ServerID = data.ServerID;
                Clients = data.Clients;
                ServerName = data.ServerName;
                Password = data.Password;
                MaxPlayers = data.MaxPlayers;
                Gameworld = data.Gameworld;
                Gametime = data.Gametime;
                Difficulty = data.Difficulty;
                UpdateServer();
            }
        }

        /// <summary>
        /// Updates the data on this ServerData instance from the Server Instance
        /// </summary>
        public void UpdateData()
        {
            Logging.Info("[ServerHandler] Updating ServerData from Server Instance");
            Clients = Server.Users;
            ServerName = Server.ServerName;
            Password = Server.Password;
            MaxPlayers = Server.MaxPlayers;
            Difficulty = Server.Difficulty;
        }

        /// <summary>
        /// Updates the data on the Server Instance from this ServerData Instance
        /// </summary>
        public void UpdateServer()
        {
            Logging.Info("[ServerHandler] Updating Server Instance from ServerData");
            Server.Users = Clients;
            Server.ServerName = ServerName;
            Server.Password = Password;
            Server.MaxPlayers = MaxPlayers;
            Server.Difficulty = Difficulty;
        }

        /// <summary>
        /// Saves the ServerData to a File inside the "./Multiplayer/Servers/" directory
        /// </summary>
        public void SaveData(object sender, EventArgs args)
        {
            string fname = ServerName;
            string floca = Path.Combine(serverpath, fname + ".server");
            File.WriteAllBytes(floca, this.Serialize());
            Logging.Info($"[ServerHandler] Saving ServerData to '{floca}'");
        }
    }
}