namespace SemesterWork
{
    public class StatisticsActivity : Activity
    {
        public StatisticsActivity(MainWindow window) : base(window)
        {
            new MainMenuActivity(Window);
        }
    }
}