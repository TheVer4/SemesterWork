using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SemesterWork
{
    public class ActivityWithDynamics : Activity
    {
        protected static DataGrid positions
        {
            get => _positions;
            set
            {
                _positions = value;
                UpdateDynamics();
            }
        }

        protected static TextBox textForm
        {
            get => _textForm;
            set
            {
                _textForm = value;
                UpdateDynamics();
            }
        }

        protected static TextBox number
        {
            get => _number;
            set
            {
                _number = value;
                UpdateDynamics();
            }
        }

        protected static TextBlock total
        {
            get => _total;
            set
            {
                _total = value;
                UpdateDynamics();
            }
        }

        private static DataGrid _positions;
        private static TextBox _textForm;
        private static TextBox _number;
        private static TextBlock _total;
        
        
        public ActivityWithDynamics(MainWindow window) : base(window)
        {
        }
        
        protected static void UpdateDynamics()
        {
            if (total != null)
                total.Text = $"{LanguageEngine.Language["FastInvoiceActivity Total"]}: {EventHandler.ItemsPositions.Select(x => (x as CheckLine).FullPrice).Sum()}";
            if (number != null)
                number.Text = null;
            if (textForm != null)
                textForm.Text = null;
            positions?.Items.Refresh();
        }

        protected void ClearOnClick()
        {
            if (number != null && number.Text.Length != 0)
                number.Text = "";
            if (EventHandler.ItemsPositions.Count == 0)
                new MainMenuActivity(Window);
            else if (positions.SelectedIndex == -1)
            {
                if (MessageBox.Show(LanguageEngine.Language["WareHouseActivity DeleteConfirm"], LanguageEngine.Language["WareHouseActivity DeleteConfirmTitle"], MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    EventHandler.ItemsPositions.Clear();
            }
            else
            {
                if (MessageBox.Show(LanguageEngine.Language["WareHouseActivity SingleDeleteConfirm"], LanguageEngine.Language["WareHouseActivity SingleDeleteConfirmTitle"], MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    EventHandler.ItemsPositions.RemoveAt(positions.SelectedIndex);
            }
            UpdateDynamics();
        }

        protected void AddPosition(Action<string, string> action)
        {
            var worker = new BackgroundWorker();
            string textFormText = textForm.Text, numberText = number.Text;
            worker.DoWork += (sender, args) => action(textFormText, numberText);
            worker.RunWorkerCompleted += (sender, args) => UpdateDynamics();
            worker.RunWorkerAsync();
        }
    }
}