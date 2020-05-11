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
                    MainWindow.Lang["Printer NotFoundException"],
                    MainWindow.Lang["Printer NotFoundExceptionTitle"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}