using System;
using Packets;

namespace Multiplayer.Networking
{

    public class ClientConnectedEventArgs : EventArgs
    {
        public int ConnectionId { get; }
        public ClientConnectedEventArgs(int connectionId)
        {
            this.ConnectionId = connectionId;
        }
    }

    public class ClientDisconnectedEventArgs : EventArgs
    {
        public int ConnectionId { get; }
        public ClientDisconnectedEventArgs(int connectionId)
        {
            this.ConnectionId = connectionId;
        }
    }

}
