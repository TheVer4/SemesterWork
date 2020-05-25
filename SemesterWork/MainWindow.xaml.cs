namespace SemesterWork
{
    public partial class MainWindow
    {
        public MainWindow()
        {           
            InitializeComponent();
            Environment.Initialize();
            new LoginActivity(this);
            Closed += (sender, args) => Environment.Destroy();
        }
        
    }
}