using System.Data.Entity.Core;
using System.Linq;
using System.Windows;

namespace SemesterWork
{
    public class CheckLine
    {
        public ProductData Data { get; private set; }

        private double _amount;
        public double Amount
        {
            get => _amount;
            set
            {
                FullPrice = Data.Price * value;
                _amount = value;
            }
        }

        public double FullPrice 
        { 
            get => Data.Price * Amount;
            private set { }
        }

        public CheckLine(string code, double amount)
        {        
            var info = DBController.Find(code);
            if (!info.Any())
                throw new ObjectNotFoundException();
            Data = new ProductData(info);
            Amount = amount;
        }

        public override string ToString()
        {
            var firstLine = Data.ShortName + " " + Data.Price + " руб.";
            var secondLine = "x" + Amount + ' ' + Data.Units + " = " + FullPrice + " руб.";
            return firstLine + new string(' ', 32 - firstLine.Length)
                + new string(' ', 32 - secondLine.Length) + secondLine;
        }
    }
}