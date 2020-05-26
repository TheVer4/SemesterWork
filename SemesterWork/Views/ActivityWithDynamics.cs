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
        protected static DataGrid _positions;
        protected static TextBox _textForm;
        protected static TextBox _number;
        protected static TextBlock _total; 
        
        protected ActivityWithDynamics(MainWindow window) : base(window)
        {
            _positions = null;
            _textForm = null;
            _number = null;
            _total = null;           
        }
        
        protected void UpdateDynamics()
        {
            if (_total != null)
                _total.Text = $"{LanguageEngine.Language["FastInvoiceActivity Total"]}: {EventHandler.ItemsPositions.Select(x => (x as CheckLine).FullPrice).Sum()}";
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
            UpdateDynamics();
        }

        protected void AddPosition(Action<string, string> action)
        {
            var worker = new BackgroundWorker();
            string textFormText = _textForm.Text, numberText = _number?.Text;
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
            PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
    }
}