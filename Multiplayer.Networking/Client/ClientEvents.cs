using System;

namespace Multiplayer.Networking.Client
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public ClientConnectedEventArgs(int connectionId)
        {
            this.ConnectionId = connectionId;
        }
        public int ConnectionId { get; set; }
    }

    public class ClientDisconnectedEventArgs : EventArgs
    {
        public ClientDisconnectedEventArgs(int connectionId)
        {
            this.ConnectionId = connectionId;
        }
        public int ConnectionId { get; set; }
    }
}
