using System;
using Google.Protobuf;
using Multiplayer.Networking.Shared;

namespace Multiplayer.Networking.Server
{
    public abstract class ServerPacketHandler<T> : IPacketHandler<T>
    {
        public virtual int Priority => 0;
        public abstract Type[] PacketsFilter { get; }

        public abstract void HandlePacket(GameUser sender, T packet);

        protected readonly GameServer server;

        public ServerPacketHandler(GameServer server)
        {
            this.server = server;
        }
    }
}
