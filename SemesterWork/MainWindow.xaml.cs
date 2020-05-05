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
            var res = DBController.Execute("SELECT Name FROM WareHouse WHERE EAN13=4660003400250");
            InitializeComponent();
            printInvoice = new PrintInvoice();
            //FastInvoiceActivity();
            LoginActivity();
        }
    }
}