using System.ComponentModel;
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

            _apply = new Button() { Content = LanguageEngine.Language["UserChangingActivity SaveButton"], Height = 50, FontSize = 20 };
            _loginTBox.Text = user.Id;
            _loginTBox.IsEnabled = false;
            _nameTBox.Text = user.Name;
            _accessLevelSelector.SelectedItem = user.AccessLevel;
            _accessLevelSelector.SelectionChanged += (sender, args) =>
            {
                if (user.Id == EventHandler.CurrentUser.Id && (sender as ComboBox).SelectedItem.ToString() != "Admin")
                    MessageBox.Show(LanguageEngine.Language["UserChangingActivity ChangingAccessLevel"],
                        LanguageEngine.Language["UserChangingActivity ChangingAccessLevelTitle"],
                        MessageBoxButton.OK, MessageBoxImage.Warning);
            };
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
            worker.RunWorkerCompleted += (sender, args) =>
            {
                if (EventHandler.CurrentUser.AccessLevel == "Admin")
                    new UserControlServiceActivity(Window);
                else
                {
                    EventHandler.ItemsPositions.Clear();
                    new MainMenuActivity(Window);
                    MessageBox.Show(LanguageEngine.Language["UserChangingActivity Kicked"],
                        LanguageEngine.Language["UserChangingActivity KickedTitle"],
                        MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            };
            worker.RunWorkerAsync();
        }
    }
}
