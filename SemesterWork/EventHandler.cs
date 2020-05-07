using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
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
                int amount = 1;
                if (_number.Text.Length != 0) 
                    amount = int.Parse(_number.Text);
                try
                {
                    var pos = new CheckLine(new ProductData(info), amount);
                    var asd = _invoicePositions.Where(x => x.Data.EAN13 == code);
                    if (asd.Any())
                        asd.FirstOrDefault().Amount += amount;
                     else
                        _invoicePositions.Add(pos);
                }
                catch
                {
                    MessageBox.Show($"Позиция с кодом {code} не найдена, попробуте повторить операцию",
                    "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            };
        }
        
        
        private void AddPositionOnClick(object sender, RoutedEventArgs e)
        {
            string code = _barcodeForm.Text;
            _readBarcode = code;
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
    }
}