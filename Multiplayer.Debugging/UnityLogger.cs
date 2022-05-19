using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;
//using Multiplayer.Extensions;
using Multiplayer.Shared;
using System.Text;
using LogType = Multiplayer.Shared.LogType;

namespace Multiplayer.Debugging
{
    public class UnityLogger : Shared.ILogger
    {
        private static readonly object externalLogLock = new object();
        private readonly string sourceFilePath;

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

        public UnityLogger([CallerFilePath] string sourceFilePath = "")
        {
            this.sourceFilePath = sourceFilePath;
            Start();
        }

        public void LogInternal(Shared.LogType logType, string message, Exception ex = null)
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


        #region ILogger Implementation
        public void Log(LogType logType, object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0)
        {
            var sb = new StringBuilder();
            sb.Append($"[{DateTime.Now:HH:mm:ss:ffff}] [MP] [{logType}] [{sourceFilePath}->{memberName}#{sourceLineNumber}] ");

            sb.Append(obj + Environment.NewLine);
            if (ex != null) sb.AppendLine(ex.ToString());
            LogInternal(logType, sb.ToString());
        }

        public void Debug(object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0) => Log(LogType.Debug, obj, ex, memberName, sourceLineNumber);

        public void Info(object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0) => Log(LogType.Info, obj, ex, memberName, sourceLineNumber);

        public void Warn(object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0) => Log(LogType.Warn, obj, ex, memberName, sourceLineNumber);

        public void Error(object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0) => Log(LogType.Error, obj, ex, memberName, sourceLineNumber);
        #endregion
    }
}
