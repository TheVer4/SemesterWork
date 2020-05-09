namespace SemesterWork
{
    public partial class MainWindow
    {
        private PrintInvoice _printInvoice;
        public static LanguageEngine Lang;
        private void InitializeEnvironment()
        {
            _printInvoice = new PrintInvoice();
            Variables.BarcodeScannerPort = "COM4";
            Lang = new LanguageEngine();
            LoginActivity();
        }
    }
}