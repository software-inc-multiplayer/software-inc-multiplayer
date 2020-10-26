using System;
using Multiplayer.Networking.Shared;
using Packets;

namespace Multiplayer.Networking.Server.Handlers
{
    public class ChatHandler : PacketHandler
    {
        public override Type[] PacketsFilter => new Type[] { typeof(ChatMessage), typeof(PrivateChatMessage) };

        public ChatHandler(GameServer server) : base(server) { }

        public override void HandlePacket(GameUser sender, IPacket packet)
        {
            if (packet is ChatMessage chatMessage)
            {
                this.server.Broadcast(chatMessage);
            }
            else if (packet is PrivateChatMessage privateChatMessage)
            {
                var receiverUser = this.server.UserManager.GetUser(privateChatMessage.Receiver);
                if (receiverUser == null)
                {
                    this.server.Send(sender, new PrivateChatMessage(null, sender.Id, "Invalid target player for pm"));
                    return;
                }

                this.server.Send(receiverUser, privateChatMessage);
            }
        }
    }
}
