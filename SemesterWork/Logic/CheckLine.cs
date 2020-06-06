using System;

namespace SemesterWork
{
    public class CheckLine
    {
        public ProductData Data { get; set; }

        private double _amount;
        public double Amount
        {
            get => _amount;
            set
            {
                FullPrice = Math.Round(Data.Price * value, 2);
                _amount = Math.Round(value, 3);
            }
        }

        public double FullPrice 
        { 
            get => Math.Round(Data.Price * Amount, 2);
            private set { }
        }

        public CheckLine(ProductData data, double amount)
        {
            Data = data;
            Amount = amount;
        }

        public override string ToString()
        {
            var firstLine = Data.ShortName.Substring(0, Data.ShortName.Length > 16 ? 16 : Data.ShortName.Length ) + " " + Data.Price + " руб.";
            var secondLine = "x" + Amount + ' ' + Data.Units + " = " + FullPrice + " руб.";
            return firstLine + new string(' ', 32 - firstLine.Length)
                + new string(' ', 32 - secondLine.Length) + secondLine;
        }
    }
}