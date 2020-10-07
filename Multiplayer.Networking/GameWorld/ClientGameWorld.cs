using Multiplayer.Debugging;

namespace Multiplayer.Networking
{
    public static partial class Client
    {
        private static void OnGameWorldReceived(TcpGameWorld world)
        {
            GameWorld.World changes = (GameWorld.World)world.Data.GetValue("changes");
            bool addition = (bool)world.Data.GetValue("addition");
            Logging.Info($"[Client] Updating GameWorld => " + addition);
            GameWorld.Client.Instance.UpdateLocalWorld(changes, addition);
            GameWorld.Client.Instance.world.RefreshData();
        }
        public static void Send(TcpGameWorld changes)
        {
            Logging.Info("[Client] Sending gameworld update");
            client.Send(changes.Serialize());
        }
    }
}
