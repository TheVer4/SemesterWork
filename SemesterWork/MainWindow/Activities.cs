using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
        private TextBox _number;
        private TextBox _textForm;
        private TextBlock _total;

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
            fastInvoice.Click += (sender, args) => FastInvoiceActivity();
            warehouse.Click += (sender, args) => WareHouseServiceActivity();
            statistics.Click += (sender, args) => StatisticsActivity();
            userControlService.Click += (sender, args) => UserControlServiceActivity();
            settings.Click += (sender, args) => SettingsActivity();
            logout.Click += (sender, args) =>
            {
                EventHandler.Logout();
                LoginActivity();
            };
            Grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 1);

            if (!EventHandler.IsSettingsOK)
                MessageBox.Show("An error occurred while loading the settings. Default settings were set.",  // не локализовано, потому что может появиться
                    "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);                                // только с дефолтной (English) локализацией
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
            TextBlock cashier = new TextBlock() { Text = $"{LanguageEngine.Language["FastInvoiceActivity Cashier"]}: {EventHandler.CurrentUser.Name} ", TextAlignment = TextAlignment.Right, FontSize = 20 };
            topBar.Children.Add(programName);
            Grid.SetColumn(programName, 0);
            topBar.Children.Add(dateTime);
            Grid.SetColumn(dateTime, 1);
            topBar.Children.Add(cashier);
            Grid.SetColumn(cashier, 2);

            InitClock(dateTime);

            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });

            Grid barcodeInput = new Grid();
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            _textForm = new TextBox() { FontSize = 48 };
            _textForm.PreviewTextInput += NumberValidationTextBox;
            _number = new TextBox() { FontSize = 48 };

            _textForm.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    AddPosition(EventHandler.AddPosition, _textForm.Text, _number.Text);
            };

            Button addPosition = new Button() { Content = LanguageEngine.Language["FastInvoiceActivity AddPosition"], FontSize = 48 };
            addPosition.Click += (sender, args) => AddPosition(EventHandler.AddPosition, _textForm.Text, _number.Text);

            barcodeInput.Children.Add(_textForm);
            Grid.SetColumn(addPosition, 1);
            barcodeInput.Children.Add(addPosition);
            invoiceControls.Children.Add(barcodeInput);


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
                ItemsSource = EventHandler.ItemsPositions,
                SelectionMode = DataGridSelectionMode.Single,
                FontSize = 20, 
                AutoGenerateColumns = false,
                Columns =
                {
                    new DataGridTextColumn() { Header = LanguageEngine.Language["FastInvoiceActivity Position"], Binding = binds[0], MinWidth = 500 },
                    new DataGridTextColumn() { Header = LanguageEngine.Language["FastInvoiceActivity Price"], Binding = binds[1], MinWidth = 150 },
                    new DataGridTextColumn() { Header = LanguageEngine.Language["FastInvoiceActivity Count"], Binding = binds[2] },
                    new DataGridTextColumn() { Header = LanguageEngine.Language["FastInvoiceActivity Units"], Binding = binds[3] },
                    new DataGridTextColumn() { Header = LanguageEngine.Language["FastInvoiceActivity FullPrice"], Binding = binds[4], MinWidth = 200 }
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
                key.Click += (sender, args) => _number.Text += ((Button)sender).Content.ToString();
                keyboard.Children.Add(key);
                Grid.SetColumn(key, 2 - i % 3);
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

            Image crossImage = new Image() { Width = 50, Height = 50 };
            BitmapSource source = null;
            Button clear = new Button();
            var imageSourceWorker = new BackgroundWorker();
            imageSourceWorker.DoWork += (sender, args) =>
            {
                source = GetBitmapSource(@"images/cross.png");
            };
            imageSourceWorker.RunWorkerCompleted += (sender, args) =>
            {
                crossImage.Source = source;
                clear.Content = crossImage;
            };
            imageSourceWorker.RunWorkerAsync();
            clear.Click += (sender, args) => ClearOnClick();
            keyboard.Children.Add(clear);
            Grid.SetColumn(clear, 2);
            Grid.SetRow(clear, 3);

            controls.Children.Add(keyboard);
            Button payment = new Button() { Content = LanguageEngine.Language["FastInvoiceActivity Payment"], FontSize = 40, Height = 100 };
            payment.Click += (sender, args) => PaymentOnClick();
            Button amount = new Button() { Content = LanguageEngine.Language["FastInvoiceActivity Amount"], FontSize = 40, Height = 100 };
            amount.Click += (sender, args) =>
            {
                var numberText = _number.Text;
                var selectedIndex = _positions.SelectedIndex;
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, args) => EventHandler.AmountOnClick(numberText, selectedIndex);
                worker.RunWorkerCompleted += (sender, args) => UpdateScreen();
                worker.RunWorkerAsync();
            };
            _total.Text = $"{LanguageEngine.Language["FastInvoiceActivity Total"]}: 0";
            _total.FontSize = 40;
            _total.Margin = new Thickness(15, 20, 0, 0);
            controls.Children.Add(payment);
            controls.Children.Add(amount);
            controls.Children.Add(_total);
            invoiceControls.Children.Add(controls);
            Grid.SetColumn(controls, 1);
            Grid.SetRow(controls, 1);

            EventHandler.StartScannerReceiver(AddPosition, EventHandler.AddPosition,  _number.Text);
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
            TextBlock manager = new TextBlock() { Text = $"{LanguageEngine.Language["WareHouseActivity Manager"]}: {EventHandler.CurrentUser.Name} ", TextAlignment = TextAlignment.Right, FontSize = 20 };
            topBar.Children.Add(programName);
            Grid.SetColumn(programName, 0);
            topBar.Children.Add(dateTime);
            Grid.SetColumn(dateTime, 1);
            topBar.Children.Add(manager);
            Grid.SetColumn(manager, 2);
            
            InitClock(dateTime);

            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition());
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.25, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });

            Grid barcodeInput = new Grid();
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            _textForm = new TextBox() { FontSize = 48 };
            _textForm.PreviewTextInput += NumberValidationTextBox;
            _textForm.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    AddPosition((a, b) => EventHandler.AddPositionForSaving(a), _textForm.Text, null);
            };

            Button addPosition = new Button() { Content = LanguageEngine.Language["WareHouseActivity AddPosition"], FontSize = 48 };
            addPosition.Click += (sender, args) => AddPosition((a, b) => EventHandler.AddPositionForSaving(a), _textForm.Text, null);
            barcodeInput.Children.Add(_textForm);
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
                ItemsSource = EventHandler.ItemsPositions,
                SelectionMode = DataGridSelectionMode.Single,
                FontSize = 20, 
                AutoGenerateColumns = false,
                Columns =
                {
                    new DataGridTextColumn() { Header = LanguageEngine.Language["WareHouseActivity EAN13"], Binding = binds[0], MinWidth = 250 },
                    new DataGridTextColumn() { Header = LanguageEngine.Language["WareHouseActivity FullName"], Binding = binds[1], MinWidth = 500 },
                    new DataGridTextColumn() { Header = LanguageEngine.Language["WareHouseActivity Price"], Binding = binds[2], MinWidth = 150},
                    new DataGridTextColumn() { Header = LanguageEngine.Language["WareHouseActivity Amount"], Binding = binds[3] },
                    new DataGridComboBoxColumn() { Header = LanguageEngine.Language["WareHouseActivity Units"], TextBinding = binds[4], ItemsSource = new List<string>() {"шт.", "кг."}},
                    new DataGridTextColumn() { Header = LanguageEngine.Language["WareHouseActivity ShortName"], Binding = binds[5] }
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

            Image crossImage = new Image() { Width = 50, Height = 50 };
            var worker = new BackgroundWorker();
            BitmapSource source = null;
            Button clear = new Button() { Height = 100 };
            worker.DoWork += (sender, args) =>
            {
                source = GetBitmapSource(@"images/cross.png");
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                crossImage.Source = source;
                clear.Content = crossImage;
            };
            worker.RunWorkerAsync();
            clear.Click += (sender, args) => ClearOnClick();
            controls.Children.Add(clear);
            Grid.SetColumn(clear, 0);

            var deleteButton = new Button() { Content = LanguageEngine.Language["WareHouseActivity DeleteFromDBButton"], FontSize = 40, Height = 100 };
            deleteButton.Click += (sender, args) =>
            {
                var selectedIndex = _positions.SelectedIndex;
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, args) => EventHandler.DeleteFromDB(selectedIndex);
                worker.RunWorkerCompleted += (sender, args) => UpdateScreen();
                worker.RunWorkerAsync();
            };
            controls.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 1);

            Button saveButton = new Button() { Content = LanguageEngine.Language["WareHouseActivity Save"], FontSize = 40, Height = 100 };
            saveButton.Click += (sender, args) =>
            {
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, args) => EventHandler.SavePositionsForWarehouse();
                worker.RunWorkerCompleted += (sender, args) =>
                    MessageBox.Show(LanguageEngine.Language["WareHouseActivity SaveMessageBox"],
                        LanguageEngine.Language["WareHouseActivity SaveMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
                worker.RunWorkerAsync();
            };
            controls.Children.Add(saveButton);
            Grid.SetColumn(saveButton, 2);

            invoiceControls.Children.Add(controls);
            Grid.SetRow(controls, 2);

            EventHandler.StartScannerReceiver(AddPosition, (a, b) => EventHandler.AddPositionForSaving(a));
        }
        
        public void StatisticsActivity()
        {
            
        }

        public void UserControlServiceActivity()
        {
            ClearScreen();

            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32, GridUnitType.Star) });

            Grid topBar = new Grid();
            Grid.Children.Add(topBar);
            Grid.SetRow(topBar, 0);
            Grid userControls = new Grid();
            Grid.Children.Add(userControls);
            Grid.SetRow(userControls, 1);
            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            topBar.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock programName = new TextBlock() { Text = $" {Variables.ProgramName}", FontSize = 20 };
            TextBlock dateTime = new TextBlock() { Text = DateTime.Now.ToString(CultureInfo.CurrentCulture), TextAlignment = TextAlignment.Center, FontSize = 20 };
            TextBlock admin = new TextBlock() { Text = $"Администратор: {EventHandler.CurrentUser.Name} ", TextAlignment = TextAlignment.Right, FontSize = 20 }; //TODO localize
            topBar.Children.Add(programName);
            Grid.SetColumn(programName, 0);
            topBar.Children.Add(dateTime);
            Grid.SetColumn(dateTime, 1);
            topBar.Children.Add(admin);
            Grid.SetColumn(admin, 2);
            
            InitClock(dateTime);

            userControls.ColumnDefinitions.Add(new ColumnDefinition());
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.25, GridUnitType.Star) });
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });

            Grid userInput = new Grid();
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.5, GridUnitType.Star) });
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.5, GridUnitType.Star) });
            _textForm = new TextBox() { FontSize = 48 };
            _textForm.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    AddPosition((a,b) => EventHandler.AddUserPosition(a), _textForm.Text, null);
            };

            var findUser = new Button() { Content = "Найти", FontSize = 48 }; //TODO localize
            Button addNewUser = new Button() { Content = "Добавить", FontSize = 48 }; //TODO localize
            findUser.Click += (sender, args) => AddPosition((a,b) => EventHandler.AddUserPosition(a), _textForm.Text, null);
            addNewUser.Click += (sender, args) => EventHandler.AddNewUser();
            userInput.Children.Add(_textForm);
            Grid.SetColumn(userInput, 0);
            userInput.Children.Add(findUser);
            Grid.SetColumn(findUser, 1);
            userInput.Children.Add(addNewUser);
            Grid.SetColumn(addNewUser, 2);
            userControls.Children.Add(userInput);

            Binding[] binds = {
                new Binding("Id") { Mode = BindingMode.OneTime },
                new Binding("Name") { Mode = BindingMode.Default },
                new Binding("AccessLevel") { Mode = BindingMode.Default }
            };
            _positions = new DataGrid()
            {
                ItemsSource = EventHandler.ItemsPositions,
                SelectionMode = DataGridSelectionMode.Single,
                FontSize = 20,
                AutoGenerateColumns = false,
                Columns =
                {
                    new DataGridTextColumn() { Header = "id", Binding = binds[0], MinWidth = 200 }, //TODO localize
                    new DataGridTextColumn() { Header = "Имя", Binding = binds[1], MinWidth = 750 }, //TODO localize
                    new DataGridComboBoxColumn() { Header = "Уровень доступа" , TextBinding = binds[2], MinWidth = 500, ItemsSource = new List<string> { "Normal", "Manager", "Admin" } }, //TODO localize
                }
            };
            foreach (var column in _positions.Columns)
                column.CanUserSort = false;
            userControls.Children.Add(_positions);
            Grid.SetRow(_positions, 1);

            Grid controls = new Grid();
            controls.ColumnDefinitions.Add(new ColumnDefinition());
            controls.ColumnDefinitions.Add(new ColumnDefinition());
            controls.ColumnDefinitions.Add(new ColumnDefinition());

            Image crossImage = new Image() { Width = 50, Height = 50 };
            var worker = new BackgroundWorker();
            BitmapSource source = null;
            Button clear = new Button() { Height = 100 };
            worker.DoWork += (sender, args) =>
            {
                source = GetBitmapSource(@"images/cross.png");
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                crossImage.Source = source;
                clear.Content = crossImage;
            };
            worker.RunWorkerAsync();
            clear.Click += (sender, args) => ClearOnClick();
            controls.Children.Add(clear);
            Grid.SetColumn(clear, 0);

            var deleteButton = new Button() { Content = "Удалить пользователя", FontSize = 40, Height = 100 }; //TODO localize
            deleteButton.Click += (sender, args) =>
            {
                var selectedIndex = _positions.SelectedIndex;
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, args) => EventHandler.DeleteUserFromDB(selectedIndex);
                worker.RunWorkerCompleted += (sender, args) => UpdateScreen();
                worker.RunWorkerAsync();
            };
            controls.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 1);

            Button saveButton = new Button() { Content = "Сохранить", FontSize = 40, Height = 100 }; //TODO localize
            saveButton.Click += (sender, args) => EventHandler.SaveUsersPositions();
            controls.Children.Add(saveButton);
            Grid.SetColumn(saveButton, 2);

            userControls.Children.Add(controls);
            Grid.SetRow(controls, 2);
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
            var printerNetTBlock = new TextBlock() { Text = $"{LanguageEngine.Language["SettingsActivity NetPrinterAddress"]}: {Variables.PrinterPath}"};
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
                SettingsActivity();
                EventHandler.SaveSettings();
            };
            cancel.Click += (sender, args) =>
            {
                if (MessageBox.Show(LanguageEngine.Language["SettingsActivity CancelConfirm"], LanguageEngine.Language["SettingsActivity CancelConfirmTitle"], MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                    MainMenuActivity();
            };

            Grid.Children.Add(panel);
            Grid.SetColumn(panel, 1);
            Grid.SetRow(panel, 1);
        }

        public void ClearScreen()
        {
            _total = null;
            EventHandler.StopScannerReceiver();
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

        private void ClearOnClick()
        {
            if (_number != null && _number.Text.Length != 0)
                _number.Text = "";
            if (EventHandler.ItemsPositions.Count == 0)
                MainMenuActivity();
            else if (_positions.SelectedIndex == -1)
            {
                if (MessageBox.Show(LanguageEngine.Language["WareHouseActivity DeleteConfirm"], LanguageEngine.Language["WareHouseActivity DeleteConfirmTitle"], MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                    EventHandler.ItemsPositions.Clear();
            }
            else
            {
                if (MessageBox.Show(LanguageEngine.Language["WareHouseActivity SingleDeleteConfirm"], LanguageEngine.Language["WareHouseActivity SingleDeleteConfirmTitle"], MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                    EventHandler.ItemsPositions.RemoveAt(_positions.SelectedIndex);
            }
            UpdateScreen();
        }

        private void UpdateScreen()
        {
            if (_total != null)
                _total.Text = $"{LanguageEngine.Language["FastInvoiceActivity Total"]}: {EventHandler.ItemsPositions.Select(x => (x as CheckLine).FullPrice).Sum()}";
            if (_number != null)
                _number.Text = null;
            if (_textForm != null)
                _textForm.Text = null;
            _positions.Items.Refresh();
        }

        private void InitClock(TextBlock dateTime)
        {
            DispatcherTimer dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Interval = TimeSpan.FromSeconds(1);
            dateTimeTimer.Tick += (sender, args) => { dateTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture); };
            dateTimeTimer.Start();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        public void PaymentOnClick()
        {
            //TODO сюда напиши уже что-нибудь
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) => EventHandler.ProceedPayment();
            worker.RunWorkerCompleted += (sender, args) => UpdateScreen();
            worker.RunWorkerAsync();
        }

        private void AddPosition(Action<string, string> action, string textFormText, string number)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) => action(textFormText, number);
            worker.RunWorkerCompleted += (sender, args) => UpdateScreen();
            worker.RunWorkerAsync();
        }
        
        /*private void AddPosition(string textFormText, string numberText)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) => EventHandler.AddPosition(textFormText, numberText);
            worker.RunWorkerCompleted += (sender, args) => UpdateScreen();
            worker.RunWorkerAsync();
        }*/
        
        /*private void AddPositionForSaving(string textFormText)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) => EventHandler.AddPositionForSaving(textFormText);
            worker.RunWorkerCompleted += (sender, args) => UpdateScreen();
            worker.RunWorkerAsync();
        }*/

        /*private void AddUserPosition(string textFormText)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) => EventHandler.AddUserPosition(textFormText);
            worker.RunWorkerCompleted += (sender, args) => UpdateScreen();
            worker.RunWorkerAsync();
        }*/
        
        private void Authorize(string login, string password)
        {
            var worker = new BackgroundWorker();
            var isAuthorized = false;
            worker.DoWork += (sender, args) => isAuthorized = EventHandler.Authorize(login, password);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                if (isAuthorized)
                    MainMenuActivity();
            };
            worker.RunWorkerAsync();
        }
    }
}