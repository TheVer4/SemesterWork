using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SemesterWork
{
    public class SettingsActivity : Activity
    {
        public SettingsActivity(MainWindow window) : base(window)
        {
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });

            var panel = new StackPanel();
            var languageSet = new StackPanel();
            var languageTBlock = new TextBlock() { Text = LanguageEngine.Language["SettingsActivity Language"], FontSize = 20 };
            var languageSelector = new ComboBox() { FontSize = 20 };
            languageSelector.SelectedIndex = LanguageEngine.Languages.IndexOf(LanguageEngine.Current);
            languageSelector.ItemsSource = LanguageEngine.Languages.Select(language => new TextBlock() { Text = language, FontSize = 20 });
            
            languageSet.Children.Add(languageTBlock);
            languageSet.Children.Add(languageSelector);

            var scannerSet = new StackPanel();
            var scannerTBlock = new TextBlock() { Text = LanguageEngine.Language["SettingsActivity ScannerPort"], FontSize = 20 };
            var scannerTBox = new TextBox() { Text = Variables.BarcodeScannerPort, FontSize = 20 };
            scannerSet.Children.Add(scannerTBlock);
            scannerSet.Children.Add(scannerTBox);
            
            var printerSet = new StackPanel();
            var printerTBlock = new TextBlock() { Text = LanguageEngine.Language["SettingsActivity NetPrinterName"], FontSize = 20 };
            var printerTBox = new TextBox() { Text = Variables.PrinterPath.Substring(Variables.PrinterPath.LastIndexOf('\\') + 1, 
                Variables.PrinterPath.Length - Variables.PrinterPath.LastIndexOf('\\') - 1), FontSize = 20 };
            var printerNetTBlock = new TextBlock() 
                { Text = $"{LanguageEngine.Language["SettingsActivity NetPrinterAddress"]}: {Variables.PrinterPath}"};
            printerSet.Children.Add(printerTBlock);
            printerSet.Children.Add(printerTBox);
            printerSet.Children.Add(printerNetTBlock);

            var apply = new Button() { Content = LanguageEngine.Language["SettingsActivity Apply"], Height = 50, FontSize = 20 };
            var cancel = new Button() { Content = LanguageEngine.Language["SettingsActivity Cancel"], Height = 50, FontSize = 20 };

            panel.Children.Add(languageSet);
            panel.Children.Add(printerSet);
            panel.Children.Add(scannerSet);
            panel.Children.Add(apply);
            panel.Children.Add(cancel);

            string printerPath = Variables.PrinterPath;
            printerTBox.TextChanged += (sender, args) =>
            {
                var data = sender as TextBox;
                 printerPath = data.Text.Contains('\\') 
                    ? data.Text 
                    : @$"\\{Variables.MachineName}\{data.Text}";
                printerNetTBlock.Text = $@"{LanguageEngine.Language["SettingsActivity NetPrinterAddress"]}: {printerPath}";
            };

            string scanner = Variables.BarcodeScannerPort;
            scannerTBox.TextChanged += (sender, args) =>
            {
                var data = sender as TextBox;
                scanner = data.Text.StartsWith("COM") 
                    ? data.Text 
                    : "COM";
                data.Text = scanner;
                data.SelectionStart = data.Text.Length;
            };

            apply.Click += (sender, args) =>
            {
                LanguageEngine.Current = ((TextBlock) languageSelector.SelectedItem).Text;
                Variables.PrinterPath = printerPath;
                Variables.BarcodeScannerPort = scanner;
                EventHandler.IsSettingsOK = true;
                new SettingsActivity(Window);
                EventHandler.SaveSettings();
            };
            cancel.Click += (sender, args) =>
            {
                if (MessageBox.Show(LanguageEngine.Language["SettingsActivity CancelConfirm"],
                        LanguageEngine.Language["SettingsActivity CancelConfirmTitle"], MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                    new MainMenuActivity(Window);
            };

            Window.Grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 1);
        }
    }
}