using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SemesterWork
{
    public partial class MainWindow
    {
        private List<CheckLine> invoicePositions = new List<CheckLine>();
        public void LoginActivity()
        {
            ClearScreen();
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(3, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            TextBlock logo = new TextBlock() { Text = Variables.ProgramName, FontSize = 72, TextAlignment = TextAlignment.Center };
            Grid.Children.Add(logo);
            Grid.SetColumnSpan(logo, 3);
            Grid.SetColumn(logo, 1);
            StackPanel panel = new StackPanel();
            TextBox login = new TextBox() {};
            TextBox password = new TextBox() {};
            panel.Children.Add(new TextBlock() { Text = "Account" });
            panel.Children.Add(login);
            panel.Children.Add(new TextBlock() { Text = "Password" });
            panel.Children.Add(password);
            Button enter = new Button() { Content = "Авторизация" };
            Button close = new Button() { Content = "Выйти" };
            panel.Children.Add(enter);
            panel.Children.Add(close);
            enter.Click += (sender, args) => MainMenuActivity();
            close.Click += (sender, args) =>
            {
                switch (MessageBox.Show(
                    "Вы действительно хотите выйти?",
                    "Подтвердите действие",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        Close();
                        break;
                    case MessageBoxResult.No:
                        break;
                }
            };
            Grid.Children.Add(panel);
            Grid.SetColumn(panel, 2);
            Grid.SetRow(panel, 1);
        }

        public void MainMenuActivity()
        {
            ClearScreen();
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            StackPanel panel = new StackPanel();
            Button fastInvoice = new Button() { Content = "Быстрый чек", Height = 50 };
            Button settings = new Button() { Content = "Настройки", Height = 50 };
            Button logout = new Button() { Content = "Деавторизация", Height = 50 };
            panel.Children.Add(fastInvoice);
            panel.Children.Add(settings);
            panel.Children.Add(logout);
            fastInvoice.Click += (sender, args) => FastInvoiceActivity();
            settings.Click += (sender, args) => { };
            logout.Click += (sender, args) => LoginActivity();
            Grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 1);

        }

        public void FastInvoiceActivity()
        {
            ClearScreen();
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32, GridUnitType.Star) });
            Grid topBar = new Grid();
            Grid.Children.Add(topBar);
            Grid.SetRow(topBar, 0);
            Grid invoiceControls = new Grid();
            Grid.Children.Add(invoiceControls);
            Grid.SetRow(invoiceControls, 1);
            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            TextBlock programName = new TextBlock() { Text = Variables.ProgramName, FontSize = 20 };
            TextBlock dateTime = new TextBlock() { Text = DateTime.Now.ToString(), TextAlignment = TextAlignment.Center, FontSize = 20 };
            TextBlock cashier = new TextBlock() { Text = "Кассир: Мадам Брошкина ", TextAlignment = TextAlignment.Right, FontSize = 20 };
            topBar.Children.Add(programName);
            Grid.SetColumn(programName, 0);
            topBar.Children.Add(dateTime);
            Grid.SetColumn(dateTime, 1);
            topBar.Children.Add(cashier);
            Grid.SetColumn(cashier, 2);
            DispatcherTimer dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Interval = TimeSpan.FromMilliseconds(1000);
            dateTimeTimer.Tick += (sender, args) => { dateTime.Text = DateTime.Now.ToString(); };
            dateTimeTimer.Start();
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Grid barcodeInput = new Grid();
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            TextBox barcodeForm = new TextBox() { FontSize = 48 };
            Button addPosition = new Button() { Content = "Добавить", FontSize = 48 };
            barcodeInput.Children.Add(barcodeForm);
            Grid.SetColumn(addPosition, 1);
            barcodeInput.Children.Add(addPosition);
            invoiceControls.Children.Add(barcodeInput);
            
            Grid numberField = new Grid();
            numberField.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            numberField.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            TextBox number = new TextBox() { FontSize = 48 };
            Image crossImage = new Image() { Width = 50, Height = 50, Source = getBitmapSource(@"images/cross.png") };
            Button clear = new Button() { Content = crossImage };
            numberField.Children.Add(number);
            numberField.Children.Add(clear);
            Grid.SetColumn(clear, 1);
            Grid.SetColumn(numberField, 1);
            invoiceControls.Children.Add(numberField);
            DataGrid positions = new DataGrid()
            {
                ItemsSource = invoicePositions,
                FontSize = 20, AutoGenerateColumns = false, Name = "CashierTable",
                Columns =
                {
                    new DataGridTextColumn() { Header = "Позиция", Binding = new Binding("Name"), MinWidth = 500 },
                    new DataGridTextColumn() { Header = "Цена", Binding = new Binding("Price") },
                    new DataGridTextColumn() { Header = "Кол-во", Binding = new Binding("Amount") },
                    new DataGridTextColumn() { Header = "Стоимость", Binding = new Binding("FullPrice") }
                }
            };
            invoiceControls.Children.Add(positions);
            Grid.SetRow(positions, 1);
            
            StackPanel controls = new StackPanel();
            Grid keyboard = new Grid() { ShowGridLines = false };
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            for (int i = 9; i >= 0; i--)
            {
                Button key = new Button() { Content = (9 - i).ToString(), FontSize = 40, Height = 100 };
                keyboard.Children.Add(key);
                Grid.SetColumn(key, 2 - i % 3 );
                Grid.SetRow(key, i / 3);
            }
            controls.Children.Add(keyboard);
            Button payment = new Button() { Content = "Оплата", FontSize = 40,  Height = 100 };
            Button amount = new Button() { Content = "Кол", FontSize = 40, Height = 100 };
            Button storn = new Button() { Content = "Сторно", FontSize = 40,  Height = 100 };
            TextBlock total = new TextBlock() { Text = "ИТОГО: 0", FontSize = 40, Margin = new Thickness(15, 20, 0, 0) };
            controls.Children.Add(payment);
            controls.Children.Add(amount);
            controls.Children.Add(storn);
            controls.Children.Add(total);
            invoiceControls.Children.Add(controls);
            Grid.SetColumn(controls, 1);
            Grid.SetRow(controls, 1);
        }
        
        public void ClearScreen()
        {
            Grid.Children.Clear();
            Grid.ColumnDefinitions.Clear();
            Grid.RowDefinitions.Clear();
        }

        private BitmapSource getBitmapSource(string path)
        {
            Stream imageStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
    }
}