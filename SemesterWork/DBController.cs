using System.Collections.Generic;
using System.Data.SQLite;

namespace SemesterWork
{
    static class DBController
    {
        public static List<object> Find(string code)
        {
            var result = new List<object>();
            using (var connection = new SQLiteConnection(Variables.DBConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand($"SELECT * FROM WareHouse WHERE EAN13={code}", connection))
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                            for (int i = 0; i < reader.FieldCount; i++)
                                result.Add(reader.GetValue(i));
            }
            return result;
        }
    }
}