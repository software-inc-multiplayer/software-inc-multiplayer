using Google.Protobuf;
using Multiplayer.Networking.Shared;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Server
{
    public abstract class ServerPacketHandler<T> : IPacketHandler where T : IMessage
    {
        public virtual int Priority => 0;
        public void HandlePacket(GameUser sender, GamePacket packet)
        {
            var fields = packet.GetType().GetFields();
            foreach (var field in fields)
            {
                if (field.FieldType == typeof(T))
                {
                    HandlePacket(sender, (T)field.GetValue(packet));
                }
            }
        }

        public abstract void HandlePacket(GameUser sender, T packet);

        protected readonly GameServer server;

        public ServerPacketHandler(GameServer server)
        {
            this.server = server;
        }
    }
}
