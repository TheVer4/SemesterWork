using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SemesterWork
{
    static class DocumentsDBController
    {
        public static long DateLeftLimit
        {
            get
            {
                var time = DBController
                    .SQLNonVoidCommand("SELECT DateTime FROM documents ORDER BY DateTime LIMIT 1")
                    .FirstOrDefault();
                return time != null ? long.Parse(time.First()) : 0;
            }
        }
        
        public static long DateRightLimit
        {
            get
            {
                var time = DBController
                    .SQLNonVoidCommand("SELECT DateTime FROM documents ORDER BY DateTime DESC LIMIT 1")
                    .FirstOrDefault();
                return time != null ? long.Parse(time.First()) : 0;
            }
        }

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
