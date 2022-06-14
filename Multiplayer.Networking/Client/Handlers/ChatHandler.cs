using System;
using Multiplayer.Networking.Shared;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Client.Handlers
{
    public class ChatHandler : ClientPacketHandler<ChatMessage>
    {
        public override Type[] PacketsFilter => new Type[] { typeof(ChatMessage) };
        public ChatHandler(GameClient client) : base(client) { }
        public override void HandlePacket(GameUser sender, ChatMessage packet)
        {
            // TODO: Fix this
        }
    }
}
