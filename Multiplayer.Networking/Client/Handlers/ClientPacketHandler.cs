using Google.Protobuf;
using Multiplayer.Networking.Shared;
using Multiplayer.Packets;

namespace Multiplayer.Networking.Client
{
    public abstract class ClientPacketHandler<T> : IPacketHandler where T : IMessage
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

        protected readonly GameClient client;

        public ClientPacketHandler(GameClient client)
        {
            this.client = client;
        }
    }
}
