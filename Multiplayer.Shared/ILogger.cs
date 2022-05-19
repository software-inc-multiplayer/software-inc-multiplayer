using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Shared
{
    /// <summary>
    /// shared logging interface
    /// </summary>
    public interface ILogger
    {
        void Log(LogType logType, object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0);
        void Debug(object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0);
        void Info(object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0);
        void Warn(object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0);
        void Error(object obj, Exception ex = null, [CallerMemberName] string memberName = "", [CallerLineNumber] int sourceLineNumber = 0);
    }

    public enum LogType
    {
        Info = 0,
        Debug = 2,
        Warn = 3,
        Error = 4
    }
}
