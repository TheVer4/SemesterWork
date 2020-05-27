using System.Collections.Generic;

namespace SemesterWork
{
    static class UserDBController
    {
        public static List<string> FindByName(string name)
        {
            return DBController.SQLFindUnique("Users", "Name", name);
        }

        public static List<string> FindById(string id)
        {
            return DBController.SQLFindUnique("Users", "id", id);
        }

        public static List<string> FindByHash(string hash)
        {
            return DBController.SQLFindUnique("Users", "Hash", hash);
        }

        public static List<List<string>> FindByAccessLevel(string accessLevel)
        {
            return DBController.SQLFind("Users", "AccessLevel", accessLevel);
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
            DBController.SQLCommand(
                $"UPDATE Users SET " +
                $"Name = '{data.Name}', " +
                $"AccessLevel = '{data.AccessLevel}', " +
                $"Hash = '{hash}' " +
                $"WHERE id = '{data.Id}'");
        }

        public static void Update(User data)
        {
            DBController.SQLCommand(
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
