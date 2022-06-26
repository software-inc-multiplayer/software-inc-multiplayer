using Facepunch.Steamworks.Data;
using Google.Protobuf;
using Multiplayer.Networking.Shared;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Server
{
    public abstract class ServerPacketHandler<T> : IPacketHandler where T : IMessage
    {
        public virtual int Priority => 0;
        public void HandlePacket(IPacketHandler.PacketSender sender, GamePacket packet)
        {
            var fields = packet.GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(T))
                {
                    HandlePacket(sender.Connection.Value, (T)field.GetValue(packet));
                }
            }
        }

        public abstract void HandlePacket(Connection sender, T packet);

        protected readonly GameServer server;

        public ServerPacketHandler(GameServer server)
        {
            this.server = server;
        }
    }
}
