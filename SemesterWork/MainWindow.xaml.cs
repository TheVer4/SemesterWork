using System.IO.Ports;
using System.Windows;

namespace SemesterWork
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            CheckLine a;
            try { a = new CheckLine("123", 2); } 
            catch { }
            InitializeComponent();
            InitializeEnvironment();
        }
    }
}