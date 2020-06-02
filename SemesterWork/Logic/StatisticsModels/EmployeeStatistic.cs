using System.Collections.Generic;
using System.Linq;

namespace SemesterWork
{
    public class EmployeeStatistic
    {
        public string CashierName { get; set; }
        public int Invoices { get; set; }
        public double Average { get => Total / Invoices; set {} }
        public double Total { get; set; }

        public EmployeeStatistic(string name, int invoices, double total)
        {
            CashierName = name;
            Invoices = invoices;
            Total = total;
        }
        public EmployeeStatistic(List<Invoice> invoices)
        {
            foreach (var invoice in invoices)
                Total += invoice.Cash + invoice.Cashless + invoice.Change;
            CashierName = invoices.First().CashierName;
            Invoices = invoices.Count;
        }
    }
}