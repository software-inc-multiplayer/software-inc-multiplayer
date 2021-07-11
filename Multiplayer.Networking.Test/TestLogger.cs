using System;
using Multiplayer.Shared;

namespace Multiplayer.Networking.Test
{
    internal class TestLogger : ILogger
    {
        #region ILogger Implementation
        public void Log(LogType logType, object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        public void Debug(object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        public void Info(object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        public void Warn(object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }

        public void Error(object obj, Exception ex = null, string memberName = "", int sourceLineNumber = 0)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
