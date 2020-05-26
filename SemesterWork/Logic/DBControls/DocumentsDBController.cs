using System.Collections.Generic;

namespace SemesterWork
{
    static class DocumentsDBController
    {
        public static List<string> FindById(int id)
        {
            return DBController.SQLFindUnique("documents", "id", id);
        }

        public static List<List<string>> FindByDateTime(int first, int second)
        {
            return DBController.SQLFindBetween("documents", "DateTime", first, second);
        }

        public static List<string> FindByCashierName(string name)
        {
            return DBController.SQLFindUnique("documents", "Name", name);
        }

        public static void Add(int dateTime, string name, string checkLine)
        {
            DBController.SQLInsert(
                "documents",
                "DateTime, CashierName, Data",
                $"{dateTime}, '{name}', '{checkLine}'");
        }
    }
}
