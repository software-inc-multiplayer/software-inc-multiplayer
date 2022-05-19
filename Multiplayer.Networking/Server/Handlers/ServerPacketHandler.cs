using System;
using Multiplayer.Networking.Shared;
using Packets;

namespace Multiplayer.Networking.Server
{
    public abstract class ServerPacketHandler : IPacketHandler
    {
        public virtual int Priority => 0;
        public abstract Type[] PacketsFilter { get; }

        public abstract void HandlePacket(GameUser sender, IPacket packet);

        protected readonly GameServer_old server;

        public ServerPacketHandler(GameServer_old server)
        {
            this.server = server;
        }
    }
}
