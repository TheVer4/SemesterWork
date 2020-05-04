using System;
using System.Text;
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
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";

            printInvoice.Print();
        }
    }
}