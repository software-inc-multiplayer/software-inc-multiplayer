//using Newtonsoft.Json;
using System.Net;
using System.Text.RegularExpressions;
using UnityEngine;
namespace Multiplayer.Core
{
    public static class IPUtils
    {
        public static string GetIP()
        {
            /*string externalIP;
            if (PlayerPrefs.HasKey("cachedIP"))
            {
                externalIP = (string)JsonConvert.DeserializeObject(PlayerPrefs.GetString("cachedIP"));
                return externalIP;
            }
            externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
            externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
            PlayerPrefs.SetString("cachedIP", JsonConvert.SerializeObject(externalIP));
            return externalIP;*/
            return "";
        }
    }
}

