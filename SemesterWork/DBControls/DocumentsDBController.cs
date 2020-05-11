using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SemesterWork
{
    static class DocumentsDBController
    {
        public static List<string> FindById(int id)
        {
            return DBController.SQLFind("documents", "id", id);
        }

        public static List<string> FindByDateTime(int first, int second)
        {
            return DBController.SQLFindBetween("documents", "DateTime", first, second);
        }

        public static List<string> FindByCashierName(string name)
        {
            return DBController.SQLFind("documents", "Name", name);
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
