using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SemesterWork
{
    public class LoginActivity : Activity
    {
        public LoginActivity(MainWindow window) : base(window)
        {
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });

            TextBlock logo = new TextBlock() { Text = Variables.ProgramName, FontSize = 72, TextAlignment = TextAlignment.Center };
            Window.Grid.Children.Add(logo);
            Grid.SetColumnSpan(logo, 3);
            Grid.SetColumn(logo, 1);

            StackPanel panel = new StackPanel();
            TextBox login = new TextBox() { FontSize = 20 };
            PasswordBox password = new PasswordBox() { FontSize = 20 };

            panel.Children.Add(new TextBlock() { Text = LanguageEngine.Language["LoginActivity Account"], FontSize = 20 });
            panel.Children.Add(login);
            panel.Children.Add(new TextBlock() { Text = LanguageEngine.Language["LoginActivity Password"], FontSize = 20 });
            password.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    Authorize(login.Text, password.Password);
            };
            panel.Children.Add(password);
            Button enter = new Button() { Content = LanguageEngine.Language["LoginActivity Authorize"], FontSize = 20 };
            Button close = new Button() { Content = LanguageEngine.Language["LoginActivity Exit"], FontSize = 20 };
            panel.Children.Add(enter);
            panel.Children.Add(close);
            enter.Click += (sender, args) => Authorize(login.Text, password.Password);
            close.Click += (sender, args) =>
            {
                switch (MessageBox.Show(
                    LanguageEngine.Language["LoginActivity ExitMessageBox"],
                    LanguageEngine.Language["LoginActivity ExitMessageBoxTitle"],
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        Window.Close();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            };
            Window.Grid.Children.Add(panel);
            Grid.SetColumn(panel, 2);
            Grid.SetRow(panel, 1);
        }

        private void Authorize(string login, string password)
        {
            var worker = new BackgroundWorker();
            var isAuthorized = false;
            worker.DoWork += (sender, args) => isAuthorized = EventHandler.Authorize(login, password);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                if (isAuthorized)
                    new MainMenuActivity(Window);
            };
            worker.RunWorkerAsync();
        }
    } 
}