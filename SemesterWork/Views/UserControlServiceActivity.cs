using SemesterWork.Views;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace SemesterWork
{
    public class UserControlServiceActivity : ActivityWithDynamics
    {
        public UserControlServiceActivity(MainWindow window) : base(window)
        {
            Grid userControls = new Grid();
            Window.Grid.Children.Add(userControls);
            Grid.SetRow(userControls, 1);
            userControls.ColumnDefinitions.Add(new ColumnDefinition());
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.25, GridUnitType.Star) });
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });

            Grid userInput = new Grid();
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(7.5, GridUnitType.Star) });
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.5, GridUnitType.Star) });
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.5, GridUnitType.Star) });

            _textForm = new TextBox() { FontSize = 48 };
            _textForm.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    ThreadedAction((a, b) => EventHandler.AddUserPosition(a));
            };

            var findUser = new Button() { Content = LanguageEngine.Language["UserControlServiceActivity SearchButton"], FontSize = 48 };
            var allUsers = new Button() { Content = LanguageEngine.Language["UserControlServiceActivity AllButton"], FontSize = 48 };          
            findUser.Click += (sender, args) => ThreadedAction((a, b) => EventHandler.AddUserPosition(a));
            allUsers.Click += (sender, args) => ThreadedAction((a, b) => EventHandler.AddAllUsers());
            userInput.Children.Add(_textForm);
            Grid.SetColumn(userInput, 0);
            userInput.Children.Add(findUser);
            Grid.SetColumn(findUser, 1);
            userInput.Children.Add(allUsers);
            Grid.SetColumn(allUsers, 2);
            userControls.Children.Add(userInput);

            Binding[] binds = 
            {
                new Binding("Id"),
                new Binding("Name"),
                new Binding("AccessLevel") 
            };
            foreach (var bind in binds)
                bind.Mode = BindingMode.OneWay;
            _positions = new DataGrid()
            {
                ItemsSource = EventHandler.ItemsPositions,
                SelectionMode = DataGridSelectionMode.Single,              
                FontSize = 20,
                AutoGenerateColumns = false,
                Columns =
                {
                    new DataGridTextColumn() { Header = LanguageEngine.Language["UserControlServiceActivity ID"], Binding = binds[0], MinWidth = 200 }, 
                    new DataGridTextColumn() { Header = LanguageEngine.Language["UserControlServiceActivity FullName"], Binding = binds[1], MinWidth = 750 },
                    new DataGridComboBoxColumn() { Header = LanguageEngine.Language["UserControlServiceActivity AccessLevel"], TextBinding = binds[2], MinWidth = 500, ItemsSource = new List<string> { "Normal", "Manager", "Admin" } },
                }
            };
            foreach (var column in _positions.Columns)
                column.CanUserSort = false;
            userControls.Children.Add(_positions);
            _positions.MouseDoubleClick += (sender, args) =>
            {
                if (MessageBox.Show(LanguageEngine.Language["UserControlServiceActivity ConfirmChangeUser"],
                        LanguageEngine.Language["UserControlServiceActivity ConfirmChangeUserTitle"], MessageBoxButton.YesNo,
                         MessageBoxImage.Warning) == MessageBoxResult.Yes)
                    new UserChangingActivity(Window, ((sender as DataGrid).SelectedItem as User));
            };
            Grid.SetRow(_positions, 1);

            Grid controls = new Grid();
            controls.ColumnDefinitions.Add(new ColumnDefinition());
            controls.ColumnDefinitions.Add(new ColumnDefinition());
            controls.ColumnDefinitions.Add(new ColumnDefinition());
            controls.ColumnDefinitions.Add(new ColumnDefinition());

            Image crossImage = new Image() { Width = 50, Height = 50 };
            var worker = new BackgroundWorker();
            BitmapSource source = null;
            Button clear = new Button() { Height = 100 };
            worker.DoWork += (sender, args) =>
            {
                source = GetBitmapSource(@"images/cross.png");
            };
            worker.RunWorkerCompleted += (sender, args) =>
            {
                crossImage.Source = source;
                clear.Content = crossImage;
            };
            worker.RunWorkerAsync();
            clear.Click += (sender, args) => ClearOnClick();
            controls.Children.Add(clear);
            Grid.SetColumn(clear, 0);

            var deleteButton = new Button() { Content = LanguageEngine.Language["UserControlServiceActivity DeleteUser"], FontSize = 40, Height = 100 };
            deleteButton.Click += (sender, args) =>
            {
                var selectedIndex = _positions.SelectedIndex;
                var worker = new BackgroundWorker();
                worker.DoWork += (sender, args) => EventHandler.DeleteUserFromDB(selectedIndex);
                worker.RunWorkerCompleted += (sender, args) => UpdateDynamics();
                worker.RunWorkerAsync();
            };
            controls.Children.Add(deleteButton);
            Grid.SetColumn(deleteButton, 1);

            var addNewUser = new Button() { Content = LanguageEngine.Language["UserControlServiceActivity NewUser"], FontSize = 40, Height = 100 };
            addNewUser.Click += (sender, args) => new NewUserActivity(Window);
            controls.Children.Add(addNewUser);
            Grid.SetColumn(addNewUser, 2);

            Button saveButton = new Button() { Content = LanguageEngine.Language["UserControlServiceActivity SaveAll"], FontSize = 40, Height = 100 };
            saveButton.Click += (sender, args) => ThreadedAction((a, b) => EventHandler.SaveUsersPositions());
            controls.Children.Add(saveButton);
            Grid.SetColumn(saveButton, 3);

            userControls.Children.Add(controls);
            Grid.SetRow(controls, 4);
        }
    }
}