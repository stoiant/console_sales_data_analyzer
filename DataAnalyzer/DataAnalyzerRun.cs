using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManyConsole;
using log4net;
using DataAnalyzer.Helpers;
using DataAnalyzer.Views;

namespace DataAnalyzer
{
    class DataAnalyzerRun : ConsoleCommand
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private const int Success = 0;
        private const int Failure = 2;

        public string SalesFile { get; set; }
        public string ProductFile { get; set; }

        public DataAnalyzerRun()
        {
            // Register the actual command with a simple sdescription.
            IsCommand("TopProductTypes", "Perform summary of the top 5 product types.");

            const string longDescription = @"
            ///////////////////////////   Data Analyzer    ////////////////////////
            // Command line tool that takes in these Sales Data File (CSV) and
            // Product Data File (JSON Array) as arguments on the command line 
            // and prints to standard out the five product types with the 
            // best peak / non-peak sales ratio.
            // * Entries which have invalid format are ignored and logged
            // * When there are no non-peak sales entries are ignored and logged
            // * When there is a tie, it is decided based on alphabetical order
            // * Best attempt is made to produce statistics without termination
            // * peak period (define here as (all orders placed in October, 
            //   and December)
            // * non-peak period (orders placed in any other month)
            // * display the five product types with the highest result (alphabetical on tie,
            //   up to 5 entries are displayed
            // * Products is saved in memory
            // * Sales is itterated over to perform any calculations as it can be a large file
            // 
            // Please look at README.MD for more details.
            //
            ///////////////////////////////////////////////////////////////////////
            ";

            // Longer description for the help on the scommand.
            HasLongDescription(longDescription);

            // Required options/flags, append '=' to obtain the required value.
            HasRequiredOption("p|productFile=", "The full path of the products JSON file.", p => ProductFile = p);
            HasRequiredOption("s|salesFile=", "The full path of the sales CSV file.", p => SalesFile = p);
        }

        public override int Run(string[] remainingArguments)
        {
            try
            {
                using (new ExecutionTimeLogger("Data Analyzer"))
                {
                    int topN = -1;
                    List<DateTime> peakPeriod = new List<DateTime>();
                    //obtain configuration settings
                    try
                    {
                        topN = Properties.AppSettings.Default.TopN;
                        foreach (string date in Properties.AppSettings.Default.PeakPeriod)
                        {
                            //only month will be used and supplied in the configuration
                            peakPeriod.Add(DateTime.Parse(date));
                        }
                    }
                    catch(Exception e)
                    {
                        Logger.Error("Failed to obtain application settings.", e);
                        return Failure;
                    }

                    ResultSummary results = new ResultSummary();
                    Dictionary<string, long> exceptions = new Dictionary<string, long>();

                    //parse the list of products then itterate over the sales data
                    //to perform the calculations
                    Dictionary<string, PeakNonPeak> topPerformersByProductType = 
                        AnalyzeData.GetTopPeakNonPeakByProductType(SalesFile, ProductFile, topN, peakPeriod, 
                        out results, out exceptions);
                    //report the results
                    DataAnalyzerView.DisplayTopPerformersByProduct(topPerformersByProductType, results, exceptions);

                    //Add more data analysys in the future
                }
                return Success;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Failure;
            }
        }
    }
}
