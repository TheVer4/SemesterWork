using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SemesterWork.Views
{
    public class UserChangingActivity : NewUserActivity
    {
        public UserChangingActivity(MainWindow window, User user) : base(window)
        {
            _panel.Children.Remove(_apply);
            _panel.Children.Remove(_cancel);

            _apply = new Button() { Content = "Сохранить", Height = 50, FontSize = 20 }; //TODO localize
            _loginTBox.Text = user.Id;
            _loginTBox.IsEnabled = false;
            _nameTBox.Text = user.Name;
            _accessLevelSelector.SelectedItem = user.AccessLevel;
            _apply.Click += (sender, args) => SaveUserChanges(
                new User(user.Id, _nameTBox.Text, _accessLevelSelector.SelectedItem.ToString()),
                _passwordPBox.Password);

            _panel.Children.Add(_apply);
            _panel.Children.Add(_cancel);
        }

        private void SaveUserChanges(User user, string password)
        {
            var worker = new BackgroundWorker();
            worker.DoWork += (sender, args) => EventHandler.UpdateUser(user, password);
            worker.RunWorkerCompleted += (sender, args) => new UserControlServiceActivity(Window);
            worker.RunWorkerAsync();
        }
    }
}
