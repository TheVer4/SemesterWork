using System.IO.Ports;
using System.Windows;

namespace SemesterWork
{
    public partial class MainWindow
    {
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";
            printInvoice.Print();
        }

        private void BarcodeReaded(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort e = (SerialPort) sender;
            var scanned = e.ReadExisting();
            //getting info from DB and locating it to `invoicePositions`
        }
    }
}