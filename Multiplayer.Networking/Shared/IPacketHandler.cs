using Multiplayer.Packets;

namespace Multiplayer.Networking.Shared
{
    public interface IPacketHandler
    {
        int Priority { get; }
        void HandlePacket(GameUser sender, GamePacket packet);
    }
}
