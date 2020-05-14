using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;

namespace SemesterWork
{
    public partial class MainWindow
    {
        public void ClearScreen()
        {
            _total = null;
            _barcodeReader?.Dispose();
            _updateSmth?.Stop();
            Grid.Children.Clear();
            Grid.ColumnDefinitions.Clear();
            Grid.RowDefinitions.Clear();
        }

        private BitmapSource GetBitmapSource(string path)
        {
            Stream imageStreamSource = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            PngBitmapDecoder decoder = new PngBitmapDecoder(imageStreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
            return decoder.Frames[0];
        }

        private void ClearOnClick()
        {
            if (_number != null && _number.Text.Length != 0)
                _number.Text = "";
            if (_itemsPositions.Count == 0)
                MainMenuActivity();
            else if (_positions.SelectedIndex == -1)
            {
                if (MessageBox.Show(Lang["WareHouseActivity DeleteConfirm"], Lang["WareHouseActivity DeleteConfirmTitle"], MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                    _itemsPositions.Clear();
            }
            else
            {
                if (MessageBox.Show(Lang["WareHouseActivity SingleDeleteConfirm"], Lang["WareHouseActivity SingleDeleteConfirmTitle"], MessageBoxButton.YesNo,
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                    _itemsPositions.RemoveAt(_positions.SelectedIndex);
            }
            UpdateScreen();
        }

        private void AmountOnClick(object sender, RoutedEventArgs e)
        {
            if (_positions.SelectedIndex == -1)
                return;
            if (_number.Text.Length == 0)
                (_itemsPositions[_positions.SelectedIndex] as CheckLine).Amount++;
            else
            {
                double amount = double.Parse(_number.Text, CultureInfo.InvariantCulture);
                string units = (_itemsPositions[_positions.SelectedIndex] as CheckLine).Data.Units;
                if (amount <= 0)
                    _itemsPositions.RemoveAt(_positions.SelectedIndex);
                else if ((_itemsPositions[_positions.SelectedIndex] as CheckLine).Amount < amount)
                    (_itemsPositions[_positions.SelectedIndex] as CheckLine).Amount = units == "шт."
                        ? Math.Round(amount, mode: MidpointRounding.AwayFromZero)
                        : amount;
                else if (MessageBox.Show("Проведите картой", "Подтвердите действие", MessageBoxButton.YesNo,
                        MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    (_itemsPositions[_positions.SelectedIndex] as CheckLine).Amount = units == "шт."
                        ? Math.Round(amount, mode: MidpointRounding.AwayFromZero)
                        : amount;
            }
            UpdateScreen();
        }

        private void BarcodeRead(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort e = (SerialPort)sender;
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
                if (info.Any())
                {
                    var amount = _number.Text.Length != 0
                        ? double.Parse(_number.Text, CultureInfo.InvariantCulture)
                        : 1;
                    var item = new CheckLine(new ProductData(info), amount);
                    var availablePosition = _itemsPositions.Where(x => (x as CheckLine).Data.EAN13 == code);
                    if (availablePosition.Any())
                        (availablePosition.FirstOrDefault() as CheckLine).Amount += amount;
                    else
                        _itemsPositions.Add(item);
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
                var availablePosition = _itemsPositions.Where(x => (x as DBProductData).Data.EAN13 == code);
                if (availablePosition.Any())
                {
                    MessageBox.Show(String.Format(Lang["WareHouseActivity PositionContainsQuestion"], code),
                        Lang["WareHouseActivity PositionContainsQuestionTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
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
                        _itemsPositions.Add(new DBProductData(new ProductData(code), false));
                    else if (MessageBox.Show(String.Format(Lang["WareHouseActivity ContainsQuestion"], code),
                            Lang["WareHouseActivity ContainsQuestionTitle"], MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                        _itemsPositions.Add(new DBProductData(new ProductData(info), true));
                    UpdateScreen();
                };
                worker.RunWorkerAsync();
            }
            else
                MessageBox.Show(Lang["WareHouseActivity EAN13FormatError"],
                    Lang["WareHouseActivity EAN13FormatErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void AddUserPosition(string infoStr)
        {
            var info = new List<string>();
            var worker = new BackgroundWorker();
            var availablePosition = _itemsPositions.Where(x => (x as User).Name == infoStr || (x as User).Id == infoStr);
            if (availablePosition.Any())
            {
                MessageBox.Show("Пользоавтель с такими данными уже есть в таблице", //loc
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); //loc
                UpdateScreen();
                return;
            }
            worker.DoWork += (sender, args) =>
            {
                info = UserDBController.FindById(infoStr);
                if (!info.Any())
                    info = UserDBController.FindByName(infoStr);
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                if (info.Any())
                    _itemsPositions.Add(new User(info));
                else
                    MessageBox.Show("Пользователь с такими данными не найден", //loc
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); //loc
                UpdateScreen();
            };
            worker.RunWorkerAsync();
        }

        private void AddNewUser()
        {
            //TODO
            //появление дополнительных окон ввода справа
            //при нажатии конфирм добавление в таблицу
        }

        private void SavePositionsForWarehouse()
        {
            var worker = new BackgroundWorker();
            foreach (DBProductData position in _itemsPositions)
                if (position.IsInDB)
                    worker.DoWork += (sender, args) =>
                        WareHouseDBController.Update(position.Data);
                else
                {
                    worker.DoWork += (sender, args) =>
                        WareHouseDBController.Insert(position.Data);
                    position.IsInDB = true;
                }
            worker.RunWorkerCompleted += (sender, args) =>
            {
                MessageBox.Show(Lang["WareHouseActivity SaveMessageBox"],
                    Lang["WareHouseActivity SaveMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
            };
            worker.RunWorkerAsync();
        }

        private void SaveUsersPositions()
        {
            var worker = new BackgroundWorker();
            foreach (User user in _itemsPositions)
                worker.DoWork += (sender, args) =>
                    UserDBController.Update(user);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                MessageBox.Show(Lang["WareHouseActivity SaveMessageBox"],
                    Lang["WareHouseActivity SaveMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
            };
            worker.RunWorkerAsync();
        }

        private void DeleteFromDB()
        {
            var worker = new BackgroundWorker();
            if (_itemsPositions.Count == 0)
                MessageBox.Show(Lang["WareHouseActivity DeleteFromDB DeletingError"],
                    Lang["WareHouseActivity DeleteFromDB DeletingErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            else if (_positions.SelectedIndex == -1)
            {
                if (MessageBox.Show(Lang["WareHouseActivity DeleteFromDB DeletingPositions"],
                        Lang["WareHouseActivity DeleteFromDB DeletingPositionsTitle"], MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (DBProductData position in _itemsPositions)
                        if (position.IsInDB)
                            worker.DoWork += (sender, args) => 
                                WareHouseDBController.Remove(position.Data.EAN13);
                    worker.RunWorkerCompleted += (sender, args) =>
                    {
                        _itemsPositions.Clear();
                        UpdateScreen();
                    };
                }
            }
            else
            {
                if ((_itemsPositions[_positions.SelectedIndex] as DBProductData).IsInDB)
                {
                    if (MessageBox.Show(Lang["WareHouseActivity DeleteFromDB DeletingPosition"],
                            Lang["WareHouseActivity DeleteFromDB DeletingPositionTitle"], MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        var code = (_itemsPositions[_positions.SelectedIndex] as DBProductData).Data.EAN13;
                        worker.DoWork += (sender, args) =>
                            WareHouseDBController.Remove(code);
                        worker.RunWorkerCompleted += (sender, args) =>
                        {
                            _itemsPositions.RemoveAt(_positions.SelectedIndex);
                            UpdateScreen();
                        };
                    }
                }
                else
                    MessageBox.Show(Lang["WareHouseActivity DeleteFromDB DeletingNotInDB"],
                        Lang["WareHouseActivity DeleteFromDB DeletingNotInDBTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
            }
            worker.RunWorkerAsync();
        }

        private void DeleteUserFromDB()
        {
            var worker = new BackgroundWorker();
            if (_itemsPositions.Count == 0)
                MessageBox.Show("Нечего удалять", //loc
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); //loc
            else if (_itemsPositions.Select(x => (x as User).Id).Contains(_currentUser.Id))
                MessageBox.Show("Вы не можете удалить себя.", //loc
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); //loc
            else if (_positions.SelectedIndex == -1)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить все эти позиции?", //loc
                        "Подтвердите действие", MessageBoxButton.YesNo, //loc
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (User user in _itemsPositions)
                        worker.DoWork += (sender, args) =>
                            UserDBController.Remove(user.Id);
                    worker.RunWorkerCompleted += (sender, args) =>
                    {
                        _itemsPositions.Clear();
                        UpdateScreen();
                    };
                }
            }
            else
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить эту позицию?", //loc 
                        "Подтвердите действие", MessageBoxButton.YesNo, //loc
                         MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    var id = (_itemsPositions[_positions.SelectedIndex] as User).Id;
                    worker.DoWork += (sender, args) =>
                        UserDBController.Remove(id);
                    worker.RunWorkerCompleted += (sender, args) =>
                    {
                        _itemsPositions.RemoveAt(_positions.SelectedIndex);
                        UpdateScreen();
                    };
                }
            }
            worker.RunWorkerAsync();
        }

        private void SaveSettings()
        {
            var path = Variables.CFGPath;
            using var sw = new StreamWriter(path, false);
            sw.WriteLine($"Language={LanguageEngine.Current}");
            sw.WriteLine($"PrinterPath={Variables.PrinterPath}");
            sw.WriteLine($"BarcodeScannerPort={Variables.BarcodeScannerPort}");   
        }

        private void UpdateFromCFG()
        {
            var path = Variables.CFGPath;
            if (File.Exists(path))
            {
                using var sr = new StreamReader(path);

                var languageLine = sr.ReadLine();
                var language = languageLine.Substring(languageLine.IndexOf('=') + 1);
                if (LanguageEngine.Languages.Contains(language))
                    LanguageEngine.Current = language;
                else
                    _isSettingsOK = false;

                var printerPath = sr.ReadLine();
                Variables.PrinterPath = printerPath.Substring(printerPath.IndexOf('=') + 1);

                var barcodeScannerPort = sr.ReadLine();
                Variables.BarcodeScannerPort = barcodeScannerPort.Substring(barcodeScannerPort.IndexOf('=') + 1);
            }
            else
                _isSettingsOK = false;
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
            var currentUserName = _currentUser.Name;
            var jsonStr = JsonConvert.SerializeObject(_itemsPositions);
            _printInvoice.Print(_itemsPositions.Select(x => (CheckLine)x).ToList());
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
                DocumentsDBController.Add(
                    (int) DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                    currentUserName, 
                    jsonStr);
            foreach (CheckLine position in _itemsPositions)
                worker.DoWork += (sender, args) =>
                    WareHouseDBController.DecreaseAmountBy(position.Data.EAN13, position.Amount);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                _itemsPositions.Clear();
                UpdateScreen();
            };
            worker.RunWorkerAsync();
        }

        private void UpdateScreen()
        {            
            _readBarcode = null;
            if (_total != null)
                _total.Text = $"{Lang["FastInvoiceActivity Total"]}: {_itemsPositions.Select(x => (x as CheckLine).FullPrice).Sum()}";
            if (_number != null)
                _number.Text = null;
            if (_textForm != null)
                _textForm.Text = null;
            _positions.Items.Refresh();
        }
    }
}