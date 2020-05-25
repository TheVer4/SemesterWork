using System.Collections.Generic;

namespace SemesterWork
{
    public class ProductData
    {
        public string EAN13 { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Amount { get; set; }
        public string Units { get; set; }
        public string ShortName { get; set; }

        public ProductData(List<string> data)
        {
            EAN13 = data[0];
            Name = data[1];
            Price = double.Parse(data[2]);
            Amount = double.Parse(data[3]);
            Units = data[4];
            ShortName = data[5];
        }

        public ProductData(string code)
        {
            EAN13 = code;
        }
    }
}
