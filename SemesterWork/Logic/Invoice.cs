using System.Collections.Generic;

namespace SemesterWork
{
    public class Invoice
    {
        public double Cash { get; set; }
        public double Cashless { get; set; }
        public double Sale { get; set; }
        public double Change { get; set; }
        public List<CheckLine> Positions { get; set; }
        public string CashierName { get; set; }
        
        public Invoice(double cash, double cashless, double sale, double change, List<CheckLine> positions, string cashierName)
        {
            Cash = cash;
            Cashless = cashless;
            Positions = positions;
            CashierName = cashierName;
            Sale = sale;
            Change = change;
        }
    }
}