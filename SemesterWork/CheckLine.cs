using System.Linq;

namespace SemesterWork
{
    public class CheckLine
    {
        public string EAN13 { get; private set; }
        public double Price { get; private set; }
        public double FullAmount { get; private set; }
        public string Units { get; private set; }
        public string ShortName { get; private set; }

        private double _amount;
        public double Amount
        {
            get => _amount;
            set
            {
                FullPrice = Price * value;
                _amount = value;
            }
        }

        public double FullPrice 
        { 
            get => Price * Amount;
            private set { }
        }

        public CheckLine(string code, double amount)
        {
            var info = DBController.Find(code); ;
            
            EAN13 = info[0].ToString();
            Price = (double)info[2];
            FullAmount = (double)info[3];
            Units = info[4].ToString();
            ShortName = info[5].ToString();
            Amount = amount;
        }

        public override string ToString()
        {
            var firstLine = ShortName + " " + Price + " руб.";
            var secondLine = "x" + Amount + ' ' + Units + " = " + FullPrice + " руб.";
            return firstLine + new string(' ', 32 - firstLine.Length)
                + new string(' ', secondLine.Length) + secondLine;
        }
    }
}