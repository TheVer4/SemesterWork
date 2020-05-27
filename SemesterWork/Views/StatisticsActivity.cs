using System.Windows.Controls;

namespace SemesterWork
{
    public class StatisticsActivity : Activity
    {
        public StatisticsActivity(MainWindow window) : base(window)
        {
            Grid grid = new Grid(); //TODO statistics
            new MainMenuActivity(Window);
        }
    }
}