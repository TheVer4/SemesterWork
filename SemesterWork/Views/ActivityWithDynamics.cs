using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace SemesterWork
{
    public class ActivityWithDynamics : Activity
    {
        protected DataGrid _positions;
        protected TextBox _textForm;
        protected TextBox _number;
        protected TextBlock _total; 
        protected double finalTotal;

        protected ActivityWithDynamics(MainWindow window) : base(window)
        {
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32, GridUnitType.Star) });

            Grid topBar = new Grid();
            Window.Grid.Children.Add(topBar);
            Grid.SetRow(topBar, 0);

            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            topBar.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock programName = new TextBlock() { Text = $" {Variables.ProgramName}", FontSize = 20 };
            TextBlock dateTime = new TextBlock() 
            { 
                Text = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                TextAlignment = TextAlignment.Center, FontSize = 20 };
            TextBlock admin = new TextBlock() 
            { 
                Text = $"{LanguageEngine.Language["ActivityWithDynamics Cashier"]}: {EventHandler.CurrentUser.Name} ",
                TextAlignment = TextAlignment.Right, FontSize = 20 
            };
            topBar.Children.Add(programName);
            Grid.SetColumn(programName, 0);
            topBar.Children.Add(dateTime);
            Grid.SetColumn(dateTime, 1);
            topBar.Children.Add(admin);
            Grid.SetColumn(admin, 2);

            InitClock(dateTime);
            
            finalTotal = EventHandler.ItemsPositions.OfType<CheckLine>().Select(x => x.FullPrice).Sum();
        }
        
        protected void UpdateDynamics()
        {
            finalTotal = EventHandler.ItemsPositions.OfType<CheckLine>().Select(x => x.FullPrice).Sum();
            if (_total != null)
                _total.Text = $"{LanguageEngine.Language["FastInvoiceActivity Total"]}: {finalTotal}";
            if (_number != null)
                _number.Text = null;
            if (_textForm != null)
                _textForm.Text = null;
            _positions?.Items.Refresh();           
        }

        protected void ClearOnClick()
        {
            if (_number != null && _number.Text.Length != 0)
                _number.Text = "";
            if (EventHandler.ItemsPositions.Count == 0)
                new MainMenuActivity(Window);
            else if (_positions.SelectedIndex == -1)
            {
                if (MessageBox.Show(LanguageEngine.Language["WareHouseActivity DeleteConfirm"],
                    LanguageEngine.Language["WareHouseActivity DeleteConfirmTitle"], MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    EventHandler.ItemsPositions.Clear();
            }
            else
            {
                if (MessageBox.Show(LanguageEngine.Language["WareHouseActivity SingleDeleteConfirm"],
                    LanguageEngine.Language["WareHouseActivity SingleDeleteConfirmTitle"], MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    EventHandler.ItemsPositions.RemoveAt(_positions.SelectedIndex);
            }
            UpdateDynamics();
        }

        protected void ThreadedAction(Action<string, string> action)
        {
            var worker = new BackgroundWorker();
            string textFormText = _textForm?.Text, numberText = _number?.Text;
            worker.DoWork += (sender, args) => action(textFormText, numberText);
            worker.RunWorkerCompleted += (sender, args) => UpdateDynamics();
            worker.RunWorkerAsync();
        }

        protected void InitClock(TextBlock dateTime)
        {
            DispatcherTimer dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Interval = TimeSpan.FromSeconds(1);
            dateTimeTimer.Tick += (sender, args) => { dateTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture); };
            dateTimeTimer.Start();
        }

        protected void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        protected BitmapSource GetBitmapSource(string path)
        {
            Stream imageStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            PngBitmapDecoder decoder = new PngBitmapDecoder(
                imageStreamSource,
                BitmapCreateOptions.PreservePixelFormat,
                BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
    }
}