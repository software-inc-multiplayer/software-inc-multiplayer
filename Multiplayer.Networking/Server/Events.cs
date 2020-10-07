using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Networking
{
    public static partial class Server
    {
        public static event EventHandler OnSavingServer;
        public static event EventHandler OnServerStart;
        public static event EventHandler OnServerStop;
        public static event EventHandler<Helpers.User> OnUserJoin;
        public static event EventHandler<Helpers.User> OnUserLeave;
    }
}
