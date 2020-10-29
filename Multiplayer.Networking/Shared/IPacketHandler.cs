using System;
using Packets;

namespace Multiplayer.Networking.Shared
{
    public interface IPacketHandler
    {
        int Priority { get; }
        Type[] PacketsFilter { get; }
        void HandlePacket(GameUser sender, IPacket packet);
    }
}
