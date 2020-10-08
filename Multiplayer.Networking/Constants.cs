using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Networking
{
    public static class Constants
    {
        public enum DisconnectReason
        {
            ServerStop,
            Leaving,
            Kicked,
            Banned,
            UnhandledPacket,
            InvalidHandshake
        }
    }
}
