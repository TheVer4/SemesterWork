namespace SemesterWork
{
    public abstract class Activity
    {
        protected readonly MainWindow Window;

        public Activity(MainWindow window)
        {
            Window = window;
            Window.ClearScreen();
        }        
    }
}