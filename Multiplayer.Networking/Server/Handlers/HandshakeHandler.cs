using Facepunch.Steamworks.Data;
using Multiplayer.Networking.Shared.Managers;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Server.Handlers
{
    [RegisterManager(RegisterType.Server, GamePacket.PacketOneofCase.Handshake)]
    public class HandshakeHandler : ServerPacketHandler<Handshake>
    {
        public HandshakeHandler(GameServer server) : base(server)
        { }

        public override void HandlePacket(Connection sender, Handshake packet)
        {
            
        }
    }
}