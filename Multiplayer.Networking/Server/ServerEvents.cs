using System;
using Multiplayer.Networking.Shared;
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

    public class UserConnectedEventArgs : EventArgs
    {
        public GameUser User { get; }
        public UserConnectedEventArgs(GameUser user)
        {
            this.User = user;
        }
    }

    public class UserDisconnectedEventArgs : EventArgs
    {
        public GameUser User { get; }
        public UserDisconnectedEventArgs(GameUser user)
        {
            this.User = user;
        }
    }

}
