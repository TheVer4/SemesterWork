using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace SemesterWork.Views
{
    public class NewUserActivity : ActivityWithDynamics
    {
        protected TextBox _nameTBox;
        protected TextBox _loginTBox;
        protected PasswordBox _passwordPBox;
        protected PasswordBox _rePasswordPBox;
        protected ComboBox _accessLevelSelector;
        protected Button _apply;
        protected Button _cancel;
        protected StackPanel _panel;

        public NewUserActivity(MainWindow window) : base(window)
        {
            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(5, GridUnitType.Star) });
            Window.Grid.Children.Add(grid);
            Grid.SetRow(grid, 1);

            _panel = new StackPanel();

            _apply = new Button() { Content = "Добавить", Height = 50, FontSize = 20 }; //TODO localize

            var name = new StackPanel();
            var nameTBlock = new TextBlock() { Text = "Full Name", FontSize = 20, Height = 30 }; //TODO localize
            _nameTBox = new TextBox() { FontSize = 20 };
            _nameTBox.TextChanged += (sender, args) => OnSomethingChange(_apply);
            name.Children.Add(nameTBlock);
            name.Children.Add(_nameTBox);
            _panel.Children.Add(name);

            var login = new StackPanel();
            var loginTBlock = new TextBlock() { Text = "Login", FontSize = 20, Height = 30 }; //TODO localize
            _loginTBox = new TextBox() { FontSize = 20 };
            _loginTBox.TextChanged += (sender, args) => OnSomethingChange(_apply);
            login.Children.Add(loginTBlock);
            login.Children.Add(_loginTBox);
            _panel.Children.Add(login);

            var password = new StackPanel();
            var passwordTBlock = new TextBlock() { Text = "Password", FontSize = 20, Height = 30 }; //TODO localize
            _passwordPBox = new PasswordBox() { FontSize = 20 };
            _passwordPBox.PasswordChanged += (sender, args) => OnSomethingChange(_apply);
            password.Children.Add(passwordTBlock);
            password.Children.Add(_passwordPBox);
            _panel.Children.Add(password);

            var rePassword = new StackPanel();
            var rePasswordTBlock = new TextBlock() { Text = "Confirm the password", FontSize = 20, Height = 30 }; //TODO localize
            _rePasswordPBox = new PasswordBox() { FontSize = 20 };
            _rePasswordPBox.PasswordChanged += (sender, args) => OnSomethingChange(_apply);
            rePassword.Children.Add(rePasswordTBlock);
            rePassword.Children.Add(_rePasswordPBox);
            _panel.Children.Add(rePassword);

            var accessLevel = new StackPanel();
            var accessLevelTBlock = new TextBlock() { Text = "Уровень доступа", FontSize = 20 }; //TODO localize
            _accessLevelSelector = new ComboBox() { FontSize = 20 };
            _accessLevelSelector.SelectedIndex = 0;
            _accessLevelSelector.ItemsSource = new List<string> { "Normal", "Manager", "Admin" }; //TODO localize
            accessLevel.Children.Add(accessLevelTBlock);
            accessLevel.Children.Add(_accessLevelSelector);
            _panel.Children.Add(accessLevel);
           
            _apply.IsEnabled = false;
            _apply.Click += (sender, args) => AddNewUser(
                new User(_loginTBox.Text, _nameTBox.Text, _passwordPBox.Password),
                _accessLevelSelector.SelectedItem.ToString());
            _panel.Children.Add(_apply);

            _cancel = new Button() { Content = "Отмена", Height = 50, FontSize = 20 }; //TODO localize
            _cancel.Click += (sender, args) =>
            {
                if (MessageBox.Show("Вы уверены, что хотите выйти?", "Подтвердите действие", MessageBoxButton.YesNo, //TODO localize
                        MessageBoxImage.Question) == MessageBoxResult.Yes)
                    new UserControlServiceActivity(Window);
            };
            _panel.Children.Add(_cancel);

            Grid.SetColumn(_panel, 1);
            Grid.SetRow(_panel, 1);
            grid.Children.Add(_panel);
        }

        protected void AddNewUser(User newUser, string password)
        {
            var worker = new BackgroundWorker();
            var isInputOk = false;
            worker.DoWork += (sender, args) => isInputOk = EventHandler.AddNewUser(newUser, password);
            worker.RunWorkerCompleted += (sender, args) =>
            {
                if (isInputOk)
                    new UserControlServiceActivity(Window);
                else
                {
                    _loginTBox.Text = "";
                    MessageBox.Show($"Пользователь с таким логином уже существует, попробуйте {newUser.Id}{new Random().Next(0, 1000)} или _{newUser.Id}_", //TODO localize
                        "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error); //TODO localize
                }
            };
            worker.RunWorkerAsync();
        }

        private void OnSomethingChange(Button apply)
        {
            apply.IsEnabled =
                _nameTBox.Text.Length != 0 &&
                _loginTBox.Text.Length != 0 &&
                _passwordPBox.Password.Length != 0 &&
                _passwordPBox.Password == _rePasswordPBox.Password;
        }
    }
}
