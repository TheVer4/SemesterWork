using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SemesterWork
{
    public partial class MainWindow
    {
        private string _readBarcode;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";
            printInvoice.Print();
        }
        
        private void ClearOnClick(object sender, RoutedEventArgs e)
        {
            if(_invoicePositions.Count == 0)
                MainMenuActivity();
            if (_positions.SelectedIndex == -1)
            {
                if (MessageBox.Show("Проведите картой", "Подтвердите действие", MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    _invoicePositions.Clear();
            } 
            else
            {
                _invoicePositions.RemoveAt(_positions.SelectedIndex);
            }
            FastInvoiceUpdate();
        }

        private void BarcodeRead(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort e = (SerialPort) sender;
            var scanned = e.ReadTo("\r");
            _readBarcode = scanned;
        }

        private void AddPosition(string code)
        {
            var info = new List<string>();
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                info = WareHouseDBController.Find(code);
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                double amount = 1;
                if (_number.Text.Length != 0) 
                    amount = double.Parse(_number.Text, CultureInfo.InvariantCulture);
                if (info.Any())
                {
                    var item = new CheckLine(new ProductData(info), amount);
                    var availablePosition = _invoicePositions.Where(x => x.Data.EAN13 == code);
                    if (availablePosition.Any())
                        availablePosition.FirstOrDefault().Amount += amount;
                    else
                        _invoicePositions.Add(item);
                }
                else
                    MessageBox.Show($"Позиция с кодом {code} не найдена, попробуте повторить операцию",
                    "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                FastInvoiceUpdate();
            };
            worker.RunWorkerAsync();
        }
         
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Authorize(string login, string password)
        {
            var userInfo = new List<string>();
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                userInfo = UserDBController.FindById(login);
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                string userHash;
                using (var hash = System.Security.Cryptography.SHA512.Create())
                    userHash = BitConverter.ToString(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
                if (userInfo.Any() && userInfo[3] == userHash)
                {
                    _currentUser = new User(userInfo);
                    MainMenuActivity();
                }
                else
                    MessageBox.Show("Неверно введены данные, попробуйте снова.",
                    "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            };
            worker.RunWorkerAsync();
        }

        private void FastInvoiceUpdate()
        {            
            _readBarcode = null;
            _total.Text = $"ИТОГО: { _invoicePositions.Select(x => x.FullPrice).Sum() }";
            _number.Text = null;
            _barcodeForm.Text = null;
            _positions.Items.Refresh();
        }
    }
}