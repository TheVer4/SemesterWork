using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace SemesterWork
{
    public static class EventHandler
    {
        public static List<object> ItemsPositions { get; } = new List<object>();
        public static User CurrentUser { get; set; }
        public static bool IsSettingsOK { get; set; } = true;

        private static string _readBarcode;

        private static DispatcherTimer _timer;
        
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
            _readBarcode = scanned;
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
                MessageBox.Show(String.Format(LanguageEngine.Language["FastInvoiceActivity AddPositionErrorMessageBox"], code),
                    LanguageEngine.Language["FastInvoiceActivity AddPositionErrorMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void AddPositionForSaving(string code)
        {
            if (code.Length == 13)
            {
                var availablePosition = ItemsPositions.Where(x => (x as DBProductData).Data.EAN13 == code);
                if (availablePosition.Any())
                    MessageBox.Show(String.Format(LanguageEngine.Language["WareHouseActivity PositionContainsQuestion"], code),
                        LanguageEngine.Language["WareHouseActivity PositionContainsQuestionTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
                else 
                {
                    var info = WareHouseDBController.Find(code);
                    if (!info.Any())
                        ItemsPositions.Add(new DBProductData(new ProductData(code), false));
                    else if (MessageBox.Show(String.Format(LanguageEngine.Language["WareHouseActivity ContainsQuestion"], code),
                            LanguageEngine.Language["WareHouseActivity ContainsQuestionTitle"], MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                        if(!ItemsPositions.Where(x => (x as DBProductData).Data.EAN13 == code).Any())
                            ItemsPositions.Add(new DBProductData(new ProductData(info), true));
                }
            }
            else
                MessageBox.Show(LanguageEngine.Language["WareHouseActivity EAN13FormatError"],
                    LanguageEngine.Language["WareHouseActivity EAN13FormatErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void AddUserPosition(string infoStr)
        {
            var usersInfo = UserDBController.FindLike(infoStr);
            if (usersInfo.Any())
                foreach (var userInfo in usersInfo)
                {
                    var currentUser = new User(userInfo);
                    if (!(ItemsPositions
                            .Select(x => (x as User))
                            .Where(x => x.Name == currentUser.Name || x.Id == currentUser.Id)
                            .Any()))
                        ItemsPositions.Add(currentUser);
                    else if (usersInfo.Count == 1)
                        MessageBox.Show("Пользоавтель с такими данными уже есть в таблице", //TODO localize
                            "Внимание", MessageBoxButton.OK, MessageBoxImage.Error); //TODO localize
                }
            else
                MessageBox.Show("Пользователь с такими данными не найден", //TODO localize
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); //TODO localize                  
        }

        public static bool AddNewUser(User newUser, string password)
        {
            var newUserHash = GetHash(password);
            try
            {
                UserDBController.Add(newUser.Id, newUser.Name, newUser.AccessLevel, newUserHash);
            }
            catch(System.Data.SQLite.SQLiteException)
            {
                return false;
            }
            ItemsPositions.Add(newUser);
            return true;
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

        public static void UpdateUser(User user, string password)
        {
            UserDBController.Update(user, GetHash(password));
            var oldUser = ItemsPositions.FirstOrDefault(x => (x as User).Id == user.Id);
            ItemsPositions[ItemsPositions.IndexOf(oldUser)] = user;
        }

        public static void SaveUsersPositions()
        {
            foreach (User user in ItemsPositions)
                UserDBController.Update(user);
            MessageBox.Show(LanguageEngine.Language["WareHouseActivity SaveMessageBox"],
                LanguageEngine.Language["WareHouseActivity SaveMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public static void DeleteFromDB(int selectedIndex)
        {
            if (ItemsPositions.Count == 0)
                MessageBox.Show(LanguageEngine.Language["WareHouseActivity DeleteFromDB DeletingError"],
                    LanguageEngine.Language["WareHouseActivity DeleteFromDB DeletingErrorTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            else if (selectedIndex == -1)
            {
                if (MessageBox.Show(LanguageEngine.Language["WareHouseActivity DeleteFromDB DeletingPositions"],
                        LanguageEngine.Language["WareHouseActivity DeleteFromDB DeletingPositionsTitle"], MessageBoxButton.YesNo,
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
                    if (MessageBox.Show(LanguageEngine.Language["WareHouseActivity DeleteFromDB DeletingPosition"],
                            LanguageEngine.Language["WareHouseActivity DeleteFromDB DeletingPositionTitle"], MessageBoxButton.YesNo,
                            MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    {
                        var code = (ItemsPositions[selectedIndex] as DBProductData).Data.EAN13;
                        WareHouseDBController.Remove(code);
                        ItemsPositions.RemoveAt(selectedIndex);
                    }
                }
                else
                    MessageBox.Show(LanguageEngine.Language["WareHouseActivity DeleteFromDB DeletingNotInDB"],
                        LanguageEngine.Language["WareHouseActivity DeleteFromDB DeletingNotInDBTitle"], MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        public static void DeleteUserFromDB(int selectedIndex)
        {
            if (ItemsPositions.Count == 0)
                MessageBox.Show(LanguageEngine.Language["UserControlServiceActivity NothingToRemove"],
                    LanguageEngine.Language["UserControlServiceActivity NothingToRemoveTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            else if (ItemsPositions.Select(x => (x as User).Id).Contains(CurrentUser.Id))
                MessageBox.Show(LanguageEngine.Language["UserControlServiceActivity SelfRemoveDisallowed"],
                    LanguageEngine.Language["UserControlServiceActivity SelfRemoveDisallowedTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            else if (selectedIndex == -1)
            {
                if (MessageBox.Show("Вы уверены, что хотите удалить все эти позиции из базы?", //TODO localize
                        "Подтвердите действие", MessageBoxButton.YesNo, //TODO localize
                        MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    foreach (User user in ItemsPositions)
                        UserDBController.Remove(user.Id);
                    ItemsPositions.Clear();
                }
            }
            else
            {
                if (MessageBox.Show(LanguageEngine.Language["UserControlServiceActivity ConfirmUserRemoving"],
                        LanguageEngine.Language["UserControlServiceActivity ConfirmUserRemovingTitle"], MessageBoxButton.YesNo,
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

        public static string GetHash(string password)
        {
            using var hash = System.Security.Cryptography.SHA512.Create();
            return BitConverter.ToString(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
        }

        public static bool Authorize(string login, string password)
        {
            var userInfo = UserDBController.FindById(login);
            string userHash = GetHash(password);
            if (userInfo.Any() && userInfo[3] == userHash)
            {
                CurrentUser = new User(userInfo);
                return true;
            }
            MessageBox.Show(LanguageEngine.Language["LoginActivity AuthorizationMessageBox"],
                LanguageEngine.Language["LoginActivity AuthorizationMessageBoxTitle"], MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        
        public static void ProceedPayment(Invoice invoice)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'"; //TODO move to settings
            var currentUserName = CurrentUser.Name;
            var jsonStr = JsonConvert.SerializeObject(invoice);
            PrintInvoice.Print(invoice);
            DocumentsDBController.Add(
                (int) DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                currentUserName, 
                jsonStr);
            foreach (CheckLine position in invoice.Positions)
                WareHouseDBController.DecreaseAmountBy(position.Data.EAN13, position.Amount);
            ItemsPositions.Clear();
        }

        public static void StartScannerReceiver(
            Action<Action<string, string>> action, 
            Action<string, string> subAction)
        {
            _readBarcode = null;
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += (sender, args) =>
            {
                if (_readBarcode != null)
                {
                    string readBarcodeSync = _readBarcode;
                    action((a,b) => subAction(readBarcodeSync, b));
                }
                _readBarcode = null;
            };
            _timer.Start();
        }

        public static void StopScannerReceiver()
        {
            _timer?.Stop();    
            Environment.BarcodeReader?.Dispose();    
        }

        public static void ExportOnClick(object sender, RoutedEventArgs e)
        {
            Stream myStream;
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
 
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.FileName = $"Export {DateTime.Now.ToString().Replace(':', '-')}";
            saveFileDialog1.RestoreDirectory = true ;
 
            if(saveFileDialog1.ShowDialog() == true)
            {
                if ((myStream = saveFileDialog1.OpenFile()) != null)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Actual from;123;to;123");
                    sb.AppendLine("Cashier;Invoices;Average;Total");
                    foreach (var pos in ItemsPositions.OfType<EmployeeStatistic>())
                        sb.AppendLine($"{pos.CashierName};{pos.Invoices};{pos.Average};{pos.Total}");
                    byte[] data = Encoding.Default.GetBytes(sb.ToString());
                    myStream.Write(data, 0, data.Length);
                    myStream.Close();
                }
                //TODO localize
            }
        }

        public static List<string> UsersList()
        {
            return DBController.SQLFindDistinct("documents", "CashierName");
        }

        private static void FillEmployeeStatistic(string cashierName, long fromTime, long toTime)
        {
            List<Invoice> data = new List<Invoice>();
            var info = DBController.SQLNonVoidCommand(
                $"SELECT Data FROM documents WHERE CashierName='{cashierName}' AND DateTime BETWEEN {fromTime} AND {toTime}");
            var jsons = info.Select(x => x.First()).ToList();
            foreach (var json in jsons)
                data.Add(JsonConvert.DeserializeObject<Invoice>(json));
            if(data.Count != 0) ItemsPositions.Add(new EmployeeStatistic(data));
        }
    
        public static void AddStatisticsPositions(string name, long fromTime, long toTime)
        { 
            ItemsPositions.Clear();
            if (name == "All")
            {
                foreach (var user in UsersList())
                    FillEmployeeStatistic(user, fromTime, toTime);
                ItemsPositions.Add(new EmployeeStatistic("Total", ItemsPositions.Select(x => ((EmployeeStatistic) x).Invoices).Sum(), ItemsPositions.Select(x => ((EmployeeStatistic) x).Total).Sum()));
            }
            else
                FillEmployeeStatistic(name, fromTime, toTime);
        }
    }
}