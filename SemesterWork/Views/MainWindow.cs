using System.Windows.Controls;

namespace SemesterWork
{
    public partial class MainWindow
    {   
        public void ClearScreen()
        {
            EventHandler.StopScannerReceiver();
            Grid.Children.Clear();
            Grid.ColumnDefinitions.Clear();
            Grid.RowDefinitions.Clear();
        }
    }
}