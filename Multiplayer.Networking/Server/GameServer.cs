using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Google.Protobuf.WellKnownTypes;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Server
{
    public class GameServer
    {
        public GameServerSocket SocketManager { get; private set; }

        protected Dictionary<ulong, Connection> steamIdToConnection = new Dictionary<ulong, Connection>();


        public void Start(ServerInfo serverInfo)
        {
            //this.ServerInfo = serverInfo;
            //this.BakeHandlers();
            this.SocketManager = SteamNetworkingSockets.CreateNormalSocket<GameServerSocket>(NetAddress.AnyIp(serverInfo.Port));
            this.SocketManager.Connected += this.SocketManager_Connected;
            this.SocketManager.MessageReceived += this.SocketManager_MessageReceived;

            SteamServer.OnValidateAuthTicketResponse += SteamServer_OnValidateAuthTicketResponse;
            //this.ServerStarted?.Invoke(this, null);
        }

        private void SocketManager_Connected(Connection connection, ConnectionInfo connectionInfo)
        {
            var identity = connectionInfo.Identity;
            var steamId = identity.SteamId;
            if (steamId.Value == 0 || !steamId.IsValid)
                return;

            this.steamIdToConnection.Add(steamId.Value, connection);
        }

        private void SocketManager_MessageReceived(Connection connection, Any anyMessage, string messageType)
        {
            //Handshake.Descriptor.FullName
            //anyMessage.TypeUrl
        }

        private void SteamServer_OnValidateAuthTicketResponse(SteamId steamId, SteamId ownerId, AuthResponse authResponse)
        {
            if (!steamId.IsValid)
                return; //ignore invalid steam ids

            if(authResponse != AuthResponse.OK)
            {
                //disconnect this guy ASAP!
            }
        }
    }
}
