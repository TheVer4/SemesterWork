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
            //var a = new CheckLine("1111111111111", 2);
            //DBController.Insert("3213213213213", "Несквик с пивом", 228.0, 420.0, "шт.", "Нескв. с пивом");
            //DBController.Update("3213213213213", "Amount", 421.0);
            //DBController.Remove("3213213213213");
            //DBController.Insert("1111111111111", "Яблоки", 57.87, 455.34, "кг.", "Яблоки");
            //DBController.DecreaseAmountBy("1111111111111", 3.45);
            //DBController.IncreaseAmountBy("1111111111111", 3.45);
            //DBController.AddUser("1234567890", "Абдулах Мед", "Assистент");
            //var b = DBController.FindUser("1234567890");
            //DBController.UpdateUser("1234567890", "AccessLevel", "NotAssистент");
            //DBController.RemoveUser("1234567890");
            InitializeComponent();
            printInvoice = new PrintInvoice();
            //FastInvoiceActivity();
            LoginActivity();
        }
    }
}