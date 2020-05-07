using System;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows;

namespace SemesterWork
{
    public partial class MainWindow
    {

        private bool updatedSomeValue;

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
            try 
            { 
                invoicePositions.Add(new CheckLine(scanned, 1)); 
            }
            catch
            {
                MessageBox.Show($"Позиция с кодом {scanned} не найдена, попробуте повторить операцию",
                "Произошла ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            updatedSomeValue = true;
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