using System.Windows;

namespace SemesterWork
{
    public partial class MainWindow
    {
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";
            printInvoice.Print();
        }
    }
}