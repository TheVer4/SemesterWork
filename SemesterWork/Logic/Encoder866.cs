using System.Text;

namespace SemesterWork
{
    public static class Encoder866
    {
        public static Encoding Encoding = Encoding.GetEncoding(866);

        public static byte[] Recode(string str)
            => Encoding.GetBytes(str);

        public static string Recode(byte[] arr)
            => Encoding.GetString(arr);
    }
}