using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace SemesterWork
{
    static class DBController
    {
        public static List<List<string>> SQLFind(string table, string column, object value)
        {
            return SQLNonVoidCommand(
                $"SELECT * FROM {table} " +
                $"WHERE {column}='{value}'");
        }

        public static List<string> SQLFindUnique(string table, string column, object value)
        {
            return SQLFind(table, column, value).FirstOrDefault() ?? new List<string>();
        }

        public static List<List<string>> SQLFindBetween(string table, string column, object first, object second)
        {
            return SQLNonVoidCommand(
                $"SELECT * FROM {table} " +
                $"WHERE {column} BETWEEN {first} AND {second}");
        }

        public static List<string> SQLFindDistinct(string table, string column)
        {
            return SQLNonVoidCommand(
                $"SELECT DISTINCT {column} FROM {table}")
                .Select(x => x.First())
                .ToList();
        }      

        public static void SQLInsert(string table, string columns, string values)
        {
            SQLVoidCommand(
                $"INSERT INTO {table} ({columns}) " +
                $"VALUES ({values})");
        }

        public static void SQLUpdate(
            string table,
            string udatableColumn,
            object newValue,
            string mainColumn,
            object mainValue)
        {
            SQLVoidCommand(
                $"UPDATE {table} SET {udatableColumn}='{newValue}' " +
                $"WHERE {mainColumn}='{mainValue}'");
        }

        public static void SQLRemove(string table, string column, object value)
        {
            SQLVoidCommand(
                $"DELETE FROM {table} " +
                $"WHERE {column}='{value}'");
        }

        public static void SQLVoidCommand(string SQLCommand)
        {
            using var connection = new SQLiteConnection(Variables.DBConnectionString);
            connection.Open();
            var command = new SQLiteCommand(SQLCommand, connection);
            var number = command.ExecuteNonQuery();
        }

        public static List<List<string>> SQLNonVoidCommand(string SQLCommand)
        {
            var result = new List<List<string>>();
            using var connection = new SQLiteConnection(Variables.DBConnectionString);
            connection.Open();
            var command = new SQLiteCommand(SQLCommand, connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var list = new List<string>();
                for (var i = 0; i < reader.FieldCount; i++)
                    list.Add(reader.GetValue(i).ToString());
                result.Add(list);
            }
            return result;
        }
    }
}