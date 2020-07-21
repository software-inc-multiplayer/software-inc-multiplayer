using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Debugging
{
    public class Logging
    {
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
                        DevConsole.Console.Log(e);
                        break;
                    case LogType.Info:
                        DevConsole.Console.LogInfo(e);
                        break;
                    case LogType.Error:
                        DevConsole.Console.LogError(e);
                        break;
                    case LogType.Warn:
                        DevConsole.Console.LogWarning(e);
                        break;
                }
            }
        }
        public static void Log(params LogObject[] obj)
        {
            foreach (LogObject f in obj)
            {
                switch (f.LogType)
                {
                    case LogType.Debug:
                        DevConsole.Console.Log(f.Object);
                        break;
                    case LogType.Info:
                        DevConsole.Console.LogInfo(f.Object);
                        break;
                    case LogType.Error:
                        DevConsole.Console.LogError(f.Object);
                        break;
                    case LogType.Warn:
                        DevConsole.Console.LogWarning(f.Object);
                        break;
                }
            }
        }
        public static void Debug(params object[] obj)
        {
            foreach(object e in obj)
            {
                DevConsole.Console.Log(e);
            }
        }
        public static void Info(params object[] obj)
        {
            foreach(object e in obj)
            {
                DevConsole.Console.LogInfo(e);
            }
        }
        public static void Warn(params object[] obj)
        {
            foreach(object e in obj)
            {
                DevConsole.Console.LogWarning(e);
            }
        }
        public static void Error(params object[] obj)
        {
            foreach(object e in obj)
            {
                DevConsole.Console.LogError(e);
            }
        }
    }
}
