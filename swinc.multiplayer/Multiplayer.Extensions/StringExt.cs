using System.IO;
using System.Linq;
using UnityEngine;

namespace Multiplayer.Extensions
{
    public static class StringExt
    {
        private static readonly char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
        public static string MakeSafe(this string str)
        {
            return new string(str.Select(ch => invalidFileNameChars.Contains(ch) ? '_' : ch).ToArray());
        }
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
