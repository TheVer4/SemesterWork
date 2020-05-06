namespace SemesterWork
{
    public partial class MainWindow
    {
        PrintInvoice printInvoice;
        private void InitializeEnvironment()
        {
            printInvoice = new PrintInvoice();
            Variables.BarcodeScannerPort = "COM4";
            //FastInvoiceActivity();
            LoginActivity();
        }
    }
}