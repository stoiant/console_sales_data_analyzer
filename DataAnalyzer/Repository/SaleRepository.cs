using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace DataAnalyzer
{
    class SaleRepository : IRepository<SaleVO>
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        Dictionary<string, SaleVO> IRepository<SaleVO>.DictionaryList(string source)
        {
            throw new NotImplementedException();
        }

        IEnumerable<SaleVO> IRepository<SaleVO>.List(string source)
        {
            if (!File.Exists(source))
            {
                Logger.ErrorFormat("The specified input file '{0}' cannot be found. Please ensure that the file exists.",
                    source);
                return null;
            }
            return ParseCSV(source);
        }

        /// <summary>
        /// Parse the Sales.csvs and create a list of SaleVO's
        /// Do best effort to process what data is available.
        /// This none conservative approach is generally not good for PROD
        /// </summary>
        /// <param name="source">Valid path to Sales.csv</param>
        /// <returns></returns>
        private IEnumerable<SaleVO> ParseCSV(string source)
        {
            long index = 0;
            long numBadRecords = 0;

            int numFields = 7;

            using (var readFile = new StreamReader(source))
            {
                string line;
                string[] parts;

                while ((line = readFile.ReadLine()) != null)
                {
                    parts = line.Split(',');
                    index += 1;

                    if (parts == null)
                    {
                        break;
                    }

                    index += 1;

                    if (parts.Length != numFields)
                    {
                        Logger.ErrorFormat("Line: {0} has {1} fields but expected: {2}. Ignoring error and continuing processing.",
                            index, parts.Length, numFields);
                        continue;
                    }

                    // Skip first row which in this case is a header with column names
                    if (index <= 1) continue;

                    string productId = parts[0];
                    DateTime orderDate = DateTime.MinValue;
                    DateTime shipDate = DateTime.MinValue;
                    decimal priceInCents = -1m;
                    int NPSScore = -1;
                    string postalCode = parts[5];
                    int serviceLevel = -1;

                    /*
                        * Column Type check
                        */
                    var validRow = !string.IsNullOrWhiteSpace(parts[0]) &&
                                    DateTime.TryParse(parts[1], out orderDate) &&
                                    DateTime.TryParse(parts[2], out shipDate) &&
                                    decimal.TryParse(parts[3].Trim(), out priceInCents) &&
                                    int.TryParse(parts[4].Trim(), out NPSScore) &&
                                    !string.IsNullOrWhiteSpace(parts[5]) &&
                                    int.TryParse(parts[6], out serviceLevel);

                    /*
                        * Validate Questionable fields
                        * I was unable to obtain schema for the fields so currently
                        * fields are not validate for range, being empty etc. but
                        * can be enhaned once the info is made available
                        */

                    if (validRow)
                    {

                        yield return new SaleVO()
                        {
                            Product_ID = productId,
                            Order_Date = orderDate,
                            Ship_Date = shipDate,
                            Price_In_Cents = priceInCents,
                            NPS_Score = NPSScore,
                            Postal_Code = postalCode,
                            Service_Level = serviceLevel
                        };
                    }
                    else
                    {
                        Logger.ErrorFormat("Encountered BAD data for sales on row {0}. Continuing processing, ignoring error.", index);
                        numBadRecords++;
                    }
                }
            }
        }
    }
}
