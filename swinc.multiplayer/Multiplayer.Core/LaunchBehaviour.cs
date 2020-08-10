using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Multiplayer.Networking;
using Multiplayer.Debugging;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiplayer.Core
{
    public class LaunchBehaviour : ModBehaviour
    {
        public static List<GameObject> ActiveObjects = new List<GameObject>() { };

        public override void OnActivate()
        {
            new ServerClass().Start();
        }

        public override void OnDeactivate()
        {
            foreach(GameObject e in ActiveObjects)
            {
                e.SetActive(false);
                ActiveObjects.Remove(e);
            }
            ServerClass.Instance.Stop();
        }
    }
}
