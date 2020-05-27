using System;
using System.Collections.Generic;

namespace SemesterWork
{
    public class Invoice
    {
        public double Cash { get; private set; }
        public double Cashless { get; private set; }
        public double Sale { get; private set; }
        public double Change { get; private set; }
        public List<CheckLine> Positions { get; private set; }
        public string CashierName { get; private set; }
        
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