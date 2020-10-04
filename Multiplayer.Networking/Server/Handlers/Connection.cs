using Multiplayer.Debugging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telepathy;

namespace Multiplayer.Networking
{
    public partial class ServerClass
    {
        public void SendLoginRequest(int ConnectionID)
        {

        }
        public void OnClientConnect(Message data)
        {
            Logging.Info("[Server] New Client Connected", $"[Server] Client's ID - {data.connectionId}");
            
        }
    }
}
