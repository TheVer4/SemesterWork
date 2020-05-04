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
            InitializeComponent();
            printInvoice = new PrintInvoice();
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";
            LoginActivity();
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            printInvoice.Print();
        }
    }
}