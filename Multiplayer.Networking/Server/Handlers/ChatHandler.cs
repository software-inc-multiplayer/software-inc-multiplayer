using System.Numerics;
using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Shared.Managers;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Server.Handlers
{
    [RegisterManager(RegisterType.Server, GamePacket.PacketOneofCase.ChatMessage)]
    public class ChatHandler : ServerPacketHandler<ChatMessage>
    {
        public ChatHandler(GameServer server) : base(server) { }

        public override void HandlePacket(GameUser sender, ChatMessage packet)
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
