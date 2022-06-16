using Multiplayer.Networking.Shared;
using Multiplayer.Networking.Shared.Managers;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Client.Handlers
{
    [RegisterManager(RegisterType.Client, GamePacket.PacketOneofCase.ChatMessage)]
    public class ChatHandler : ClientPacketHandler<ChatMessage>
    {
        public ChatHandler(GameClient client) : base(client) { }
        public override void HandlePacket(GameUser sender, ChatMessage packet)
        {
            // TODO: Fix this
        }
    }
}
