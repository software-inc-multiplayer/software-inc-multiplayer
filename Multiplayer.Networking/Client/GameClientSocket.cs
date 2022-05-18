using System;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Multiplayer.Debugging;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Client
{
    public class GameClientSocket : ConnectionManager, IDisposable
    {
        private ILogger log;

        public GameClientSocket()
        {
            log = new FileLogger();
        }

        public override void OnConnected(ConnectionInfo info)
        {
            base.OnConnected(info);
        }

        public override void OnConnecting(ConnectionInfo info)
        {
            base.OnConnecting(info);
        }



        //base implementation is good enaugh
        /*public override void OnConnectionChanged(ConnectionInfo info)
        {
            base.OnConnectionChanged(info);
        }*/

        public override void OnDisconnected(ConnectionInfo info)
        {
            this.Close();
            base.OnDisconnected(info);
        }

        public override void OnMessage(IntPtr data, int size, long messageNum, long recvTime, int channel)
        {

            //TODO deserialize message and enqueue it
            //TODO catch exceptions and store them somewhere else, as this gets executed on another thread

        }

        public void Dispose()
        {
            try
            {
                if (this.Connected || Connecting)
                {
                    this.Connection.Close(true);
                    Close();
                }
            }
            catch (Exception ex)
            {
                this.log.Error(ex);
            }
        }
    }
}
