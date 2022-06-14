using System;
using Google.Protobuf;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Shared
{
    public interface IPacketHandler<T>
    {
        int Priority { get; }
        Type[] PacketsFilter { get; }
        void HandlePacket(GameUser sender, T packet);
    }
}
