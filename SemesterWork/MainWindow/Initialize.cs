namespace SemesterWork
{
    public partial class MainWindow
    {
        PrintInvoice printInvoice;
        private LanguageEngine _lang;
        private void InitializeEnvironment()
        {
            printInvoice = new PrintInvoice();
            Variables.BarcodeScannerPort = "COM4";
            _lang = new LanguageEngine();
            LoginActivity();
        }
    }
}