﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
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

            panel.Children.Add(new TextBlock() { Text = Lang["LoginActivity Account"], FontSize = 20 });
            panel.Children.Add(login);
            panel.Children.Add(new TextBlock() { Text = Lang["LoginActivity Password"], FontSize = 20 });
            password.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    Authorize(login.Text, password.Text);
            };
            panel.Children.Add(password);
            Button enter = new Button() { Content = Lang["LoginActivity Authorize"], FontSize = 20 };
            Button close = new Button() { Content = Lang["LoginActivity Exit"], FontSize = 20 };
            panel.Children.Add(enter);
            panel.Children.Add(close);
            enter.Click += (sender, args) => Authorize(login.Text, password.Text);
            close.Click += (sender, args) =>
            {
                switch (MessageBox.Show(
                    Lang["LoginActivity ExitMessageBox"],
                    Lang["LoginActivity ExitMessageBoxTitle"],
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
            Button fastInvoice = new Button() { Content = Lang["MainMenuActivity FastInvoice"], Height = 50, FontSize = 20 };
            Button updateService = new Button() { Content = Lang["MainMenuActivity WareHouse"], Height = 50, FontSize = 20 };
            Button settings = new Button() { Content = Lang["MainMenuActivity Settings"], Height = 50, FontSize = 20 };
            Button logout = new Button() { Content = Lang["MainMenuActivity Logout"], Height = 50, FontSize = 20 };

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

            TextBlock programName = new TextBlock() { Text = $" {Variables.ProgramName}", FontSize = 20 };
            TextBlock dateTime = new TextBlock() { Text = DateTime.Now.ToString(CultureInfo.CurrentCulture), TextAlignment = TextAlignment.Center, FontSize = 20 };
            TextBlock cashier = new TextBlock() { Text = $"{Lang["FastInvoiceActivity Cashier"]}: {_currentUser.Name} ", TextAlignment = TextAlignment.Right, FontSize = 20 };
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

            Button addPosition = new Button() { Content = Lang["FastInvoiceActivity AddPosition"], FontSize = 48 };
            addPosition.Click += (sender, args) => AddPosition(_barcodeForm.Text);
            barcodeInput.Children.Add(_barcodeForm);
            Grid.SetColumn(addPosition, 1);
            barcodeInput.Children.Add(addPosition);
            invoiceControls.Children.Add(barcodeInput);
            
            _number = new TextBox() { FontSize = 48 };
            _number.PreviewTextInput += NumberValidationTextBox;
            Grid.SetColumn(_number, 1);
            invoiceControls.Children.Add(_number);
            Binding[] binds = 
            {
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
                SelectionMode = DataGridSelectionMode.Single,
                FontSize = 20, AutoGenerateColumns = false, Name = "CashierTable",
                Columns =
                {
                    new DataGridTextColumn() { Header = Lang["FastInvoiceActivity Position"], Binding = binds[0], MinWidth = 500 },
                    new DataGridTextColumn() { Header = Lang["FastInvoiceActivity Price"], Binding = binds[1], MinWidth = 150 },
                    new DataGridTextColumn() { Header = Lang["FastInvoiceActivity Count"], Binding = binds[2] },
                    new DataGridTextColumn() { Header = Lang["FastInvoiceActivity Units"], Binding = binds[3] },
                    new DataGridTextColumn() { Header = Lang["FastInvoiceActivity FullPrice"], Binding = binds[4], MinWidth = 200 }
                }
            };
            foreach (var column in _positions.Columns) {
                column.CanUserSort = false;
                column.IsReadOnly = true;
            }
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
            Button payment = new Button() { Content = Lang["FastInvoiceActivity Payment"], FontSize = 40,  Height = 100 };
            payment.Click += PaymentOnClick;
            Button amount = new Button() { Content = Lang["FastInvoiceActivity Amount"], FontSize = 40, Height = 100 };
            amount.Click += AmountOnClick;

            _total.Text = $"{Lang["FastInvoiceActivity Total"]}: 0";
            _total.FontSize = 40;
            _total.Margin = new Thickness(15, 20, 0, 0);
            controls.Children.Add(payment);
            controls.Children.Add(amount);
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

            TextBlock programName = new TextBlock() { Text = $" {Variables.ProgramName}", FontSize = 20 };
            TextBlock dateTime = new TextBlock() { Text = DateTime.Now.ToString(CultureInfo.CurrentCulture), TextAlignment = TextAlignment.Center, FontSize = 20 };
            TextBlock cashier = new TextBlock() { Text = $"{Lang["WareHouseActivity Manager"]}: {_currentUser.Name} ", TextAlignment = TextAlignment.Right, FontSize = 20 };
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

            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition());
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.25, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });

            Grid barcodeInput = new Grid();
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            _barcodeForm = new TextBox() { FontSize = 48 };
            _barcodeForm.PreviewTextInput += NumberValidationTextBox;

            Button addPosition = new Button() { Content = Lang["WareHouseActivity AddPosition"], FontSize = 48 };
            addPosition.Click += (sender, args) => AddPositionForSaving(_barcodeForm.Text);
            barcodeInput.Children.Add(_barcodeForm);
            Grid.SetColumn(addPosition, 1);
            barcodeInput.Children.Add(addPosition);
            invoiceControls.Children.Add(barcodeInput);
            
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
                SelectionMode = DataGridSelectionMode.Single,
                FontSize = 20, AutoGenerateColumns = false, Name = "CashierTable",
                Columns =
                {
                    new DataGridTextColumn() { Header = Lang["WareHouseActivity EAN13"], Binding = binds[0], MinWidth = 250 },
                    new DataGridTextColumn() { Header = Lang["WareHouseActivity FullName"], Binding = binds[1], MinWidth = 500 },
                    new DataGridTextColumn() { Header = Lang["WareHouseActivity Price"], Binding = binds[2], MinWidth = 150},
                    new DataGridTextColumn() { Header = Lang["WareHouseActivity Amount"], Binding = binds[3] },
                    new DataGridComboBoxColumn() { Header = Lang["WareHouseActivity Units"], TextBinding = binds[4], ItemsSource = new List<string>() {"шт.", "кг."}},
                    new DataGridTextColumn() { Header = Lang["WareHouseActivity ShortName"], Binding = binds[5] }
                }
            };
            foreach (var column in _positions.Columns)
                column.CanUserSort = false;
            invoiceControls.Children.Add(_positions);
            Grid.SetRow(_positions, 1);
            
            Grid controls = new Grid();
            controls.ColumnDefinitions.Add(new ColumnDefinition());
            controls.ColumnDefinitions.Add(new ColumnDefinition());
            controls.ColumnDefinitions.Add(new ColumnDefinition());

            Image crossImage = new Image() { Width = 50, Height = 50, Source = GetBitmapSource(@"images/cross.png") };
            Button clear = new Button() { Content = crossImage, Height = 100 };
            clear.Click += (sender, args) => ClearSavingOnClick();
            controls.Children.Add(clear);
            Grid.SetColumn(clear, 0);

            var deleteButton = new Button() { Content = Lang["WareHouseActivity DeleteFromDBButton"], FontSize = 40, Height = 100 };
            deleteButton.Click += (sender, args) => DeleteFromDB();
            controls.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 1);

            Button saveButton = new Button() { Content = Lang["WareHouseActivity Save"], FontSize = 40,  Height = 100 };
            saveButton.Click += (sender, args) => SavePositions();
            controls.Children.Add(saveButton);
            Grid.SetColumn(saveButton, 2);

            invoiceControls.Children.Add(controls);
            Grid.SetRow(controls, 2);

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
            var languageTBlock = new TextBlock() { Text = Lang["SettingsActivity Language"], FontSize = 20 };
            var languageSelector = new ComboBox() { FontSize = 20 };
            languageSelector.SelectedIndex = Lang.Languages.IndexOf(LanguageEngine.Current);
            languageSelector.ItemsSource = Lang.Languages.Select(language => new TextBlock() { Text = language, FontSize = 20 });
            
            languageSet.Children.Add(languageTBlock);
            languageSet.Children.Add(languageSelector);

            var scannerSet = new StackPanel();
            var scannerTBlock = new TextBlock() { Text = Lang["SettingsActivity ScannerPort"], FontSize = 20 };
            var scannerTBox = new TextBox() { Text = Variables.BarcodeScannerPort, FontSize = 20 };
            scannerSet.Children.Add(scannerTBlock);
            scannerSet.Children.Add(scannerTBox);
            
            var printerSet = new StackPanel();
            var printerTBlock = new TextBlock() { Text = Lang["SettingsActivity NetPrinterName"], FontSize = 20 };
            var printerTBox = new TextBox() { Text = Variables.PrinterPath.Substring(Variables.PrinterPath.LastIndexOf('\\') + 1, 
                Variables.PrinterPath.Length - Variables.PrinterPath.LastIndexOf('\\') - 1), FontSize = 20 };
            var printerNetTBlock = new TextBlock() { Text = $"{Lang["SettingsActivity NetPrinterAddress"]}: {Variables.PrinterPath}"};
            printerSet.Children.Add(printerTBlock);
            printerSet.Children.Add(printerTBox);
            printerSet.Children.Add(printerNetTBlock);

            var apply = new Button() { Content = Lang["SettingsActivity Apply"], Height = 50, FontSize = 20 };
            var cancel = new Button() { Content = Lang["SettingsActivity Cancel"], Height = 50, FontSize = 20 };

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
                printerNetTBlock.Text = $@"{Lang["SettingsActivity NetPrinterAddress"]}: {printerPath}";
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
                SettingsActivity();
                SaveSettings();
            };
            cancel.Click += (sender, args) =>
            {
                if (MessageBox.Show(Lang["SettingsActivity CancelConfirm"], Lang["SettingsActivity CancelConfirmTitle"], MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                    MainMenuActivity();
            };

            Grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 1);
        }
    }
}