using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    public interface IRepository<T>
    {
        IEnumerable<T> List(string source);
        Dictionary<string, T> DictionaryList(string source);
    }
}
