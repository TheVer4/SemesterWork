using System.Windows;

namespace SemesterWork
{
    public class Activity
    {
        protected readonly MainWindow Window;

        public Activity(MainWindow window)
        {
            Window = window;
            Window.ClearScreen();
        }
        
    }
}