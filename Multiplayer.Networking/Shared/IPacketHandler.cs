using Facepunch.Steamworks.Data;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Shared
{
    
    
    public interface IPacketHandler
    {
        
        public class PacketSender
        {
            public Connection? Connection;
            public GameUser? User;
        }
        
        int Priority { get; }
        void HandlePacket(PacketSender sender, GamePacket packet);
    }
}
