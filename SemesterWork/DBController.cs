using System.Collections.Generic;
using System.Data.SQLite;

namespace SemesterWork
{
    static class DBController
    {
        public static List<string> Find(string code)
        {
            return SQLFind("WareHouse", "EAN13", code);
        }

        public static void Insert(
            string ean13,
            string name,
            double price,
            double amount,
            string units,
            string shortName)
        {
            SQLInsert(
                "WareHouse",
                "EAN13, Name, Price, Amount, Units, ShortName",
                $"'{ean13}', '{name}', '{price}', '{amount}', '{units}', '{shortName}'");
        }

        public static void Update(string code, string column, object value)
        {
            SQLUpdate("WareHouse", column, value, "EAN13", code);
        }

        public static void Remove(string code)
        {
            SQLRemove("WareHouse", "EAN13", code);
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

        public static List<string> FindUserById(string id)
        {
            return SQLFind("Users", "id", id);
        }

        public static List<string> FindUserByName(string name)
        {
            return SQLFind("Users", "Name", name);
        }

        public static void AddUser(string id, string name, string accessLevel, string hash)
        {
            SQLInsert(
                "Users",
                @"id, Name, AccessLevel, Hash",
                $"'{id}', '{name}', '{accessLevel}', '{hash}'");
        }

        public static void UpdateUser(string id, string column, string value)
        {
            SQLUpdate("Users", column, value, "id", id);
        }

        public static void RemoveUser(string id)
        {
            SQLRemove("Users", "id", id);
        }

        private static List<string> SQLFind(string table, string column, object value)
        {
            var result = new List<string>();
            using (var connection = new SQLiteConnection(Variables.DBConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(
                    $"SELECT * FROM {table} " +
                    $"WHERE {column}='{value}'",
                    connection);
                var reader = command.ExecuteReader();
                while (reader.Read())
                    for (var i = 0; i < reader.FieldCount; i++)
                        result.Add(reader.GetString(i));
            }
            return result;
        }

        private static void SQLInsert(string table, string columns, string values)
        {

            using (var connection = new SQLiteConnection(Variables.DBConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(
                    $"INSERT INTO {table} ({columns}) " +
                    $"VALUES ({values})",
                    connection);
                var number = command.ExecuteNonQuery();
            }
        }

        private static void SQLUpdate(
            string table, 
            string udatableColumn,
            object newValue, 
            string mainColumn,
            object mainValue)
        {
            using (var connection = new SQLiteConnection(Variables.DBConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(
                    $"UPDATE {table} SET {udatableColumn}='{newValue}' " +
                    $"WHERE {mainColumn}='{mainValue}'",
                    connection);
                var number = command.ExecuteNonQuery();
            }
        }

        private static void SQLRemove(string table, string column, object value)
        {
            using (var connection = new SQLiteConnection(Variables.DBConnectionString))
            {
                connection.Open();
                var command = new SQLiteCommand(
                    $"DELETE FROM {table} " +
                    $"WHERE {column}='{value}'",
                    connection);
                var number = command.ExecuteNonQuery();
            }
        }
    }
}