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
            //new ServerClass().Start();
        }

        public override void OnDeactivate()
        {
            List<GameObject> copy = new List<GameObject>() { };
            foreach (GameObject e in ActiveObjects)
            {
                e.SetActive(false);
                copy.Add(e);
            }
            foreach(GameObject copye in copy)
            {
                ActiveObjects.Remove(copye);
            }
            //ServerClass.Instance.Stop();
        }
    }
}
