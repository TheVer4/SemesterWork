using System.Collections.Generic;

namespace SemesterWork
{
    class WareHouseDBController
    {
        public static List<string> Find(string code)
        {
            return DBController.SQLFind("WareHouse", "EAN13", code);
        }

        public static void Insert(
            string ean13,
            string name,
            double price,
            double amount,
            string units,
            string shortName)
        {
            DBController.SQLInsert(
                "WareHouse",
                "EAN13, Name, Price, Amount, Units, ShortName",
                $"'{ean13}', '{name}', '{price}', '{amount}', '{units}', '{shortName}'");
        }

        public static void Update(string code, string column, object value)
        {
            DBController.SQLUpdate("WareHouse", column, value, "EAN13", code);
        }

        public static void Remove(string code)
        {
            DBController.SQLRemove("WareHouse", "EAN13", code);
        }

        public static void IncreaseAmountBy(string code, double amount)
        {
            var data = new ProductData(Find(code));
            Update(code, "Amount", data.Amount + amount);
        }

        public static void DecreaseAmountBy(string code, double amount)
        {
            var data = new ProductData(Find(code));
            Update(code, "Amount", data.Amount - amount);
        }
    }
}
