using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SemesterWork
{
    public class PaymentActivity : ActivityWithDynamics
    {
        private TextBlock _cashLabel, _cashlessLabel, _saleLabel, _changeLabel;
        private Button _clearOff;
        private double _cash, _cashless, _rest;
        public PaymentActivity(MainWindow window) : base(window)
        {

            Grid invoiceControls = new Grid();
            Window.Grid.Children.Add(invoiceControls);
            Grid.SetRow(invoiceControls, 1);
            
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });

            TextBlock fullTotal = new TextBlock() { Text = $"{"Сумма к оплате"}: {finalTotal}", FontSize = 48}; //TODO localize
            invoiceControls.Children.Add(fullTotal);
            
            _cashLabel =  new TextBlock() { FontSize = 36};
            _cashlessLabel = new TextBlock() { FontSize = 36};
            _changeLabel = new TextBlock() { FontSize = 36};
            _saleLabel = new TextBlock() { FontSize = 36};
            _total = new TextBlock();
            
            StackPanel stackPanel = new StackPanel();
            stackPanel.Children.Add(_cashLabel);
            stackPanel.Children.Add(_cashlessLabel);
            stackPanel.Children.Add(_changeLabel);
            stackPanel.Children.Add(_saleLabel);
            
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
                IsEnabled = false,
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
            foreach (var column in _positions.Columns)
            {
                column.CanUserSort = false;
                column.IsReadOnly = true;
            }
            stackPanel.Children.Add(_positions);
            
            
            invoiceControls.Children.Add(stackPanel);
            Grid.SetRow(stackPanel, 1);
            
            _number = new TextBox() { FontSize = 48 };
            _number.PreviewTextInput += NumberValidationTextBox;
            Grid.SetColumn(_number, 1);
            invoiceControls.Children.Add(_number);
            
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
            clear.Click += (sender, args) => CrossButton();
            keyboard.Children.Add(clear);
            Grid.SetColumn(clear, 2);
            Grid.SetRow(clear, 3);

            controls.Children.Add(keyboard);
            
            Button addCash = new Button() { Content = "Наличные", FontSize = 40, Height = 100 };
            addCash.Click += (sender, args) =>
            {
                if (_number != null && _number.Text.Length != 0)
                    ChangeTotalSummary(cash: double.Parse(_number.Text, CultureInfo.InvariantCulture));
                else
                    ChangeTotalSummary(cash: _rest);
            };
            Button addCashless = new Button() { Content = "Безнал", FontSize = 40, Height = 100 };
            addCashless.Click += (sender, args) =>
            {
                if (_number != null && _number.Text.Length != 0)
                    ChangeTotalSummary(cashless: double.Parse(_number.Text, CultureInfo.InvariantCulture));
                else
                    ChangeTotalSummary(cashless: _rest);
            };
            _clearOff = new Button() { Content = "Расчёт", FontSize = 40, Height = 100, IsEnabled = false }; //TODO localize
            _clearOff.Click += (sender, args) => PaymentOnClick();
            _total.FontSize = 40;
            _total.Margin = new Thickness(15, 20, 0, 0);
            controls.Children.Add(addCash);
            controls.Children.Add(addCashless);
            controls.Children.Add(_clearOff);
            controls.Children.Add(_total);
            invoiceControls.Children.Add(controls);
            Grid.SetColumn(controls, 1);
            Grid.SetRow(controls, 1);
            
            ChangeTotalSummary(0, 0, 0);
        }

        private void CrossButton()
        {
            if (_number != null && _number.Text.Length != 0)
                _number.Text = "";
            else
                if (MessageBox.Show("Проведите картой", "Подтвердите действие", MessageBoxButton.YesNo,
                        MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                {
                    EventHandler.ItemsPositions.Clear();
                    new FastInvoiceActivity(Window);
                }
            ChangeTotalSummary();
        }
        
        private void PaymentOnClick()
        {
            Invoice invoice = new Invoice(_cash, _cashless, 0, 0, EventHandler.ItemsPositions.OfType<CheckLine>().ToList(), EventHandler.CurrentUser.Name);
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) => EventHandler.ProceedPayment(invoice);
            worker.RunWorkerCompleted += (sender, args) => new FastInvoiceActivity(Window);
            worker.RunWorkerAsync();
        }

        public void ChangeTotalSummary(double cash = -1, double cashless = -1, double sale = -1) //TODO Localize
        {
            if (_number != null && _number.Text.Length != 0)
                _number.Text = "";
            
            if (cash != -1) _cash += cash;
            if (cashless != -1) _cashless += cashless;
            double change = finalTotal - _cash - _cashless;
            _rest = change > 0 ? finalTotal - _cash - _cashless : 0;

            if (_rest == 0) _clearOff.IsEnabled = true;
            
            _cashLabel.Text = $" {"Наличными"}: {_cash}";
            _cashlessLabel.Text = $" {"Безналичными"}: {_cashless}";
            if(sale != -1) _saleLabel.Text = $" {"Скидка"}: {sale}";
            _changeLabel.Text = $" {"Сдача"}: {(change > 0 ? 0 : Math.Abs(change))}";
            _total.Text = $"{"Остаток"}: {_rest}";
        }
        
    }
}