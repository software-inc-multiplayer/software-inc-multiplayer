using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Shared
{
    /// <summary>
    /// shared logging interface
    /// </summary>
    public interface ILogger
    {
        void Log(LogType logType, params object[] objs);
        void Debug(params object[] objs);
        void Info(params object[] objs);
        void Warn(params object[] objs);
        void Error(params object[] objs);
    }

    public enum LogType
    {
        Info = 0,
        Debug = 2,
        Warn = 3,
        Error = 4
    }
}
