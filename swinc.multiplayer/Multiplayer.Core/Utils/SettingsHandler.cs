using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;

namespace Multiplayer.Core
{
    public static class SettingsHandler
    {
        public static void Set<T>(string key, T value)
        {
            XmlSerializer x = new XmlSerializer(value.GetType());
            StringWriter writer = new StringWriter();
            x.Serialize(writer, value);
            PlayerPrefs.SetString(key, writer.ToString());
        }
        public static bool Has(string key)
        {
            return PlayerPrefs.HasKey(key);
        }
        public static bool TryGet<T>(string key, Type type, out T result)
        {
            try
            {
                XmlSerializer x = new XmlSerializer(type);
                MemoryStream writer = new MemoryStream();
                writer.Write(Encoding.UTF8.GetBytes(PlayerPrefs.GetString(key)), 0, Encoding.UTF8.GetBytes(PlayerPrefs.GetString(key)).Length);
                result = (T)x.Deserialize(writer);
                return true;
            } catch
            {
                result = default;
                return false;
            }
              
        }
    }
}
