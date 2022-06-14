using System;
using Google.Protobuf;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Shared
{
    public interface IPacketHandler
    {
        int Priority { get; }
        Type[] PacketsFilter { get; }
        void HandlePacket(GameUser sender, GamePacket packet);
    }
}
