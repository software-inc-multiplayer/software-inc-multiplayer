using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Networking
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
