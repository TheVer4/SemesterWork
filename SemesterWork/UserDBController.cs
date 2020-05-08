using System.Collections.Generic;

namespace SemesterWork
{
    class UserDBController
    {
        public static List<string> FindByName(string name)
        {
            return DBController.SQLFind("Users", "Name", name);
        }

        public static List<string> FindById(string id)
        {
            return DBController.SQLFind("Users", "id", id);
        }

        public static List<string> FindByHash(string hash)
        {
            return DBController.SQLFind("Users", "Hash", hash);
        }

        public static void Add(string id, string name, string accessLevel, string hash)
        {
            DBController.SQLInsert(
                "Users",
                @"id, Name, AccessLevel, Hash",
                $"'{id}', '{name}', '{accessLevel}', '{hash}'");
        }

        public static void Update(string id, string column, string value)
        {
            DBController.SQLUpdate("Users", column, value, "id", id);
        }

        public static void Remove(string id)
        {
            DBController.SQLRemove("Users", "id", id);
        }
    }
}
