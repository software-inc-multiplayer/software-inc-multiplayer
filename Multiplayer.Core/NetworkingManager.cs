using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Debugging;
using Multiplayer.Networking;
using UnityEngine;

namespace Multiplayer.Core
{
    public class NetworkingManager : ModBehaviour
    {
        public static UnityLogger logger = Meta.Logging;
        public static Networking.Server Server { get; set; }
        public static Client Client { get; set; }
        public override void OnActivate()
        {
            Server = new Networking.Server(logger, new Networking.Utility.PacketSerializer());
            Client = new Client(logger, new Networking.Utility.PacketSerializer());
        }

        public override void OnDeactivate()
        {

        }
    }
}
