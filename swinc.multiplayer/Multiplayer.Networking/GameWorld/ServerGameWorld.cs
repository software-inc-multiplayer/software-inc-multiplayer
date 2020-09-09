using Multiplayer.Debugging;

namespace Multiplayer.Networking
{
    public static partial class Server
    {
        public static void Send(int clientid, Helpers.TcpGameWorld changes)
        {
            Logging.Info("[Server] Sending GameWorldChanges to client " + clientid + " => " + (bool)changes.Data.GetValue("addition"));
            server.Send(clientid, changes.Serialize());
        }

        public static void Send(Helpers.TcpGameWorld changes)
        {
            Logging.Info("[Server] Sending GameWorldChanges to all clients");
            foreach (Helpers.User user in Users)
            {
                server.Send(user.ID, changes.Serialize());
            }
        }
        public static void OnRecieveGameWorld(Helpers.TcpGameWorld world)
        {
            GameWorld.Server.Instance.UpdateWorld((GameWorld.World)world.Data.GetValue("changes"), (bool)world.Data.GetValue("addition"));
            GameWorld.Server.Instance.UpdateLocalWorld((GameWorld.World)world.Data.GetValue("changes"), (bool)world.Data.GetValue("addition"));
        }
    }
}
