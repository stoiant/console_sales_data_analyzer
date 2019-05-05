using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace DataAnalyzer
{
    /// <summary>
    /// Helper class to be used to record method and app execution time
    /// </summary>
    class ExecutionTimeLogger : IDisposable
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly string methodName;
        private readonly string message;
        private readonly Stopwatch stopwatch;

        public ExecutionTimeLogger(string message = "", [CallerMemberName] string methodName = "")
        {
            this.methodName = methodName;
            this.message = message;
            stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            TimeSpan t = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
            string humanReadableTime = string.Format("{0:D2}h:{1:D2}m:{2:D2}s:{3:D3}ms",
                                            t.Hours,
                                            t.Minutes,
                                            t.Seconds,
                                            t.Milliseconds);

            Logger.DebugFormat("{0} ({1}) took {1}", message, methodName, humanReadableTime);
            GC.SuppressFinalize(this);
        }
    }
}
