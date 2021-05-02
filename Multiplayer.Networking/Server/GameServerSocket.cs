using System;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;

namespace Multiplayer.Networking.Server
{
    public class GameServerSocket : SocketManager
    {
        public override void OnConnected(Connection connection, ConnectionInfo info)
        {
            
        }

        public override void OnConnecting(Connection connection, ConnectionInfo info)
        {
            var result = connection.Accept();
        }

        public override void OnDisconnected(Connection connection, ConnectionInfo info)
        {
            
        }

        public override void OnMessage(Connection connection, NetIdentity identity, IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            
        }
    }
}
