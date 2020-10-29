using System;
using Multiplayer.Networking.Shared;
using Packets;

namespace Multiplayer.Networking.Server.Handlers
{
    public class BanHandler : ServerPacketHandler
    {
        public override int Priority => 100;
        public override Type[] PacketsFilter => new Type[] { typeof(Handshake) };

        private readonly UserManager userManager;

        public BanHandler(GameServer server, UserManager userManager) : base(server)
        {
            this.userManager = userManager;
        }

        public override void HandlePacket(GameUser sender, IPacket packet)
        {
            // TODO implement this
        }
    }
}
