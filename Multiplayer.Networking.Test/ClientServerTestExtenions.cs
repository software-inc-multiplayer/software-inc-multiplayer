using System;
using System.Diagnostics;
using System.Threading;
using Multiplayer.Networking.Client;
using Multiplayer.Networking.Server;

namespace Multiplayer.Networking.Test
{
    internal static class ClientServerTestExtenions
    {
        internal static int timeoutMillis = 1000;

        [DebuggerStepThrough]
        public static void SafeTimeout(Func<bool> func)
        {
            var start = DateTime.Now;
            var handled = false;

            while (!handled && DateTime.Now.Subtract(start).TotalMilliseconds < timeoutMillis)
            {
                handled = func();
                if (!handled)
                    Thread.Yield();
            }
        }

        [DebuggerStepThrough]
        public static void SafeHandleMessages(this GameClient client)
        {
            SafeTimeout(() => client.HandleMessages());
        }

        [DebuggerStepThrough]
        public static void SafeHandleMessages(this GameServer server)
        {
            SafeTimeout(() => server.HandleMessages());
        }
    }
}
