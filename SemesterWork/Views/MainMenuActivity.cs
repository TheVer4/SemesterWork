using System.Windows;
using System.Windows.Controls;

namespace SemesterWork
{
    public class MainMenuActivity : Activity
    {
        public MainMenuActivity(MainWindow window) : base(window)
        {
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });

            StackPanel panel = new StackPanel();
            Button fastInvoice = new Button() { Content = LanguageEngine.Language["MainMenuActivity FastInvoice"], Height = 50, FontSize = 20 };
            Button warehouse = new Button() { Content = LanguageEngine.Language["MainMenuActivity WareHouse"], Height = 50, FontSize = 20 };
            Button statistics = new Button() { Content = "Статистика", Height = 50, FontSize = 20 }; //TODO localize
            Button userControlService = new Button() { Content = "Менеджер аккаунтов", Height = 50, FontSize = 20 }; //TODO localize
            Button settings = new Button() { Content = LanguageEngine.Language["MainMenuActivity Settings"], Height = 50, FontSize = 20 };
            Button logout = new Button() { Content = LanguageEngine.Language["MainMenuActivity Logout"], Height = 50, FontSize = 20 };

            panel.Children.Add(fastInvoice);
            if (EventHandler.CurrentUser.AccessLevel != "Normal")
            {
                panel.Children.Add(warehouse);
                panel.Children.Add(statistics);
            }
            if (EventHandler.CurrentUser.AccessLevel == "Admin")
            {
                panel.Children.Add(userControlService);
                panel.Children.Add(settings);
            }
            panel.Children.Add(logout);
            fastInvoice.Click += (sender, args) => new FastInvoiceActivity(Window);
            warehouse.Click += (sender, args) => new WareHouseServiceActivity(Window);
            statistics.Click += (sender, args) => new StatisticsActivity(Window);
            userControlService.Click += (sender, args) => new UserControlServiceActivity(Window);
            settings.Click += (sender, args) => new SettingsActivity(Window);
            logout.Click += (sender, args) =>
            {
                EventHandler.Logout();
                new LoginActivity(Window);
            };
            Window.Grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 1);

            if (!EventHandler.IsSettingsOK)
                MessageBox.Show("An error occurred while loading the settings. Default settings were set.",  // не локализовано, потому что может появиться
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);                                // только с дефолтной (English) локализацией       
        }
    }
}