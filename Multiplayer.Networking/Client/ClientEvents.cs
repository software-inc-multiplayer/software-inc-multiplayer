using System;
using Multiplayer.Networking.Shared;

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

    public class UserConnectedEventArgs : EventArgs
    {
        public UserConnectedEventArgs(GameUser user)
        {
            this.User = user;
        }
        public GameUser User { get; set; }
    }

    public class UserDisconnectedEventArgs : EventArgs
    {
        public UserDisconnectedEventArgs(GameUser user)
        {
            this.User = user;
        }
        public GameUser User { get; set; }
    }
}
