using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAnalyzer
{
    public class PeakNonPeakComparer : IComparer<PeakNonPeak>
    {
        public int Compare(PeakNonPeak x, PeakNonPeak y)
        {
            if(x.Computed > y.Computed)
            {
                return 1;
            }

            if(x.Computed < y.Computed)
            {
                return -1;
            }

            if(x.Computed == y.Computed)
            {
                return 0;
            }

            return 0;
        }
    }
}
