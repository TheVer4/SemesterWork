namespace SemesterWork
{
    public partial class MainWindow
    {
        private PrintInvoice _printInvoice;
        public static LanguageEngine Lang;
        private void InitializeEnvironment()
        {
            _printInvoice = new PrintInvoice();
            Lang = new LanguageEngine();
            UpdateFromCFG();
            LoginActivity();
        }
    }
}