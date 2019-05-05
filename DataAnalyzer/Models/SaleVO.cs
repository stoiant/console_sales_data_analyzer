using System;

namespace DataAnalyzer
{
    /// <summary>
    /// Value Object to hold Sales Data
    /// </summary>
    public class SaleVO
    {
        public string Product_ID { get; set; }
        public DateTime Order_Date { get; set; }
        public DateTime Ship_Date { get; set; }
        public decimal Price_In_Cents { get; set; }
        public int NPS_Score { get; set; }
        public string Postal_Code { get; set; }
        public int Service_Level { get; set; }
    }
}
