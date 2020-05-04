using System.Collections.Generic;
using System.Data.SQLite;

using DataSet = System.Collections.Generic.List<System.Collections.Generic.List<object>>;

namespace SemesterWork
{
    static class DBController
    {
        public static DataSet Execute(string request)
        {
            var result = new DataSet();
            using (var connection = new SQLiteConnection(Variables.DBConnectionString))
            {
                connection.Open();
                using (var command = new SQLiteCommand(request, connection))
                    using (var reader = command.ExecuteReader())
                        while (reader.Read())
                        {
                            var list = new List<object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                                list.Add(reader.GetValue(i));
                            result.Add(list);
                        }
            }
            return result;
        }
    }
}