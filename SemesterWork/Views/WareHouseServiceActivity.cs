using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SemesterWork
{
    public class WareHouseServiceActivity : ActivityWithDynamics
    {
        public WareHouseServiceActivity(MainWindow window) : base(window)
        {
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
                    AddPosition((a, b) => EventHandler.AddPositionForSaving(a));
            };

            Button addPosition = new Button() { Content = LanguageEngine.Language["WareHouseActivity AddPosition"], FontSize = 48 };
            addPosition.Click += (sender, args) => AddPosition((a, b) => EventHandler.AddPositionForSaving(a));
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
                worker.RunWorkerCompleted += (sender, args) => UpdateDynamics();
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
            Environment.InitBarcodeReader();
        }
    }
}