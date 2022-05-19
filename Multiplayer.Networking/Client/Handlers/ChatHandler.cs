using System;
using Multiplayer.Networking.Shared;
using Packets;

namespace Multiplayer.Networking.Client.Handlers
{
    public class ChatHandler : ClientPacketHandler
    {
        public override Type[] PacketsFilter => new Type[] { typeof(ChatMessage), typeof(PrivateChatMessage) };

        public ChatHandler(GameClient_old client) : base(client) { }

        public override void HandlePacket(GameUser sender, IPacket packet)
        {
            if (packet is ChatMessage chatMessage)
            {
                // well ...
            }
            else if (packet is PrivateChatMessage privateChatMessage)
            {
                var receiverUser = this.client.UserManager.GetUser(privateChatMessage.Receiver);
                if (receiverUser == null)
                {
                    // -.- is it me or him? somebody is off here
                    return;
                }

                // well ... how is one going to display this message?
                // maybe we shall keep some sort of chat history on the client side?
            }
        }
    }
}
