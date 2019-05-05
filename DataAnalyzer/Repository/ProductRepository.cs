using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ErrorEventArgs = Newtonsoft.Json.Serialization.ErrorEventArgs;

namespace DataAnalyzer
{
    class ProductRepository : IRepository<ProductVO>
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private long errorsCount = 0;
        private long duplicateProductIdCount = 0;

        Dictionary<string, ProductVO> IRepository<ProductVO>.DictionaryList(string source)
        {
            if (!File.Exists(source))
            {
                Logger.ErrorFormat("The specified input file '{0}' cannot be found. Please ensure that the file exists.",
                    source);
                return null;
            }
            return ParseJSONFile(source);
        }

        /// <summary>
        /// Method is not required for Product
        /// </summary>
        /// <param name="source">Valid path to Product.json</param>
        /// <returns></returns>
        IEnumerable<ProductVO> IRepository<ProductVO>.List(string source)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parse the Product.json and create a dictionary with Product_Id as key
        /// and VO as values. Do best effort to process what data is available.
        /// This none conservative approach is generally not good for PROD
        /// </summary>
        /// <param name="source">Valid path to Product.json</param>
        /// <returns></returns>
        private Dictionary<string, ProductVO> ParseJSONFile(string source)
        {
            Dictionary<string, ProductVO> result = new Dictionary<string, ProductVO>();

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Error,
                ContractResolver = new FailingContractResolver(),
                Error = Error
            };

            JsonSerializer serializer = JsonSerializer.Create(settings);

            List<ProductVO> products = JsonConvert.DeserializeObject<List<ProductVO>>(File.ReadAllText(source));
            foreach(ProductVO product in products)
            {
                if(result.ContainsKey(product.Product_Id))
                {
                    duplicateProductIdCount++;
                    Logger.ErrorFormat("Found duplicate Product_Id: {0}. Ignoring and continuing processing.",
                        product.Product_Id);
                }
                else
                {
                    result.Add(product.Product_Id, product);
                }
            }

            Logger.InfoFormat("There were {0} errors encountered while parsing product data. If found errors will be ignored.",
                errorsCount);
            Logger.InfoFormat("Encountered {0} duplicate Product_Ids. If found will be ignored to process available data.",
                duplicateProductIdCount);

            return result;
        }

        private void Error(object sender, ErrorEventArgs errorEventArgs)
        {
            Logger.ErrorFormat("Encountered error while parssing. Message : {0} Path: {1} Member: {2}",
                errorEventArgs.ErrorContext.Error.Message,
                errorEventArgs.ErrorContext.Path,
                errorEventArgs.ErrorContext.Member);
            errorsCount++;
            errorEventArgs.ErrorContext.Handled = true;
        }

        public class FailingContractResolver : DefaultContractResolver
        {
            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty res = base.CreateProperty(member, memberSerialization);

                if (!res.Ignored)
                    // If we haven't explicitly stated that a field is not needed, we require it for compliance
                    res.Required = Required.AllowNull;

                return res;
            }
        }
    }
}
