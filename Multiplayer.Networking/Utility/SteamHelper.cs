using Facepunch.Steamworks;

namespace Multiplayer.Networking.Utility
{
    public static class SteamHelper
    {
        public static bool Initialized { get; private set; } = false;
        private const int Appid = 362620;

        public static void Enable()
        {
            if (Initialized)
                return;
            SteamClient.Init(Appid);
            Initialized = true;
        }

        public static void Disable()
        {
            if (!Initialized)
                return;
            SteamClient.Shutdown();

            Initialized = false;
        }

        public static void Update()
        {
            SteamClient.RunCallbacks();
        }
    }
}
