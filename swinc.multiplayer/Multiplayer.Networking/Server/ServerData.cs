using System;
using System.Collections.Generic;
using System.IO;
using Multiplayer.Debugging;
using ProtoBuf;

namespace Multiplayer.Networking
{
    [ProtoContract]
    public class ServerData
    {
        [ProtoMember(1)]
        public string ServerID;
        [ProtoMember(2)]
        public List<Helpers.User> Clients = new List<Helpers.User>();
        [ProtoMember(3)]
        public string ServerName;
        [ProtoMember(4)]
        public string Password;
        [ProtoMember(5)]
        public ushort MaxPlayers;
        [ProtoMember(6)]
        public int Difficulty;
        [ProtoMember(7)]
        public GameWorld.Server Gameworld;
        [ProtoMember(8)]
        public GameTime Gametime;
        [ProtoMember(9)]
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
                ServerData data = Serializer.Deserialize<ServerData>(File.OpenRead(Path.Combine(serverpath, fname + ".server")));
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
            FileStream fileStream = new FileStream(fname, FileMode.OpenOrCreate);
            Serializer.Serialize(fileStream, this);
            fileStream.Close();
            Logging.Info($"[ServerHandler] Saving ServerData to '{floca}'");
        }
    }
}