using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SemesterWork
{
    public class FastInvoiceActivity : Activity
    {
        public FastInvoiceActivity(MainWindow window) : base(window)
        {
            Window._total = new TextBlock();
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32, GridUnitType.Star) });
            Grid topBar = new Grid();
            Window.Grid.Children.Add(topBar);
            Grid.SetRow(topBar, 0);
            Grid invoiceControls = new Grid();
            Window.Grid.Children.Add(invoiceControls);
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

            Window.InitClock(dateTime);

            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(15, GridUnitType.Star) });
            invoiceControls.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            invoiceControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });

            Grid barcodeInput = new Grid();
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            barcodeInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            Window._textForm = new TextBox() { FontSize = 48 };
            Window._textForm.PreviewTextInput += Window.NumberValidationTextBox;
            Window._number = new TextBox() { FontSize = 48 };

            Window._textForm.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    Window.AddPosition(EventHandler.AddPosition, Window._textForm.Text, Window._number.Text);
            };

            Button addPosition = new Button() { Content = LanguageEngine.Language["FastInvoiceActivity AddPosition"], FontSize = 48 };
            addPosition.Click += (sender, args) => Window.AddPosition(EventHandler.AddPosition, Window._textForm.Text, Window._number.Text);

            barcodeInput.Children.Add(Window._textForm);
            Grid.SetColumn(addPosition, 1);
            barcodeInput.Children.Add(addPosition);
            invoiceControls.Children.Add(barcodeInput);


            Window._number.PreviewTextInput += Window.NumberValidationTextBox;
            Grid.SetColumn(Window._number, 1);
            invoiceControls.Children.Add(Window._number);
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

            Window._positions = new DataGrid()
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
            foreach (var column in Window._positions.Columns) {
                column.CanUserSort = false;
                column.IsReadOnly = true;
            }
            invoiceControls.Children.Add(Window._positions);
            Grid.SetRow(Window._positions, 1);

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
                key.Click += (sender, args) => Window._number.Text += ((Button)sender).Content.ToString();
                keyboard.Children.Add(key);
                Grid.SetColumn(key, 2 - i % 3);
                Grid.SetRow(key, i / 3);
            }
            Button zero = new Button() { Content = "0", FontSize = 40, Height = 100 };
            zero.Click += (sender, args) => { Window._number.Text += "0"; };
            keyboard.Children.Add(zero);
            Grid.SetRow(zero, 3);
            Button dot = new Button() { Content = ".", FontSize = 40, Height = 100 };
            dot.Click += (sender, args) => { Window._number.Text += "."; };
            keyboard.Children.Add(dot);
            Grid.SetColumn(dot, 1);
            Grid.SetRow(dot, 3);

            Image crossImage = new Image() { Width = 50, Height = 50 };
            BitmapSource source = null;
            Button clear = new Button();
            var imageSourceWorker = new BackgroundWorker();
            imageSourceWorker.DoWork += (sender, args) =>
            {
                source = Window.GetBitmapSource(@"images/cross.png");
            };
            imageSourceWorker.RunWorkerCompleted += (sender, args) =>
            {
                crossImage.Source = source;
                clear.Content = crossImage;
            };
            imageSourceWorker.RunWorkerAsync();
            clear.Click += (sender, args) => Window.ClearOnClick();
            keyboard.Children.Add(clear);
            Grid.SetColumn(clear, 2);
            Grid.SetRow(clear, 3);

            controls.Children.Add(keyboard);
            Button payment = new Button() { Content = LanguageEngine.Language["FastInvoiceActivity Payment"], FontSize = 40, Height = 100 };
            payment.Click += (sender, args) => Window.PaymentOnClick();
            Button amount = new Button() { Content = LanguageEngine.Language["FastInvoiceActivity Amount"], FontSize = 40, Height = 100 };
            amount.Click += (sender, args) =>
            {
                var numberText = Window._number.Text;
                var selectedIndex = Window._positions.SelectedIndex;
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, args) => EventHandler.AmountOnClick(numberText, selectedIndex);
                worker.RunWorkerCompleted += (sender, args) => Window.UpdateScreen();
                worker.RunWorkerAsync();
            };
            Window._total.Text = $"{LanguageEngine.Language["FastInvoiceActivity Total"]}: 0";
            Window._total.FontSize = 40;
            Window._total.Margin = new Thickness(15, 20, 0, 0);
            controls.Children.Add(payment);
            controls.Children.Add(amount);
            controls.Children.Add(Window._total);
            invoiceControls.Children.Add(controls);
            Grid.SetColumn(controls, 1);
            Grid.SetRow(controls, 1);

            EventHandler.StartScannerReceiver(Window.AddPosition, EventHandler.AddPosition,  Window._number.Text);
        }
    }
}