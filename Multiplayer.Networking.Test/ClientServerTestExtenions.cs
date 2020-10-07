using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Multiplayer.Networking.Test
{
    internal static class ClientServerTestExtenions
    {
        internal static int timeoutMillis = 1000;

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

        public static void SafeHandleMessages(this Client client)
        {
            SafeTimeout(() => client.HandleMessages());
        }

        public static void SafeHandleMessages(this Server server)
        {
            SafeTimeout(() => server.HandleMessages());
        }
    }
}
