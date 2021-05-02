using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using Multiplayer.Extensions;
using Multiplayer.Shared;
using System.Text;

namespace Multiplayer.Debugging
{
    public class UnityLogger : Shared.ILogger
    {
        public static Queue<string> messageQueue = new Queue<string>();
        private static readonly object externalLogLock = new object();

        // TODO maybe we can trigger this from client main
        public static void Start()
        {
            lock (externalLogLock)
            {
                //File.Create(Path.Combine(Application.dataPath, "Multiplayer", "latest.log"));
                //File.Create(Path.Combine(Application.dataPath, "Multiplayer", DateTime.Now.ToString("HH:mm:ss:ffff").MakeSafe() + "-logging.log"));
            }
        }
        
        // TODO maybe we can trigger this from client main
        public static void OnDisable()
        {
            lock (externalLogLock)
            {
                //File.WriteAllLines(Path.Combine(Application.dataPath, "Multiplayer", "latest.log"), messageQueue.ToArray());
                //File.WriteAllLines(Path.Combine(Application.dataPath, "Multiplayer", DateTime.Now.ToString("HH:mm:ss:ffff").MakeSafe() + "-logging.log"), messageQueue.ToArray());
            }
        }

        public UnityLogger()
        {
            Start();
        }

        public void LogInternal(Shared.LogType logType, string message)
        {
            switch (logType)
            {
                case Shared.LogType.Debug:
                    DevConsole.Console.Log(message);
                    break;
                case Shared.LogType.Info:
                    DevConsole.Console.LogInfo(message);
                    break;
                case Shared.LogType.Error:
                    DevConsole.Console.LogError(message);
                    break;
                case Shared.LogType.Warn:
                    DevConsole.Console.LogWarning(message);
                    break;
            }
            UnityEngine.Debug.Log(message);
            //log queue for file logging
            //messageQueue.Enqueue(message);
        }

        public void Log(Shared.LogType logType, params object[] objs)
        {
            var sb = new StringBuilder();

            switch (logType)
            {
                case Shared.LogType.Debug:
                    sb.Append($"[{DateTime.Now:HH:mm:ss:ffff}] [MP] [Debug] ");
                    break;
                case Shared.LogType.Info:
                    sb.Append($"[{DateTime.Now:HH:mm:ss:ffff}] [MP] [Info] ");
                    break;
                case Shared.LogType.Error:
                    sb.Append($"[{DateTime.Now:HH:mm:ss:ffff}] [MP] [Warn] ");
                    break;
                case Shared.LogType.Warn:
                    sb.Append($"[{DateTime.Now:HH:mm:ss:ffff}] [MP] [Error] ");
                    break;
            }

            foreach (object obj in objs)
            {
                sb.Append(obj);
                sb.Append(' ');
            }
            this.LogInternal(logType, sb.ToString());
        }
        
        public void Debug(params object[] objs)
        {
            this.Log(Shared.LogType.Debug, objs);
        }
        public void Info(params object[] objs)
        {
            this.Log(Shared.LogType.Info, objs);
        }
        public void Warn(params object[] objs)
        {
            this.Log(Shared.LogType.Warn, objs);
        }
        public void Error(params object[] objs)
        {
            this.Log(Shared.LogType.Error, objs);
        }
    }
}
