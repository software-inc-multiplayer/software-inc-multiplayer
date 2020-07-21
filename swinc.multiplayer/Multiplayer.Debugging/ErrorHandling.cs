using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Multiplayer.Debugging
{
    public class ErrorHandling
    {
        public static void Handle(Error error)
        {

        }
        public class Error
        {
            public string Code { get; set; }
            public string Message { get; set; }
            public string StackTrace { get; set; }
            public string Source { get; set; }
            public string TargetSite { get; set; }
            public Error()
            {

            }
            public Error(string code)
            {
                Code = code;
            }
            public Error(string code, string message)
            {
                Code = code;
                Message = message;
            }
            public Error(string code, string message, string stackTrace)
            {
                Code = code;
                Message = message;
                StackTrace = stackTrace;
            }
            public Error(string code, string message, string stackTrace, string source)
            {
                Code = code;
                Message = message;
                StackTrace = stackTrace;
                Source = source;
            }
            public Error(string code, string message, string stackTrace, string source, string targetSite)
            {
                Code = code;
                Message = message;
                StackTrace = stackTrace;
                Source = source;
                TargetSite = targetSite;
            }
        }
    }
}
