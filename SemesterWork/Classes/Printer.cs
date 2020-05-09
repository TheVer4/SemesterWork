using PrinterUtility;
using System.Windows;

namespace SemesterWork
{
    public class Printer
    {
        public void Send(byte[] data)
        {
            try { PrintExtensions.Print(data, Variables.PrinterPath); }
            catch 
            {
                MessageBox.Show(
                    MainWindow.Lang["Printer NotFoundExceptionTitle"],
                    MainWindow.Lang["Printer NotFountException"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}