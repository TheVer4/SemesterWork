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
                    LanguageEngine.Language["Printer NotFoundException"],
                    LanguageEngine.Language["Printer NotFoundExceptionTitle"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}