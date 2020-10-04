using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Debugging;
using Multiplayer.Extensions;
using UnityEngine;

namespace Multiplayer.Settings
{
    [Serializable]
    public class Setting
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }
    }
    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager settingsManager { get; set; }
        public string SettingPath { get; set; }
        public List<Setting> Settings = new List<Setting>();
        public void Start()
        {
            SettingPath = Path.Combine(Application.dataPath, "Multiplayer", "settings.json");
            if(!File.Exists(SettingPath))
            {
                File.Create(SettingPath);
                return;
            }
            string json = File.ReadAllText(SettingPath);
            Settings = JsonUtility.FromJson<List<Setting>>(json);
        }
        public bool GetSetting<T>(string key, out T value)
        {
            try
            {
                Setting settemp = null;
                Settings.ForEach(setting =>
                {
                    if (setting.Key != key) return;
                    settemp = setting;
                });
                if (settemp == null)
                {
                    value = default;
                    return false;
                }

            } catch (Exception e)
            {
                Logging.Error(e);
                value = default;
                return false;
            }
        }
    }
}
