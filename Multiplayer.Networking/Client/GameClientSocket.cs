using System;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;

namespace Multiplayer.Networking.Client
{
    public class GameClientSocket : ConnectionManager
    {
        public GameClientSocket()
        {

        }

        public override void OnConnected(ConnectionInfo info)
        {
            
        }

        public override void OnConnecting(ConnectionInfo info)
        {
        }

        //base implementation is good enaugh
        /*public override void OnConnectionChanged(ConnectionInfo info)
        {
            base.OnConnectionChanged(info);
        }*/

        public override void OnDisconnected(ConnectionInfo info)
        {
            
        }

        public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {
            //TODO deserialize message and enqueue it
            //TODO catch exceptions and store them somewhere else, as this gets executed on another thread
            
        }
    }
}
