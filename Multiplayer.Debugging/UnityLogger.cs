using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Multiplayer.Extensions;
using Multiplayer.Shared;

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
            messageQueue.Enqueue(message);
        }
        public void Log(Shared.LogType logType, params object[] objs)
        {
            foreach (object obj in objs)
            {
                string message = "";
                switch (logType)
                {
                    case Shared.LogType.Debug:
                        message = $"[{DateTime.Now:HH:mm:ss:ffff}] [Multiplayer] [Debug] {obj}";
                        break;
                    case Shared.LogType.Info:
                        message = $"[{DateTime.Now:HH:mm:ss:ffff}] [Multiplayer] [Info] {obj}";
                        break;
                    case Shared.LogType.Error:
                        message = $"[{DateTime.Now:HH:mm:ss:ffff}] [Multiplayer] [Warn] {obj}";
                        break;
                    case Shared.LogType.Warn:
                        message = $"[{DateTime.Now:HH:mm:ss:ffff}] [Multiplayer] [Error] {obj}";
                        break;
                }
                this.LogInternal(logType, message);
            }
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
