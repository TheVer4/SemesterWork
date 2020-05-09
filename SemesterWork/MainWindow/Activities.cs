using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SemesterWork
{
    public partial class MainWindow
    {
        private DataGrid _positions;
        private List<CheckLine> _invoicePositions = new List<CheckLine>();
        private List<DBProductData> _savingPositions = new List<DBProductData>();
        private BarcodeReader _barcodeReader;
        private TextBox _number;
        private User _currentUser;
        private TextBox _barcodeForm;
        private TextBlock _total;
        private string _readBarcode;
        private DispatcherTimer _updateSmth;
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
            TextBox login = new TextBox() { FontSize = 20 };
            TextBox password = new TextBox() { FontSize = 20 };
            panel.Children.Add(new TextBlock() { Text = _lang["LoginActivity Account"], FontSize = 20 });
            panel.Children.Add(login);
            panel.Children.Add(new TextBlock() { Text = _lang["LoginActivity Password"], FontSize = 20 });
            password.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    Authorize(login.Text, password.Text);
            };
            panel.Children.Add(password);
            Button enter = new Button() { Content = "Авторизация", FontSize = 20 };
            Button close = new Button() { Content = "Выйти", FontSize = 20 };
            panel.Children.Add(enter);
            panel.Children.Add(close);
            enter.Click += (sender, args) => Authorize(login.Text, password.Text);
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
            Button fastInvoice = new Button() { Content = "Быстрый чек", Height = 50, FontSize = 20 };
            Button updateService = new Button() { Content = "Склад", Height = 50, FontSize = 20 };

            Button settings = new Button() { Content = "Настройки", Height = 50, FontSize = 20 };
            Button logout = new Button() { Content = "Деавторизация", Height = 50, FontSize = 20 };
            panel.Children.Add(fastInvoice);
            if (_currentUser.AccessLevel != "Normal")
                panel.Children.Add(updateService);
            if (_currentUser.AccessLevel == "Admin")
                panel.Children.Add(settings);
            panel.Children.Add(logout);
            fastInvoice.Click += (sender, args) => FastInvoiceActivity();
            updateService.Click += (sender, args) => WareHouseServiceActivity();
            settings.Click += (sender, args) => SettingsActivity();
            logout.Click += (sender, args) =>
            {
                _currentUser = null;
                LoginActivity();
            };
            Grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 1);
        }

        public void FastInvoiceActivity()
        {
            ClearScreen();
            _total = new TextBlock();
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
            TextBlock dateTime = new TextBlock() { Text = DateTime.Now.ToString(CultureInfo.CurrentCulture), TextAlignment = TextAlignment.Center, FontSize = 20 };
            TextBlock cashier = new TextBlock() { Text = $"Кассир: {_currentUser.Name} ", TextAlignment = TextAlignment.Right, FontSize = 20 };
            topBar.Children.Add(programName);
            Grid.SetColumn(programName, 0);
            topBar.Children.Add(dateTime);
            Grid.SetColumn(dateTime, 1);
            topBar.Children.Add(cashier);
            Grid.SetColumn(cashier, 2);
            DispatcherTimer dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Interval = TimeSpan.FromMilliseconds(1000);
            dateTimeTimer.Tick += (sender, args) => { dateTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture); };
            dateTimeTimer.Start();
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Grid barcodeInput = new Grid();
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            _barcodeForm = new TextBox() { FontSize = 48 };
            _barcodeForm.PreviewTextInput += NumberValidationTextBox;
            Button addPosition = new Button() { Content = "Добавить", FontSize = 48 };
            addPosition.Click += (sender, args) => AddPosition(_barcodeForm.Text);
            barcodeInput.Children.Add(_barcodeForm);
            Grid.SetColumn(addPosition, 1);
            barcodeInput.Children.Add(addPosition);
            invoiceControls.Children.Add(barcodeInput);
            
            _number = new TextBox() { FontSize = 48 };
            _number.PreviewTextInput += NumberValidationTextBox;
            Grid.SetColumn(_number, 1);
            invoiceControls.Children.Add(_number);
            Binding[] binds = {
                new Binding("Data.Name"),
                new Binding("Data.Price"),
                new Binding("Amount"),
                new Binding("Data.Units"),
                new Binding("FullPrice")
            };
            foreach (var bind in binds)
                bind.Mode = BindingMode.OneTime;
            _positions = new DataGrid()
            {
                ItemsSource = _invoicePositions,
                FontSize = 20, AutoGenerateColumns = false, Name = "CashierTable",
                Columns =
                {
                    new DataGridTextColumn() { Header = "Позиция", Binding = binds[0], MinWidth = 500 },
                    new DataGridTextColumn() { Header = "Цена", Binding = binds[1], MinWidth = 150 },
                    new DataGridTextColumn() { Header = "Кол-во", Binding = binds[2] },
                    new DataGridTextColumn() { Header = "Ед.", Binding = binds[3] },
                    new DataGridTextColumn() { Header = "Стоимость", Binding = binds[3], MinWidth = 200 }
                }
            };
            foreach (var column in _positions.Columns)
                column.CanUserSort = false;
            invoiceControls.Children.Add(_positions);
            Grid.SetRow(_positions, 1);
            
            StackPanel controls = new StackPanel();
            Grid keyboard = new Grid() { ShowGridLines = false };
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            for (int i = 8; i >= 0; i--)
            {
                Button key = new Button() { Content = (9 - i).ToString(), FontSize = 40, Height = 100 };
                key.Click += (sender, args) => _number.Text += ((Button) sender).Content.ToString();
                keyboard.Children.Add(key);
                Grid.SetColumn(key, 2 - i % 3 );
                Grid.SetRow(key, i / 3);
            }
            Button zero = new Button() { Content = "0", FontSize = 40, Height = 100 };
            zero.Click += (sender, args) => { _number.Text += "0"; };
            keyboard.Children.Add(zero);
            Grid.SetRow(zero, 3);
            Button dot = new Button() { Content = ".", FontSize = 40, Height = 100 };
            dot.Click += (sender, args) => { _number.Text += "."; };
            keyboard.Children.Add(dot);
            Grid.SetColumn(dot, 1);
            Grid.SetRow(dot, 3);
            Image crossImage = new Image() { Width = 50, Height = 50, Source = GetBitmapSource(@"images/cross.png") };
            Button clear = new Button() { Content = crossImage };
            clear.Click += ClearOnClick;
            keyboard.Children.Add(clear);
            Grid.SetColumn(clear, 2);
            Grid.SetRow(clear, 3);
            controls.Children.Add(keyboard);
            Button payment = new Button() { Content = "ОПЛАТА", FontSize = 40,  Height = 100 };
            payment.Click += PaymentOnClick;
            Button amount = new Button() { Content = "КОЛ", FontSize = 40, Height = 100 };
            amount.Click += AmountOnClick;
            Button storn = new Button() { Content = "ТЕСТ ЧЕГОНИТЬ", FontSize = 40,  Height = 100 };
            storn.Click += (sender, args) =>
                {

                };
            _total.Text = "ИТОГО: 0";
            _total.FontSize = 40;
            _total.Margin = new Thickness(15, 20, 0, 0);
            controls.Children.Add(payment);
            controls.Children.Add(amount);
            controls.Children.Add(storn);
            controls.Children.Add(_total);
            invoiceControls.Children.Add(controls);
            Grid.SetColumn(controls, 1);
            Grid.SetRow(controls, 1);

            _barcodeReader = new BarcodeReader(Variables.BarcodeScannerPort, 9600);
            _barcodeReader.AddReader(BarcodeRead);
            _updateSmth = new DispatcherTimer();
            _updateSmth.Interval = TimeSpan.FromMilliseconds(500);
            _updateSmth.Tick += (sender, args) =>
            {
                if (_readBarcode != null)
                    AddPosition(_readBarcode);
                _readBarcode = null;
            };
            _updateSmth.Start();          
        }
        
        public void WareHouseServiceActivity()
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
            TextBlock dateTime = new TextBlock() { Text = DateTime.Now.ToString(CultureInfo.CurrentCulture), TextAlignment = TextAlignment.Center, FontSize = 20 };
            TextBlock cashier = new TextBlock() { Text = $"Менеджер: {_currentUser.Name} ", TextAlignment = TextAlignment.Right, FontSize = 20 };
            topBar.Children.Add(programName);
            Grid.SetColumn(programName, 0);
            topBar.Children.Add(dateTime);
            Grid.SetColumn(dateTime, 1);
            topBar.Children.Add(cashier);
            Grid.SetColumn(cashier, 2);
            DispatcherTimer dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Interval = TimeSpan.FromMilliseconds(1000);
            dateTimeTimer.Tick += (sender, args) => { dateTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture); };
            dateTimeTimer.Start();
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Grid barcodeInput = new Grid();
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            _barcodeForm = new TextBox() { FontSize = 48 };
            _barcodeForm.PreviewTextInput += NumberValidationTextBox;
            Button addPosition = new Button() { Content = "Добавить", FontSize = 48 };
            addPosition.Click += (sender, args) => AddPositionForSaving(_barcodeForm.Text);
            barcodeInput.Children.Add(_barcodeForm);
            Grid.SetColumn(addPosition, 1);
            barcodeInput.Children.Add(addPosition);
            invoiceControls.Children.Add(barcodeInput);
            
            _number = new TextBox() { FontSize = 48 };
            _number.PreviewTextInput += NumberValidationTextBox;
            Grid.SetColumn(_number, 1);
            invoiceControls.Children.Add(_number);
            Binding[] binds = {
                new Binding("Data.EAN13"),
                new Binding("Data.Name"),
                new Binding("Data.Price"),
                new Binding("Data.Amount"),
                new Binding("Data.Units"),
                new Binding("Data.ShortName")
            };
            foreach (var bind in binds)
                bind.Mode = BindingMode.Default;
            _positions = new DataGrid()
            {
                ItemsSource = _savingPositions,
                FontSize = 20, AutoGenerateColumns = false, Name = "CashierTable",
                Columns =
                {
                    new DataGridTextColumn() { Header = "EAN13", Binding = binds[0], MinWidth = 250 },
                    new DataGridTextColumn() { Header = "Полное название", Binding = binds[1], MinWidth = 500 },
                    new DataGridTextColumn() { Header = "Цена", Binding = binds[2], MinWidth = 150},
                    new DataGridTextColumn() { Header = "Кол-во", Binding = binds[3] },
                    new DataGridTextColumn() { Header = "Единицы измерения", Binding = binds[4] },
                    new DataGridTextColumn() { Header = "Короткое название", Binding = binds[5] }
                }
            };
            foreach (var column in _positions.Columns)
                column.CanUserSort = false;
            invoiceControls.Children.Add(_positions);
            Grid.SetRow(_positions, 1);
            
            StackPanel controls = new StackPanel();
            Grid keyboard = new Grid() { ShowGridLines = false };
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.ColumnDefinitions.Add(new ColumnDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            keyboard.RowDefinitions.Add(new RowDefinition());
            for (int i = 8; i >= 0; i--)
            {
                Button key = new Button() { Content = (9 - i).ToString(), FontSize = 40, Height = 100 };
                key.Click += (sender, args) => _number.Text += ((Button) sender).Content.ToString();
                keyboard.Children.Add(key);
                Grid.SetColumn(key, 2 - i % 3 );
                Grid.SetRow(key, i / 3);
            }
            Button zero = new Button() { Content = "0", FontSize = 40, Height = 100 };
            zero.Click += (sender, args) => { _number.Text += "0"; };
            keyboard.Children.Add(zero);
            Grid.SetRow(zero, 3);
            Button dot = new Button() { Content = ".", FontSize = 40, Height = 100 };
            dot.Click += (sender, args) => { _number.Text += "."; };
            keyboard.Children.Add(dot);
            Grid.SetColumn(dot, 1);
            Grid.SetRow(dot, 3);
            Image crossImage = new Image() { Width = 50, Height = 50, Source = GetBitmapSource(@"images/cross.png") };
            Button clear = new Button() { Content = crossImage };
            clear.Click += ClearSavingOnClick;
            keyboard.Children.Add(clear);
            Grid.SetColumn(clear, 2);
            Grid.SetRow(clear, 3);
            controls.Children.Add(keyboard);
            Button saveButton = new Button() { Content = "Сохранить", FontSize = 40,  Height = 100 };
            saveButton.Click += (sender, args) => SavePositions();
            controls.Children.Add(saveButton);
            invoiceControls.Children.Add(controls);
            Grid.SetColumn(controls, 1);
            Grid.SetRow(controls, 1);

            _barcodeReader = new BarcodeReader(Variables.BarcodeScannerPort, 9600);
            _barcodeReader.AddReader(BarcodeRead);
            _updateSmth = new DispatcherTimer();
            _updateSmth.Interval = TimeSpan.FromMilliseconds(500);
            _updateSmth.Tick += (sender, args) =>
            {
                if (_readBarcode != null)
                    AddPositionForSaving(_readBarcode);
                _readBarcode = null;
            };
            _updateSmth.Start();          
        }

        public void SettingsActivity()
        {
            ClearScreen();
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            var panel = new StackPanel();

            var languageSet = new StackPanel();
            var languageTBlock = new TextBlock() { Text = "Язык", FontSize = 20 };
            var languageSelector = new ComboBox() { FontSize = 20 };
            languageSelector.ItemsSource = _lang.Languages.Select(language => new TextBlock() { Text = language, FontSize = 20 });
            
            languageSet.Children.Add(languageTBlock);
            languageSet.Children.Add(languageSelector);

            var scannerSet = new StackPanel();
            var scannerTBlock = new TextBlock() { Text = "Порт сканнера", FontSize = 20 };
            var scannerTBox = new TextBox() { Text = Variables.BarcodeScannerPort, FontSize = 20 };
            scannerSet.Children.Add(scannerTBlock);
            scannerSet.Children.Add(scannerTBox);

            var printerSet = new StackPanel();
            var printerTBlock = new TextBlock() { Text = "Сетевое имя принтера", FontSize = 20 };
            var printerTBox = new TextBox() { Text = Variables.PrinterName, FontSize = 20 };
            printerSet.Children.Add(printerTBlock);
            printerSet.Children.Add(printerTBox);

            var apply = new Button() { Content = "Применить", Height = 50, FontSize = 20 };
            var cancel = new Button() { Content = "Отмена", Height = 50, FontSize = 20 };

            panel.Children.Add(languageSet);
            panel.Children.Add(printerSet);
            panel.Children.Add(scannerSet);
            panel.Children.Add(apply);
            panel.Children.Add(cancel);

            apply.Click += (sender, args) =>
            {

            };
            cancel.Click += (sender, args) =>
            {
                if (MessageBox.Show("Вы уверены, что хотите выйти из настроек?", "Подтвердите действие", MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                    MainMenuActivity();
            };

            Grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 1);
        }

        public void ClearScreen()
        {
            _barcodeReader?.Dispose();
            _updateSmth?.Stop();
            Grid.Children.Clear();
            Grid.ColumnDefinitions.Clear();
            Grid.RowDefinitions.Clear();
        }

        private BitmapSource GetBitmapSource(string path)
        {
            Stream imageStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
    }
}