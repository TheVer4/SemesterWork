using PrinterUtility;
using System.Windows;

namespace SemesterWork
{
    public class Printer
    {
        public void Send(byte[] data)
        {
            try 
            { 
                PrintExtensions.Print(data, Variables.PrinterPath);
            }
            catch
            {
                MessageBox.Show(
                    EventHandler.Lang["Printer NotFoundException"],
                    EventHandler.Lang["Printer NotFoundExceptionTitle"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}