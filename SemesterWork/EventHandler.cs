using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace SemesterWork
{
    public partial class MainWindow
    {

        private bool _updatedSomeValue;

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Variables.InstitutionName = "ООО 'МОЯ ОБОРОНА'";
            printInvoice.Print();
        }
        
        private void ClearOnClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void BarcodeReaded(object sender, SerialDataReceivedEventArgs args)
        {
            SerialPort e = (SerialPort) sender;
            var scanned = e.ReadTo("\r");
           AddPosition(scanned);
        }

        private void AddPosition(string code)
        {
            int amount = 1;
            //if (_number.Text.Length != 0) amount = int.Parse(_number.Text);
            try 
            { 
                _invoicePositions.Add(new CheckLine(code, amount)); 
            }
            catch
            {
                MessageBox.Show($"Позиция с кодом {code} не найдена, попробуте повторить операцию",
                "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            _updatedSomeValue = true;
        }
        
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Authorize(string login, string password)
        {
            var userInfo = DBController.FindUserById(login);
            string userHash;
            using (var hash = System.Security.Cryptography.SHA512.Create())
                userHash = BitConverter.ToString(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
            if (userInfo.Any() && userInfo[3] == userHash)
            {
                currentUser = new User(userInfo);
                MainMenuActivity();
            }
            else
                MessageBox.Show("Неверно введены данные, попробуйте снова.",
                "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}