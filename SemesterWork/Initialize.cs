namespace SemesterWork
{
    public partial class MainWindow
    {
        PrintInvoice printInvoice;
        private void InitializeEnvironment()
        {
            printInvoice = new PrintInvoice();
            //FastInvoiceActivity();
            LoginActivity();
            Variables.BarcodeScannerPort = "COM4";
            BarcodeReader barcodeReader = new BarcodeReader(Variables.BarcodeScannerPort, 9600);
            barcodeReader.AddReader(BarcodeReaded);
        }
    }
}