using System;

namespace DataAnalyzer
{
    /// <summary>
    /// Value Object to hold Products Data
    /// </summary>
    class ProductVO
    {
        public string Product_Id { get; set; }
        public string Product_Name { get; set; }
        public string Product_Type { get; set; }
        public string Product_Class { get; set; }
        public string Packaging { get; set; }
    }
}
