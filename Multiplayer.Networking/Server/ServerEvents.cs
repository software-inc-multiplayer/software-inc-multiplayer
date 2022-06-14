using System;
using Multiplayer.Networking.Shared;

namespace Multiplayer.Networking
{

    public class ClientConnectedEventArgs : EventArgs
    {
        public uint ConnectionId { get; }
        public ClientConnectedEventArgs(uint connectionId)
        {
            this.ConnectionId = connectionId;
        }
    }

    public class ClientDisconnectedEventArgs : EventArgs
    {
        public uint ConnectionId { get; }
        public ClientDisconnectedEventArgs(uint connectionId)
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
