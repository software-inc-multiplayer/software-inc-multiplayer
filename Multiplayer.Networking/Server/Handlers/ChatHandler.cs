using System.Numerics;
using Facepunch.Steamworks.Data;
using Multiplayer.Networking.Shared.Managers;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Server.Handlers
{
    [RegisterManager(RegisterType.Server, GamePacket.PacketOneofCase.ChatMessage)]
    public class ChatHandler : ServerPacketHandler<ChatMessage>
    {
        public ChatHandler(GameServer server) : base(server) { }

        public override void HandlePacket(Connection sender, ChatMessage packet)
        {
            if (BigInteger.Compare(packet.Target, BigInteger.Zero) < 0)
            {
                // For everyone.
                server.SocketManager.SendAll(packet);
            }
            else
            {
                // For specific user.
                //TODO: send to specific user
                // server.SocketManager.Send();
            }
        }
    }
}
