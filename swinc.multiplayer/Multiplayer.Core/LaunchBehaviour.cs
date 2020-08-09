using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer.Networking;
using Multiplayer.Debugging;
using System.Threading.Tasks;

namespace Multiplayer.Core
{
    public class LaunchBehaviour : ModBehaviour
    {
        public override void OnActivate()
        {
            ServerClass.Start();
        }

        public override void OnDeactivate()
        {
            ServerClass.Stop();
        }
    }
}
