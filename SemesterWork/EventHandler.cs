using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;

namespace SemesterWork
{
    public partial class MainWindow
    {

        private bool updatedSomeValue;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";
            printInvoice.Print();
        }
        
        private void ClearOnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void BarcodeReaded(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort e = (SerialPort) sender;
            var scanned = e.ReadTo("\r");
            invoicePositions.Add(new CheckLine(scanned, 1));
            updatedSomeValue = true;
        }
    }
}