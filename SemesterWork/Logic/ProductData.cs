using System.Collections.Generic;
using System.Windows.Input;
using Newtonsoft.Json;

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

        [JsonConstructor]
        public ProductData(string EAN13, string name, double price, double amount, string units, string shortName) 
            : this(EAN13)
        {
            Name =  name;
            Price = price;
            Amount =  amount;
            Units =  units;
            ShortName =  shortName;
        }
        
        public ProductData(List<string> data) 
            : this(data[0], data[1], double.Parse(data[2]), double.Parse(data[3]), data[4], data[5])
        { }

        public ProductData(string code)
        {
            EAN13 = code;
        }
    }
}
