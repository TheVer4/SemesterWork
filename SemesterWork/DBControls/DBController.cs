using System.Collections.Generic;
using System.Data.SQLite;

namespace SemesterWork
{
    static class DBController
    {      
        public static List<string> SQLFind(string table, string column, object value)
        {
            var result = new List<string>();
            using var connection = new SQLiteConnection(Variables.DBConnectionString);
            connection.Open();
            var command = new SQLiteCommand(
                $"SELECT * FROM {table} " +
                $"WHERE {column}='{value}'",
                connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
                for (var i = 0; i < reader.FieldCount; i++)
                    result.Add(reader.GetString(i));
            return result;
        }

        public static List<string> SQLFindBetween(string table, string column, object first, object second)
        {
            var result = new List<string>();
            using var connection = new SQLiteConnection(Variables.DBConnectionString);
            connection.Open();
            var command = new SQLiteCommand(
                $"SELECT * FROM {table} " +
                $"WHERE {column} BETWEEN {first} AND {second}",
            connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
                for (var i = 0; i < reader.FieldCount; i++)
                    result.Add(reader.GetString(i));
            return result;
        }

        public static void SQLInsert(string table, string columns, string values)
        {
            SQLCommand(
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
            SQLCommand(
                $"UPDATE {table} SET {udatableColumn}='{newValue}' " +
                $"WHERE {mainColumn}='{mainValue}'");
        }

        public static void SQLRemove(string table, string column, object value)
        {
            SQLCommand(
                $"DELETE FROM {table} " +
                $"WHERE {column}='{value}'");
        }

        public static void SQLCommand(string SQLCommand)
        {
            using var connection = new SQLiteConnection(Variables.DBConnectionString);
            connection.Open();
            var command = new SQLiteCommand(SQLCommand, connection);
            var number = command.ExecuteNonQuery();
        }
    }
}