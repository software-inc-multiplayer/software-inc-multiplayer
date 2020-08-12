using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Multiplayer.Debugging;
using Multiplayer.Networking;
using Newtonsoft.Json;
using UnityEngine;

namespace Multiplayer.Networking
{
    [Serializable]
    public class ServerData
    {
        [NonSerialized]
        private ServerClass _server;
        public string ServerID;
        public List<Helpers.User> Clients = new List<Helpers.User>();
        public string ServerName;
        public string Password;
        public ushort MaxPlayers;
        public GameWorld.Server Gameworld;
        string serverpath;

        /// <summary>
        /// Creates a new ServerData based on the ServerClass.Instance
        /// </summary>
        /// <param name="fname">The filename to the saved Server, if empty it will load from the ServerClass.Instance</param>
        public ServerData(string fname = "")
        {
            serverpath = Path.Combine(ModController.ModFolder, "Multiplayer", "Servers"); //Use this for SINC
            //serverpath = Path.Combine(new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).DirectoryName, "Multiplayer", "Servers"); //Use this for Console
            Directory.CreateDirectory(serverpath); //Create path if not exists
            if (string.IsNullOrEmpty(fname) || !File.Exists(Path.Combine(serverpath, fname + ".json")))
            {
                Logging.Info("[ServerHandler] ServerData does not exist, will create new one");
                _server = ServerClass.Instance;
                Gameworld = new GameWorld.Server();
                ServerID = DateTime.Now.Ticks + "";
                UpdateData();
            }
            else
            {
                Logging.Info($"[ServerHandler] Trying to load ServerData from '{fname}'");
                ServerData data = JsonConvert.DeserializeObject<ServerData>(File.ReadAllText(Path.Combine(serverpath, fname + ".json")));
                ServerID = data.ServerID;
                Clients = data.Clients;
                ServerName = data.ServerName;
                Password = data.Password;
                MaxPlayers = data.MaxPlayers;
                Gameworld = data.Gameworld;
                UpdateServer();
            }
        }

        /// <summary>
        /// Updates the data on this ServerData instance from the Server Instance
        /// </summary>
        public void UpdateData()
        {
            Logging.Info("[ServerHandler] Updating ServerData from Server Instance");
            Clients = _server.clients;
            ServerName = _server.ServerName;
            Password = _server.Password;
            MaxPlayers = _server.MaxPlayers;
        }

        /// <summary>
        /// Updates the data on the Server Instance from this ServerData Instance
        /// </summary>
        public void UpdateServer()
        {
            Logging.Info("[ServerHandler] Updating Server Instance from ServerData");
            ServerClass server = new ServerClass();
            server.clients = Clients;
            server.ServerName = ServerName;
            server.Password = Password;
            server.MaxPlayers = MaxPlayers;
            _server = ServerClass.Instance;
        }

        /// <summary>
        /// Saves the ServerData to a File inside the "./Multiplayer/Servers/" directory
        /// </summary>
        public void SaveData()
        {
            string fname = ServerName;
            string floca = Path.Combine(serverpath, fname + ".json");
            File.WriteAllText(floca, JsonConvert.SerializeObject(this));
            Logging.Info($"[ServerHandler] Saving ServerData to '{floca}'");
        }
    }
}