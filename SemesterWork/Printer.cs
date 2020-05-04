using PrinterUtility;

namespace SemesterWork
{
    public class Printer
    {
        public void Send(byte[] data)
        {
            PrintExtensions.Print(data, Variables.PrinterPath);
        }
    }
}