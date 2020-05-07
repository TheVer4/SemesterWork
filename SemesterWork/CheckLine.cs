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

        public CheckLine(ProductData data, double amount)
        {
            Data = data;
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