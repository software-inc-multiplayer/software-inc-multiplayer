using System;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Multiplayer.Networking.Shared;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Server
{
    public class GameServer : IDisposable
    {

        #region Events

        // TODO: Expand these events with relevant infomation.
        public event EventHandler ServerStarted;
        public event EventHandler ServerStopped;
        public event EventHandler ClientConnected;
        public event EventHandler ClientDisconnected; 

        #endregion
        
        public GameServerSocket SocketManager { get; private set; }
        public ServerInfo ServerInfo { get; set; }
        public UserManager UserManager { get; private set; }
        public ILogger Logger { get; private set; }
        public void Start(ServerInfo serverInfo)
        {
            this.ServerInfo = serverInfo;
            this.UserManager = new UserManager();
            this.SocketManager = SteamNetworkingSockets.CreateNormalSocket<GameServerSocket>(NetAddress.AnyIp(serverInfo.Port));
            this.SocketManager.Logger = Logger;
            Logger.Info("Server Started!");
            ServerStarted.Invoke(this, EventArgs.Empty);
        }


        public GameServer(ILogger logger) => this.Logger = logger;
        


        public void Dispose()
        {
            SocketManager?.Dispose();
        }

        public void Stop()
        {
            this.SocketManager.Close();
            Logger.Info("Server Stopped!");
            ServerStopped.Invoke(this, EventArgs.Empty);
        }
    }
}
