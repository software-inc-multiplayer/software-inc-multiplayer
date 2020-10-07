using System;

namespace Multiplayer.Debugging
{
    public class Logging
    {
        public static Logging instance;
        public enum LogType
        {
            Info = 0,
            Debug = 2,
            Warn = 3,
            Error = 4
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
            }
        }
        public static void Debug(params object[] obj)
        {
            foreach (object e in obj)
            {
                DevConsole.Console.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
            }
        }
        public static void Info(params object[] obj)
        {
            foreach (object e in obj)
            {
                DevConsole.Console.LogInfo(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
            }
        }
        public static void Warn(params object[] obj)
        {
            foreach (object e in obj)
            {
                DevConsole.Console.LogWarning(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
            }
        }
        public static void Error(params object[] obj)
        {
            foreach (object e in obj)
            {
                DevConsole.Console.LogError(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
                UnityEngine.Debug.Log(DateTime.Now.ToString("HH:mm:ss:ffff") + $" [Multiplayer] {e}");
            }
        }
    }
}
