﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using Facepunch.Steamworks;
using Facepunch.Steamworks.Data;
using Google.Protobuf;
using Multiplayer.Debugging;
using Multiplayer.Networking.Utility;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Client
{
    public class GameClientSocket : ConnectionManager, IDisposable
    {
        private ILogger log;

        public GameClientSocket()
        {
            log = new UnityLogger();
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
            // TODO: Protobuf deserialization
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
