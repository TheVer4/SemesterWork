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

        public static void Add(int dateTime, string name, string checkLine)
        {
            DBController.SQLInsert(
                "documents",
                "DateTime, CashierName, Data",
                $"{dateTime}, '{name}', '{checkLine}'");
        }
    }
}
