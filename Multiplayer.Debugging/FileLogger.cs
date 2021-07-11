using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Multiplayer.Shared;

namespace Multiplayer.Debugging
{
    public class FileLogger : ILogger
    {
        private readonly string logFileName;
        private readonly string sourceFilePath;

        private static readonly ConcurrentDictionary<string, object> _logLocksByFileName =
            new ConcurrentDictionary<string, object>();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="logFileName"></param>
        /// <param name="memberName"></param>
        /// <param name="sourceFilePath"></param>
        public FileLogger(string logFileName = "MPLog.txt", [CallerFilePath] string sourceFilePath = "")
        {
            if (string.IsNullOrEmpty(logFileName))
            {
                throw new ArgumentNullException(nameof(logFileName));
            }

            if (!File.Exists(logFileName))
            {
                File.Create(logFileName).Close();
            }

            this.logFileName = logFileName;
            this.sourceFilePath = sourceFilePath;
            _logLocksByFileName.AddOrUpdate(logFileName, new object(), (s, o) => new object());

        }

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

        private void LogInternal(LogType logType, string message)
        {
            lock (_logLocksByFileName[logFileName])
            {
                File.AppendAllText(logFileName, message);
            }
        }

    }
}
