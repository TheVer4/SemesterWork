namespace SemesterWork
{
    public partial class MainWindow
    {
        PrintInvoice printInvoice;
        private void InitializeEnvironment()
        {
            var res = DBController.Execute("SELECT Name FROM WareHouse WHERE EAN13=4660003400250");
            printInvoice = new PrintInvoice();
            //FastInvoiceActivity();
            LoginActivity();
            Variables.BarcodeScannerPort = "COM4";
            BarcodeReader barcodeReader = new BarcodeReader(Variables.BarcodeScannerPort, 9600);
            barcodeReader.AddReader(BarcodeReaded);
        }
    }
}