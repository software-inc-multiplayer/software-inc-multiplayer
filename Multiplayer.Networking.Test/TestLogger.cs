using System;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Test
{
    internal class TestLogger : ILogger
    {
        #region ILogger Implementation
        public void Log(LogType logType, object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            Console.WriteLine("[" + logType.ToString().ToUpper() + "] " + memberName + "@" + sourceLineNumber + " - " + obj);
        }

        public void Debug(object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            Log(LogType.Debug, obj, ex, memberName, sourceLineNumber);
        }

        public void Info(object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            Log(LogType.Info, obj, ex, memberName, sourceLineNumber);
        }

        public void Warn(object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            Log(LogType.Warn, obj, ex, memberName, sourceLineNumber);
        }

        public void Error(object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            Log(LogType.Error, obj, ex, memberName, sourceLineNumber);
        }
        #endregion
    }
}
