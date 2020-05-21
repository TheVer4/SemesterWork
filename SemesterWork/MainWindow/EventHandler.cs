﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace SemesterWork
{
    public static class EventHandler
    {
        public static List<object> ItemsPositions { get; set; } = new List<object>();
        public static string ReadBarcode { get; set; }
        public static LanguageEngine Lang { get; set; }
        public static User CurrentUser { get; set; }
        public static bool IsSettingsOK { get; set; } = true;

        private static PrintInvoice _printInvoice = new PrintInvoice();

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static void AmountOnClick(string number, int selectedIndex)
        {
            if (selectedIndex == -1)
                return;
            if (number.Length == 0)
                (ItemsPositions[selectedIndex] as CheckLine).Amount++;
            else
            {
                double amount = double.Parse(number, CultureInfo.InvariantCulture);
                string units = (ItemsPositions[selectedIndex] as CheckLine).Data.Units;
                if (amount <= 0)
                    ItemsPositions.RemoveAt(selectedIndex);
                else if ((ItemsPositions[selectedIndex] as CheckLine).Amount < amount)
                    (ItemsPositions[selectedIndex] as CheckLine).Amount = units == "шт."
                        ? Math.Round(amount, mode: MidpointRounding.AwayFromZero)
                        : amount;
                else if (MessageBox.Show("Проведите картой", "Подтвердите действие", MessageBoxButton.YesNo,
                        MessageBoxImage.Exclamation) == MessageBoxResult.Yes)
                    (ItemsPositions[selectedIndex] as CheckLine).Amount = units == "шт."
                        ? Math.Round(amount, mode: MidpointRounding.AwayFromZero)
                        : amount;
            }           
        }

        public static void BarcodeRead(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort e = (SerialPort)sender;
            var scanned = e.ReadTo("\r");
            ReadBarcode = scanned;
        }

        public static void AddPosition(string number)
        {
            AddPosition(ReadBarcode, number);
        }

        public static void AddPosition(string code, string number)
        {
            var info = WareHouseDBController.Find(code);
            if (info.Any())
            {
                var amount = number.Length != 0
                    ? double.Parse(number, CultureInfo.InvariantCulture)
                    : 1;
                var item = new CheckLine(new ProductData(info), amount);
                var availablePosition = ItemsPositions.Where(x => (x as CheckLine).Data.EAN13 == code);
                if (availablePosition.Any())
                    (availablePosition.FirstOrDefault() as CheckLine).Amount += amount;
                else
                    ItemsPositions.Add(item);
            }
            else
                MessageBox.Show(String.Format(Lang["FastInvoiceActivity AddPositionErrorMessageBox"], code),
                    Lang["FastInvoiceActivity AddPositionErrorMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void AddPositionForSaving(string code)
        {
            if (code.Length == 13)
            {
                var availablePosition = ItemsPositions.Where(x => (x as DBProductData).Data.EAN13 == code);
                if (availablePosition.Any())
                    MessageBox.Show(String.Format(Lang["WareHouseActivity PositionContainsQuestion"], code),
                        Lang["WareHouseActivity PositionContainsQuestionTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
                else 
                {
                    var info = WareHouseDBController.Find(code);
                    if (!info.Any())
                        ItemsPositions.Add(new DBProductData(new ProductData(code), false));
                    else if (MessageBox.Show(String.Format(Lang["WareHouseActivity ContainsQuestion"], code),
                            Lang["WareHouseActivity ContainsQuestionTitle"], MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                        ItemsPositions.Add(new DBProductData(new ProductData(info), true));
                }
            }
            else
                MessageBox.Show(Lang["WareHouseActivity EAN13FormatError"],
                    Lang["WareHouseActivity EAN13FormatErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void AddUserPosition(string infoStr)
        {
            var availablePosition = ItemsPositions.Where(x => (x as User).Name == infoStr || (x as User).Id == infoStr);
            if (availablePosition.Any())
                MessageBox.Show("Пользоавтель с такими данными уже есть в таблице", //loc
                    "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); //loc
            else
            {
                var info = UserDBController.FindById(infoStr);
                if (!info.Any())
                    info = UserDBController.FindByName(infoStr);

                if (info.Any())
                    ItemsPositions.Add(new User(info));
                else
                    MessageBox.Show("Пользователь с такими данными не найден", //loc
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); //loc
            }
        }

        public static void AddNewUser()
        {
            //TODO
            //появление дополнительных окон ввода справа
            //при нажатии конфирм добавление в таблицу
        }

        public static void SavePositionsForWarehouse()
        {
            foreach (DBProductData position in ItemsPositions)
                if (position.IsInDB)
                    WareHouseDBController.Update(position.Data);
                else
                {
                    WareHouseDBController.Insert(position.Data);
                    position.IsInDB = true;
                }
        }

        public static void SaveUsersPositions()
        {
            var worker = new BackgroundWorker();
            foreach (User user in ItemsPositions)
                worker.DoWork += (sender, args) =>
                    UserDBController.Update(user);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                MessageBox.Show(Lang["WareHouseActivity SaveMessageBox"],
                    Lang["WareHouseActivity SaveMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
            };
            worker.RunWorkerAsync();
        }

        public static void DeleteFromDB(int selectedIndex)
        {
            if (ItemsPositions.Count == 0)
                MessageBox.Show(Lang["WareHouseActivity DeleteFromDB DeletingError"],
                    Lang["WareHouseActivity DeleteFromDB DeletingErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            else if (selectedIndex == -1)
            {
                if (MessageBox.Show(Lang["WareHouseActivity DeleteFromDB DeletingPositions"],
                        Lang["WareHouseActivity DeleteFromDB DeletingPositionsTitle"], MessageBoxButton.YesNo,
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (DBProductData position in ItemsPositions)
                        if (position.IsInDB)
                            WareHouseDBController.Remove(position.Data.EAN13);
                        ItemsPositions.Clear();
                }
            }
            else
            {
                if ((ItemsPositions[selectedIndex] as DBProductData).IsInDB)
                {
                    if (MessageBox.Show(Lang["WareHouseActivity DeleteFromDB DeletingPosition"],
                            Lang["WareHouseActivity DeleteFromDB DeletingPositionTitle"], MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        var code = (ItemsPositions[selectedIndex] as DBProductData).Data.EAN13;
                        WareHouseDBController.Remove(code);
                        ItemsPositions.RemoveAt(selectedIndex);
                    }
                }
                else
                    MessageBox.Show(Lang["WareHouseActivity DeleteFromDB DeletingNotInDB"],
                        Lang["WareHouseActivity DeleteFromDB DeletingNotInDBTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public static void DeleteUserFromDB(int selectedIndex)
        {
            if (ItemsPositions.Count == 0)
                MessageBox.Show("Нечего удалять", //loc
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); //loc
            else if (ItemsPositions.Select(x => (x as User).Id).Contains(CurrentUser.Id))
                MessageBox.Show("Вы не можете удалить себя.", //loc
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); //loc
            else if (selectedIndex == -1)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить все эти позиции?", //loc
                        "Подтвердите действие", MessageBoxButton.YesNo, //loc
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (User user in ItemsPositions)
                        UserDBController.Remove(user.Id);
                    ItemsPositions.Clear();
                }
            }
            else
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить эту позицию?", //loc 
                        "Подтвердите действие", MessageBoxButton.YesNo, //loc
                         MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    var id = (ItemsPositions[selectedIndex] as User).Id;
                    UserDBController.Remove(id);
                    ItemsPositions.RemoveAt(selectedIndex);
                }
            }
        }

        public static void SaveSettings()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                var path = Variables.CFGPath;
                using var sw = new StreamWriter(path, false);
                sw.WriteLine($"Language={LanguageEngine.Current}");
                sw.WriteLine($"PrinterPath={Variables.PrinterPath}");
                sw.WriteLine($"BarcodeScannerPort={Variables.BarcodeScannerPort}");
            };
            worker.RunWorkerAsync();
        }

        public static void UpdateFromCFG()
        {
            var path = Variables.CFGPath;
            if (File.Exists(path))
            {        
                using var sr = new StreamReader(path);

                var languageLine = sr.ReadLine();
                var language = languageLine.Substring(languageLine.IndexOf('=') + 1);
                var printerPath = sr.ReadLine();
                var barcodeScannerPort = sr.ReadLine();

                Lang = new LanguageEngine();

                if (LanguageEngine.Languages.Contains(language))
                    LanguageEngine.Current = language;
                else
                    IsSettingsOK = false;
                Variables.PrinterPath = printerPath.Substring(printerPath.IndexOf('=') + 1);
                Variables.BarcodeScannerPort = barcodeScannerPort.Substring(barcodeScannerPort.IndexOf('=') + 1);
            }
            else
                IsSettingsOK = false;
        }

        public static bool Authorize(string login, string password)
        {
            var userInfo = UserDBController.FindById(login);
            string userHash;
            using (var hash = System.Security.Cryptography.SHA512.Create())
                userHash = BitConverter.ToString(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
            if (userInfo.Any() && userInfo[3] == userHash)
            {
                CurrentUser = new User(userInfo);
                return true;
            }
            else
                MessageBox.Show(Lang["LoginActivity AuthorizationMessageBox"],
                    Lang["LoginActivity AuthorizationMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        
        public static void PaymentOnClick()
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";
            var currentUserName = CurrentUser.Name;
            var jsonStr = JsonConvert.SerializeObject(ItemsPositions);
            _printInvoice.Print(ItemsPositions.Select(x => (CheckLine)x).ToList());
            DocumentsDBController.Add(
                (int) DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                currentUserName, 
                jsonStr);
            foreach (CheckLine position in ItemsPositions)
                WareHouseDBController.DecreaseAmountBy(position.Data.EAN13, position.Amount);
            ItemsPositions.Clear();
        }
    }
}