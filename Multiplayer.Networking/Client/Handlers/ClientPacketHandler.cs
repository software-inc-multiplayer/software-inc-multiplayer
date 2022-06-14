using System;
using Multiplayer.Networking.Shared;

namespace Multiplayer.Networking.Client
{
    public abstract class ClientPacketHandler<T> : IPacketHandler<T>
    {
        public virtual int Priority => 0;
        public abstract Type[] PacketsFilter { get; }

        public abstract void HandlePacket(GameUser sender, T packet);

        protected readonly GameClient client;

        public ClientPacketHandler(GameClient client)
        {
            this.client = client;
        }
    }
}
