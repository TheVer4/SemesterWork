using System.Collections.Generic;

namespace SemesterWork
{
    static class UserDBController
    {
        public static List<string> FindById(string id)
        {
            return DBController.SQLFindUnique("Users", "id", id);
        }

        public static List<List<string>> FindLike(string infoStr)
        {
            return DBController.SQLNonVoidCommand(
                $"SELECT * FROM Users " +
                $"WHERE id LIKE '%{infoStr}%' " +
                $"OR Name LIKE '%{infoStr}%' " +
                $"OR AccessLevel LIKE '%{infoStr}%'");
        }

        public static void Add(string id, string name, string accessLevel, string hash)
        {
            DBController.SQLInsert(
                "Users",
                "id, Name, AccessLevel, Hash",
                $"'{id}', '{name}', '{accessLevel}', '{hash}'");
        }

        public static void Update(User data, string hash)
        {
            DBController.SQLVoidCommand(
                $"UPDATE Users SET " +
                $"Name = '{data.Name}', " +
                $"AccessLevel = '{data.AccessLevel}', " +
                $"Hash = '{hash}' " +
                $"WHERE id = '{data.Id}'");
        }

        public static void Update(User data)
        {
            DBController.SQLVoidCommand(
                $"UPDATE Users SET " +
                $"Name = '{data.Name}', " +
                $"AccessLevel = '{data.AccessLevel}' " +
                $"WHERE id = '{data.Id}'");
        }

        public static void Remove(string id)
        {
            DBController.SQLRemove("Users", "id", id);
        }
    }
}
