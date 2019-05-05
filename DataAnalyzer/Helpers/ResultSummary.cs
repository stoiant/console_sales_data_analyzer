using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer.Helpers
{
    /// <summary>
    /// Result summary class used to obtain processing result summary
    /// </summary>
    public class ResultSummary
    {
        public long Success { get; set; }
        public long Warnings { get; set; }
        public long Errors { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public ResultSummary()
        {
            Success = 0;
            Warnings = 0;
            Errors = 0;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="success"></param>
        /// <param name="warnings"></param>
        /// <param name="errors"></param>
        public ResultSummary(long success, long warnings, long errors)
        {
            this.Success = success;
            this.Warnings = warnings;
            this.Errors = errors;
        }

        /// <summary>
        /// Get total processed items
        /// </summary>
        /// <returns></returns>
        public long GetTotal()
        {
            return Success + Errors + Warnings;
        }
    }
}
