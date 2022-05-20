using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Multiplayer.Networking.Shared;

namespace Multiplayer.Networking.Server
{
    public class GameServer : IDisposable
    {
        public GameServerSocket SocketManager { get; private set; }
        public ServerInfo ServerInfo { get; set; }
        public UserManager UserManager { get; private set; }
        public void Start(ServerInfo serverInfo)
        {
            this.ServerInfo = serverInfo;
            this.UserManager = new UserManager();
            // this.BakeHandlers();
            this.SocketManager = SteamNetworkingSockets.CreateNormalSocket<GameServerSocket>(NetAddress.AnyIp(serverInfo.Port));

            //this.ServerStarted?.Invoke(this, null);
        }

        

        public void Dispose()
        {
            SocketManager?.Dispose();
        }
    }
}
