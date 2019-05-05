using System;
using System.Collections.Generic;
using DataAnalyzer;
using DataAnalyzer.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataAnalyzerTest
{
    [TestClass]
    public class UnitTest
    {
        public const string productFile = "product_data.json";

        [TestMethod]
        [DeploymentItem("TestFiles\\product_data.json")]
        [DeploymentItem("TestFiles\\sales_data.csv")]
        public void ProvidedInput()
        {
            string salesFile = "sales_data.csv";
            ResultSummary results = new ResultSummary();
            Dictionary<string, long> exceptions = new Dictionary<string, long>();
            List<DateTime> peakPeriod = new List<DateTime>();
            peakPeriod.Add(new DateTime(1, 10, 1));
            peakPeriod.Add(new DateTime(1, 11, 1));
            peakPeriod.Add(new DateTime(1, 12, 1));

            //parse the list of products then itterate over the sales data
            //to perform the calculations
            Dictionary<string, PeakNonPeak> topPerformersByProductType =
                AnalyzeData.GetTopPeakNonPeakByProductType(salesFile, productFile, 5, peakPeriod,
                out results, out exceptions);

            //WAS NOT provided expected output so just running to ensure it does not crash
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\product_data.json")]
        [DeploymentItem("TestFiles\\sales_data_alpha_sort_all_eq_less_than_5.csv")]
        public void AlphaSortAllEq()
        {
            string salesFile = "sales_data_alpha_sort_all_eq_less_than_5.csv";
            ResultSummary results = new ResultSummary();
            Dictionary<string, long> exceptions = new Dictionary<string, long>();
            List<DateTime> peakPeriod = new List<DateTime>();
            peakPeriod.Add(new DateTime(1, 10, 1));
            peakPeriod.Add(new DateTime(1, 11, 1));
            peakPeriod.Add(new DateTime(1, 12, 1));

            //parse the list of products then itterate over the sales data
            //to perform the calculations
            Dictionary<string, PeakNonPeak> topPerformersByProductType =
                AnalyzeData.GetTopPeakNonPeakByProductType(salesFile, productFile, 5, peakPeriod,
                out results, out exceptions);

            Assert.IsTrue(topPerformersByProductType.Count == 3);

            foreach(KeyValuePair<string,PeakNonPeak> kvp in topPerformersByProductType)
            {
                Assert.IsTrue(kvp.Value.Computed == 1m);
                Assert.IsTrue(kvp.Value.PeakCount == 2);
                Assert.IsTrue(kvp.Value.NonPeak == 2);
            }
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\product_data.json")]
        [DeploymentItem("TestFiles\\sales_data_none_matching_codes_bad_input.csv")]
        public void BadInputAllNoMatches()
        {
            string salesFile = "sales_data_none_matching_codes_bad_input.csv";
            ResultSummary results = new ResultSummary();
            Dictionary<string, long> exceptions = new Dictionary<string, long>();
            List<DateTime> peakPeriod = new List<DateTime>();
            peakPeriod.Add(new DateTime(1, 10, 1));
            peakPeriod.Add(new DateTime(1, 11, 1));
            peakPeriod.Add(new DateTime(1, 12, 1));

            //parse the list of products then itterate over the sales data
            //to perform the calculations
            Dictionary<string, PeakNonPeak> topPerformersByProductType =
                AnalyzeData.GetTopPeakNonPeakByProductType(salesFile, productFile, 5, peakPeriod,
                out results, out exceptions);

            Assert.IsTrue(results.Success == 0);
            Assert.IsTrue(results.Errors == 1);
            Assert.IsTrue(results.Warnings == 0);
        }

        [TestMethod]
        [DeploymentItem("TestFiles\\product_data.json")]
        [DeploymentItem("TestFiles\\sales_data_one_code_2p_3np.csv")]
        public void OneCode_2peak_3nonpeak()
        {
            //TODO
        }
    }
}
