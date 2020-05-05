using System.Windows;

namespace SemesterWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        PrintInvoice printInvoice;
        public MainWindow()
        {
            var a = new CheckLine("1231231231231", 2);
            InitializeComponent();
            printInvoice = new PrintInvoice();
            //FastInvoiceActivity();
            LoginActivity();
        }
    }
}