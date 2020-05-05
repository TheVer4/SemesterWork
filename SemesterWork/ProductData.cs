using System.Collections.Generic;

namespace SemesterWork
{
    public class ProductData
    {
        public string EAN13 { get; private set; }
        public string Name { get; private set; }
        public double Price { get; private set; }
        public double Amount { get; private set; }
        public string Units { get; private set; }
        public string ShortName { get; private set; }

        public ProductData(List<string> data)
        {
            EAN13 = data[0];
            Name = data[1];
            Price = double.Parse(data[2]);
            Amount = double.Parse(data[3]);
            Units = data[4];
            ShortName = data[5];
        }
    }
}
