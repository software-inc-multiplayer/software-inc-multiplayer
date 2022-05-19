using System;
using Multiplayer.Networking.Shared;
using Packets;

namespace Multiplayer.Networking.Client
{
    public abstract class ClientPacketHandler : IPacketHandler
    {
        public virtual int Priority => 0;
        public abstract Type[] PacketsFilter { get; }

        public abstract void HandlePacket(GameUser sender, IPacket packet);

        protected readonly GameClient_old client;

        public ClientPacketHandler(GameClient_old client)
        {
            this.client = client;
        }
    }
}
