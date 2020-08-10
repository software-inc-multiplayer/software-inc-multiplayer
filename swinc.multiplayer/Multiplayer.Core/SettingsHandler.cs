using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Debugging;
using Newtonsoft.Json;

namespace Multiplayer.Core
{
    public static class SettingsHandler
    {
        [Serializable]
        private class Setting<T>
        {
            public string SettingType { get; set; }
            public string StrKey { get; set; }
            public T ObjValue { get; set; }
            public Setting(string key, T value)
            {
                SettingType = value.GetType().Name;
                StrKey = key;
                ObjValue = value;
            }
        }
        public static string GetSettingsFilePath(string key)
        {
            string a = GetSettingsFolderPath();
            return Path.Combine(a, $"{key}.json");
        }
        public static string GetSettingsFolderPath()
        {
            return Path.Combine(ModController.ModFolder, "Multiplayer", "Settings");
        }
        public static void SaveSetting<T>(string key, T value, bool overwrite = true)
        {
            string settingsFilePath = GetSettingsFilePath(key);
            if(File.Exists(settingsFilePath))
            {
                if(!overwrite)
                {
                    Logging.Warn($"The setting '{key}' already exists but you specified not to overwrite it.");
                    return;
                }
                Logging.Warn($"The setting '{key}' already exists, overwriting...");
                File.Delete(settingsFilePath);
            }
            Setting<T> setting = new Setting<T>(key, value);
            string json = JsonConvert.SerializeObject(setting);
            File.WriteAllText(settingsFilePath, json, Encoding.UTF8);
        }
        public static bool TryLoadSetting<T>(string key, out T value)
        {
            string settingsFilePath = GetSettingsFilePath(key);
            try
            {
                string FileContent = File.ReadAllText(settingsFilePath, Encoding.UTF8);
                value = JsonConvert.DeserializeObject<T>(FileContent);
            } catch(Exception)
            {
                value = default;
                return false;
            }
            return true;
        }
    }
}
