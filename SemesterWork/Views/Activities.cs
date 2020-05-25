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
    public partial class MainWindow
    {
        //public DataGrid _positions;
        //public TextBox _number;
        //public TextBox _textForm;
        //public TextBlock _total;
        
        public void ClearScreen()
        {
            EventHandler.StopScannerReceiver();
            Grid.Children.Clear();
            Grid.ColumnDefinitions.Clear();
            Grid.RowDefinitions.Clear();
        }

        public BitmapSource GetBitmapSource(string path)
        {
            Stream imageStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }
        
        
        public void InitClock(TextBlock dateTime)
        {
            DispatcherTimer dateTimeTimer = new DispatcherTimer();
            dateTimeTimer.Interval = TimeSpan.FromSeconds(1);
            dateTimeTimer.Tick += (sender, args) => { dateTime.Text = DateTime.Now.ToString(CultureInfo.CurrentCulture); };
            dateTimeTimer.Start();
        }

        public void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        
        
        public void Authorize(string login, string password)
        {
            var worker = new BackgroundWorker();
            var isAuthorized = false;
            worker.DoWork += (sender, args) => isAuthorized = EventHandler.Authorize(login, password);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                if (isAuthorized)
                    new MainMenuActivity(this);
            };
            worker.RunWorkerAsync();
        }
    }

    public static class DynamicView
    {
        



    }
}