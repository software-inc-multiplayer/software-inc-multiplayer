using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;

namespace Multiplayer.Networking.Server
{
    public class GameServer
    {
        public GameServerSocket SocketManager { get; private set; }
        public void Start(ServerInfo serverInfo)
        {
            //this.ServerInfo = serverInfo;
            //this.BakeHandlers();
            this.SocketManager = SteamNetworkingSockets.CreateNormalSocket<GameServerSocket>(NetAddress.AnyIp(serverInfo.Port));
            
            //this.ServerStarted?.Invoke(this, null);
        }
    }
}
