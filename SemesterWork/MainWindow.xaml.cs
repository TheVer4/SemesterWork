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
            var res = DBController.Execute("SELECT * FROM WareHouse");
            InitializeComponent();
            printInvoice = new PrintInvoice();     
            LoginActivity();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";

            printInvoice.Print();
        }
    }
}