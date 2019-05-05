using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAnalyzer.Helpers;
using log4net;

namespace DataAnalyzer
{
    /// <summary>
    /// Controller class that is used to perform data analysis on the supplied data.
    /// </summary>
    public static class AnalyzeData
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Provided sales and products files, Sales Data File (CSV) and
        /// Product Data File (JSON Array) as arguments obtain a Dictionary
        /// that contains the top N performers and the corresponding calculated
        /// Peak / Non-Peak Sales
        /// </summary>
        /// <param name="salesFile"></param> Invalid entries are ignored null returned if not valid
        /// <param name="productsFile"></param> Invalid entries are ignored null returned if not valid
        /// <param name="topN"></param> The TOP N results to be returned
        /// <param name="result"></param> Sets the Success, Error, Warning counts
        /// <param name="saleExceptions"></param> Sets <ProductID, long Count> for exception reporting
        /// <returns>Dictionary<string, PeakNonPeak> of the TOP N calculated performers</returns>
        public static Dictionary<string, PeakNonPeak> GetTopPeakNonPeakByProductType(string salesFile, string productsFile, 
            int topN, List<DateTime> peakPeriod, out ResultSummary result, out Dictionary<string,long> saleExceptions)
        {
            result = new ResultSummary();
            saleExceptions = new Dictionary<string, long>();

            if(!File.Exists(salesFile) || !File.Exists(productsFile))
            {
                Logger.ErrorFormat("Unable to access '{0}' or '{1}'. Please ensure the " +
                    "specified path if valid and accessible.", salesFile, productsFile);
                result = null;
                saleExceptions = null;
                return null;
            }

            using (new ExecutionTimeLogger("Analyze and display Peak/Non Peak Sales"))
            {
                IRepository<ProductVO> productRepository = new ProductRepository();
                Dictionary<string, ProductVO> products = productRepository.DictionaryList(productsFile);

                if (products == null || products.Count() == 0)
                {
                    Logger.ErrorFormat("Unable to populate product list. Please see log for reason.");
                    result = null;
                    saleExceptions = null;
                    return null;
                }

                IRepository<SaleVO> saleRepository = new SaleRepository();
                Dictionary<string, PeakNonPeak> analysys = new Dictionary<string, PeakNonPeak>();

                foreach (SaleVO saleVO in saleRepository.List(salesFile))
                {
                    if (!products.ContainsKey(saleVO.Product_ID))
                    {
                        Logger.DebugFormat("Found a sale with Product_ID: {0} " +
                            "that has undefined product.", saleVO.Product_ID);
                        if(saleExceptions.ContainsKey(saleVO.Product_ID))
                        {
                            saleExceptions[saleVO.Product_ID]++;
                        }
                        else
                        {
                            saleExceptions.Add(saleVO.Product_ID, 1);
;                        }
                        result.Errors++;
                        continue;
                    }

                    result.Success++;

                    bool isPeak = peakPeriod.Where(t => t.Month == saleVO.Order_Date.Month).Count() > 0;
                    string productType = products[saleVO.Product_ID].Product_Type.Trim().ToUpper();
                    if (!analysys.ContainsKey(productType))
                    {
                        PeakNonPeak peakNonPeak = new PeakNonPeak() { NonPeak = 0, PeakCount = 0 };
                        analysys.Add(productType, peakNonPeak);
                    }

                    if (isPeak)
                    {
                        analysys[productType].PeakCount++;
                    }
                    else
                    {
                        analysys[productType].NonPeak++;
                    }
                }

                //itterate over the summarized results to perform calculations
                foreach(KeyValuePair<string, PeakNonPeak> kvp in analysys)
                {
                    decimal pNP = -1;
                    if (kvp.Value.NonPeak != 0)
                    {
                        pNP = ((decimal)kvp.Value.PeakCount / (decimal)kvp.Value.NonPeak);
                    }
                    kvp.Value.Computed = pNP;
                }

                PeakNonPeakComparer comparer = new PeakNonPeakComparer();

                Dictionary<string, PeakNonPeak> topProductTypes = analysys.
                    OrderByDescending(v => v.Value, comparer). //sort results descending
                    Take(5).                                   //take the first 5 result, which are the highest
                    //OrderByDescending(v => v.Value, comparer).ThenBy(v => v.Key). //to be able to test sort by value ascending
                    ToDictionary(v => v.Key, v => v.Value);

                return topProductTypes;
            }
        }
    }
}
