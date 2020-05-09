﻿using System;
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
      
        private void ClearOnClick(object sender, RoutedEventArgs e)
        {
            if (_number.Text.Length != 0) _number.Text = "";
            else if (_invoicePositions.Count == 0)
                MainMenuActivity();
            else if (_positions.SelectedIndex == -1)
            {
                if (MessageBox.Show("Проведите картой", "Подтвердите действие", MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    _invoicePositions.Clear();
            } 
            else
            {
                if (MessageBox.Show("Проведите картой", "Подтвердите действие", MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                _invoicePositions.RemoveAt(_positions.SelectedIndex);
            }
            UpdateScreen();
        }

        private void ClearSavingOnClick(object sender, RoutedEventArgs e)
        {
            if (_savingPositions.Count == 0)
                MainMenuActivity();
            else if (_positions.SelectedIndex == -1)
            {
                if (MessageBox.Show(Lang["WareHouseActivity DeleteConfirm"], Lang["WareHouseActivity DeleteConfirmTitle"], MessageBoxButton.YesNo,
                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                    _savingPositions.Clear();
            }
            else
            {
                if (MessageBox.Show(Lang["WareHouseActivity SingleDeleteConfirm"], Lang["WareHouseActivity SingleDeleteConfirmTitle"], MessageBoxButton.YesNo,
                MessageBoxImage.Question) == MessageBoxResult.Yes)
                    _savingPositions.RemoveAt(_positions.SelectedIndex);
            }
            UpdateScreen();
        }
        
        private void AmountOnClick(object sender, RoutedEventArgs e)
        {
            if(_positions.SelectedIndex == -1) return;
            if (_number.Text.Length == 0)
                _invoicePositions[_positions.SelectedIndex].Amount++;
            else {
                double amount = double.Parse(_number.Text, CultureInfo.InvariantCulture);
                string units = _invoicePositions[_positions.SelectedIndex].Data.Units;
                if(amount <= 0) _invoicePositions.RemoveAt(_positions.SelectedIndex);
                else if (_invoicePositions[_positions.SelectedIndex].Amount < amount)
                    _invoicePositions[_positions.SelectedIndex].Amount = units == "шт." ? Math.Round(amount, MidpointRounding.ToEven) : amount;
                else if (MessageBox.Show("Проведите картой", "Подтвердите действие", MessageBoxButton.YesNo,
                    MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    _invoicePositions[_positions.SelectedIndex].Amount = units == "шт." ? Math.Round(amount, MidpointRounding.ToEven) : amount;
            }
            UpdateScreen();
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
                    MessageBox.Show(String.Format(Lang["FastInvoiceActivity AddPositionErrorMessageBox"], code),
                    Lang["FastInvoiceActivity AddPositionErrorMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
                UpdateScreen();
            };
            worker.RunWorkerAsync();
        }

        private void AddPositionForSaving(string code)
        {
            if (code.Length == 13)
            { 
                var info = new List<string>();
                var worker = new BackgroundWorker();
                var availablePosition = _savingPositions.Where(x => x.Data.EAN13 == code);
                if (availablePosition.Any())
                {
                    MessageBox.Show(String.Format(Lang["WareHouseActivity ContainsQuestion"], code),
                    Lang["WareHouseActivity ContainsQuestionTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
                    UpdateScreen();
                    return;
                }
                worker.DoWork += (sender, args) =>
                {
                    info = WareHouseDBController.Find(code);
                };
                worker.RunWorkerCompleted += (sender, args) =>
                {
                    if (!info.Any())
                    {
                        double amount = 1;
                        if (_number.Text.Length != 0)
                            amount = double.Parse(_number.Text, CultureInfo.InvariantCulture);
                        var item = new ProductData(code);
                        _savingPositions.Add(new DBProductData(new ProductData(code), false));
                    }
                    else if (MessageBox.Show(String.Format(Lang["WareHouseActivity ContainsQuestion"], code),
                        Lang["WareHouseActivity ContainsQuestionTitle"], MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                        _savingPositions.Add(new DBProductData(new ProductData(info), true));
                    UpdateScreen();
                };
                worker.RunWorkerAsync();
            }
            else
                MessageBox.Show(Lang["WareHouseActivity EAN13FormatError"],
                Lang["WareHouseActivity EAN13FormatErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void SavePositions()
        {
            var worker = new BackgroundWorker();
            foreach (var position in _savingPositions)             
                if (position.IsInDB)
                    worker.DoWork += (sender, args) =>
                        WareHouseDBController.Update(position.Data);
                else
                    worker.DoWork += (sender, args) =>
                        WareHouseDBController.Insert(position.Data);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                MessageBox.Show(Lang["WareHouseActivity SaveMessageBox"],
                Lang["WareHouseActivity SaveMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
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
                    MessageBox.Show(Lang["LoginActivity AuthorizationMessageBox"],
                    Lang["LoginActivity AuthorizationMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            };
            worker.RunWorkerAsync();
        }
        
        private void PaymentOnClick(object sender, RoutedEventArgs e)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'"; 
            _printInvoice.Print(_invoicePositions);
        }
        private void UpdateScreen()
        {            
            _readBarcode = null;
            if (_total != null)
                _total.Text = Lang["FastInvoiceActivity Total"] + $": { _invoicePositions.Select(x => x.FullPrice).Sum() }";
            _number.Text = null;
            _barcodeForm.Text = null;
            _positions.Items.Refresh();
        }
    }
}