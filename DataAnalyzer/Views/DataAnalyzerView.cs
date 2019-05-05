using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAnalyzer.Helpers;
using log4net;

namespace DataAnalyzer.Views
{
    public static class DataAnalyzerView
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void DisplayTopPerformersByProduct(Dictionary<string, PeakNonPeak> topProductTypes,
            ResultSummary result, Dictionary<string,long> exceptions)
        {
            Logger.InfoFormat("Processed total of {0} items with Success: {1}, Errors: {2}, Warning: {3}",
                result.GetTotal(), result.Success, result.Errors, result.Warnings);
            Logger.InfoFormat("There were total of '{0}' sales without matching products.",
                exceptions.Count);
            foreach(KeyValuePair<string,long> kvp in exceptions)
            {
                Logger.DebugFormat("Product_Id: {0} has {1} unmapped entries.",
                    kvp.Key, kvp.Value);
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[Product Type]".PadLeft(40));
            sb.Append("[Score]".PadLeft(15));
            sb.Append("[Peak Sales]".PadLeft(10));
            sb.Append("[Non Peak Sales]".PadLeft(10));
            sb.Append(Environment.NewLine);
            sb.Append(new String('-', 75));
            sb.Append(Environment.NewLine);

            foreach (var obj in topProductTypes.Select(o => new { ProductType = o.Key,
                                                                  Score = o.Value.Computed,
                                                                  Peak = o.Value.PeakCount,
                                                                  NonPeak = o.Value.NonPeak
                                                                    }).OrderByDescending(o=>o.Score).
                                                                    ThenBy(o => o.ProductType))
            {
                //want to have record in teh log for running the task
                string message = String.Format("{0}:{1:0.00000} (Peak Sales {2}/ Non Peak: {3})",
                    obj.ProductType, obj.Score, obj.Peak, obj.NonPeak);
                Logger.Info(message);

                sb.Append(obj.ProductType.PadLeft(40));
                sb.Append(obj.Score.ToString("0.00000").PadLeft(15));
                sb.Append(obj.Peak.ToString().PadLeft(10));
                sb.Append(obj.NonPeak.ToString().PadLeft(10));
                sb.Append(Environment.NewLine);
                sb.Append(new String('-', 75));
                sb.Append(Environment.NewLine);
            }

            //display result clutter free
            Logger.InfoFormat("Displaying top {0} (Peak Sales/ Non Peak), ignoring entries with Non Peak = 0"
                                ,topProductTypes.Count);
            sb.Append(Environment.NewLine);
            Console.Out.WriteLine(sb.ToString());
            sb.Append(Environment.NewLine);

        }
    }
}
