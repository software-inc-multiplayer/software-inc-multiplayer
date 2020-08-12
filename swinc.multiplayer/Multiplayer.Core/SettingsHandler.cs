using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

namespace Multiplayer.Core
{
    public static class SettingsHandler
    {
        public static void Set(string key, object value)
        {
            PlayerPrefs.SetString(key, JsonConvert.SerializeObject(value));
        }
        public static bool Has(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        public static object Get(string key)
        {
            return JsonConvert.DeserializeObject(PlayerPrefs.GetString(key));
        }
    }
}
