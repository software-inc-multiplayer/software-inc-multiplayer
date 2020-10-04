using Multiplayer.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Multiplayer.Debugging
{
    public class Logging
    {
        public static List<string> LogLines = new List<string>();
        public enum LogType
        {
            Info = 0,
            Debug = 2,
            Warn = 3,
            Error = 4
        }
        public static void Start()
        {
            File.Create(Path.Combine(Application.dataPath, "Multiplayer", "latest.log"));
            File.Create(Path.Combine(Application.dataPath, "Multiplayer", "Logs", DateTime.Now.ToString("HH:mm:ss:ffff").MakeSafe() + "-logging.log"));
        }
        public static void OnDisable()
        {
            File.WriteAllLines(Path.Combine(Application.dataPath, "Multiplayer", "latest.log"), LogLines.ToArray());
            File.WriteAllLines(Path.Combine(Application.dataPath, "Multiplayer", "Logs", DateTime.Now.ToString("HH:mm:ss:ffff").MakeSafe() + "-logging.log"), LogLines.ToArray());
        }

        public class LogObject
        {
            public LogType LogType { get; set; }
            public object Object { get; set; }
        }

        public static void Log(LogType logType, params object[] obj)
        {
            foreach (object e in obj)
            {
                switch (logType)
                {
                    case LogType.Debug:
                        DevConsole.Console.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                        break;
                    case LogType.Info:
                        DevConsole.Console.LogInfo(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                        break;
                    case LogType.Error:
                        DevConsole.Console.LogError(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                        break;
                    case LogType.Warn:
                        DevConsole.Console.LogWarning(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                        break;
                }
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                LogLines.Add(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [{logType}] {e}");
            }
        }
        public static void Log(params LogObject[] obj)
        {
            foreach (LogObject f in obj)
            {
                switch (f.LogType)
                {
                    case LogType.Debug:
                        DevConsole.Console.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {f.Object}");
                        break;
                    case LogType.Info:
                        DevConsole.Console.LogInfo(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {f.Object}");
                        break;
                    case LogType.Error:
                        DevConsole.Console.LogError(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {f.Object}");
                        break;
                    case LogType.Warn:
                        DevConsole.Console.LogWarning(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {f.Object}");
                        break;
                }
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {f.Object}");
                LogLines.Add(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [{f.LogType}] {f.Object}");
            }

        }
        public static void Debug(params object[] obj)
        {
            foreach (object e in obj)
            {
                DevConsole.Console.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                LogLines.Add(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Debug] {e}");
            }
        }
        public static void Info(params object[] obj)
        {
            foreach (object e in obj)
            {
                DevConsole.Console.LogInfo(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                LogLines.Add(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Info] {e}");
            }
        }
        public static void Warn(params object[] obj)
        {
            foreach (object e in obj)
            {
                DevConsole.Console.LogWarning(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                LogLines.Add(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Warn] {e}");
            }
        }
        public static void Error(params object[] obj)
        {
            foreach (object e in obj)
            {
                DevConsole.Console.LogError(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                LogLines.Add(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Error] {e}");
            }
        }
    }
}
