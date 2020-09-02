using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Multiplayer.Extensions
{
    public static class StringExt
    {
        public static void CopyToClipboard(this string str)
        {
            GUIUtility.systemCopyBuffer = str;
        }
        public static string SetStringVariable(this string str, string varName, string toSet)
        {
            return str.Replace($"%{varName}%", toSet);           
        }
    }
}
