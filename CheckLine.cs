namespace SemesterWork
{
    public class CheckLine
    {
        private readonly int _EAN13;

<<<<<<< HEAD
        public readonly string Name;
        public readonly double Price;
        public readonly string Units;
        public double FullPrice
        {
=======
        public string Name { get; private set; }
        public double Price { get; private set; }
        public string Units { get; private set; }
        public string EAN13 { get; }
        public double FullPrice 
        { 
>>>>>>> 2e0060026168a37f5bcfee1e069a8b42676be698
            get => Price * Amount;
            private set { }
        }
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

        public CheckLine(int code, double amount)
        {
            // Здесь должен быть запрос в базу данных по EAN13
            //
            // var info = ...(code);
            //
            // _EAN13 = info.Code;
            // Name = info.ShortName;
            // Price = info.Price;
            // FullPrice = Price * amount;
            // Units = info.Units;

            Amount = amount;
        }

        public override string ToString()
        {
            var firstLine = Name + ' ' + Price + " руб.";
            var secondLine = 'x' + Amount + ' ' + Units + " = " + FullPrice + " руб.";
            return firstLine + new string(' ', 32 - firstLine.Length)
                + new string(' ', secondLine.Length) + secondLine;
        }
    }
}