using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiplayer.Extensions
{
    public static class InternetTools
    {
        public static bool CheckInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (var stream = client.OpenRead("http://www.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
        public static string GetIP()
        {
            string externalIP;
            if (PlayerPrefs.HasKey("cachedIP"))
            {
                externalIP = (string)JsonUtility.FromJson<string>(PlayerPrefs.GetString("cachedIP"));
                return externalIP;
            }
            externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
            externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}")).Matches(externalIP)[0].ToString();
            PlayerPrefs.SetString("cachedIP", JsonUtility.ToJson(externalIP));
            return externalIP;
        }
    }
}
