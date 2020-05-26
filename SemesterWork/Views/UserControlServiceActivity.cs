using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            Window.Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(32, GridUnitType.Star) });

            Grid topBar = new Grid();
            Window.Grid.Children.Add(topBar);
            Grid.SetRow(topBar, 0);
            Grid userControls = new Grid();
            Window.Grid.Children.Add(userControls);
            Grid.SetRow(userControls, 1);
            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            topBar.ColumnDefinitions.Add(new ColumnDefinition());
            topBar.ColumnDefinitions.Add(new ColumnDefinition());

            TextBlock programName = new TextBlock() { Text = $" {Variables.ProgramName}", FontSize = 20 };
            TextBlock dateTime = new TextBlock() { Text = DateTime.Now.ToString(CultureInfo.CurrentCulture), TextAlignment = TextAlignment.Center, FontSize = 20 };
            TextBlock admin = new TextBlock() { Text = $"Администратор: {EventHandler.CurrentUser.Name} ", TextAlignment = TextAlignment.Right, FontSize = 20 }; //TODO localize
            topBar.Children.Add(programName);
            Grid.SetColumn(programName, 0);
            topBar.Children.Add(dateTime);
            Grid.SetColumn(dateTime, 1);
            topBar.Children.Add(admin);
            Grid.SetColumn(admin, 2);
            
            InitClock(dateTime);

            userControls.ColumnDefinitions.Add(new ColumnDefinition());
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1.25, GridUnitType.Star) });
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(10, GridUnitType.Star) });
            userControls.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });

            Grid userInput = new Grid();
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(10, GridUnitType.Star) });
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.5, GridUnitType.Star) });
            userInput.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(2.5, GridUnitType.Star) });

            _textForm = new TextBox() { FontSize = 48 };
            _textForm.KeyDown += (sender, args) =>
            {
                if (args.Key == Key.Enter)
                    AddPosition((a,b) => EventHandler.AddUserPosition(a));
            };

            var findUser = new Button() { Content = "Найти", FontSize = 48 }; //TODO localize
            Button addNewUser = new Button() { Content = "Добавить", FontSize = 48 }; //TODO localize
            findUser.Click += (sender, args) => AddPosition((a,b) => EventHandler.AddUserPosition(a));
            addNewUser.Click += (sender, args) => EventHandler.AddNewUser();
            userInput.Children.Add(_textForm);
            Grid.SetColumn(userInput, 0);
            userInput.Children.Add(findUser);
            Grid.SetColumn(findUser, 1);
            userInput.Children.Add(addNewUser);
            Grid.SetColumn(addNewUser, 2);
            userControls.Children.Add(userInput);

            Binding[] binds = {
                new Binding("Id") { Mode = BindingMode.OneTime },
                new Binding("Name") { Mode = BindingMode.Default },
                new Binding("AccessLevel") { Mode = BindingMode.Default }
            };
            _positions = new DataGrid()
            {
                ItemsSource = EventHandler.ItemsPositions,
                SelectionMode = DataGridSelectionMode.Single,
                FontSize = 20,
                AutoGenerateColumns = false,
                Columns =
                {
                    new DataGridTextColumn() { Header = "id", Binding = binds[0], MinWidth = 200 }, //TODO localize
                    new DataGridTextColumn() { Header = "Имя", Binding = binds[1], MinWidth = 750 }, //TODO localize
                    new DataGridComboBoxColumn() { Header = "Уровень доступа" , TextBinding = binds[2], MinWidth = 500, ItemsSource = new List<string> { "Normal", "Manager", "Admin" } }, //TODO localize
                }
            };
            foreach (var column in _positions.Columns)
                column.CanUserSort = false;
            userControls.Children.Add(_positions);
            Grid.SetRow(_positions, 1);

            Grid controls = new Grid();
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

            var deleteButton = new Button() { Content = "Удалить пользователя", FontSize = 40, Height = 100 }; //TODO localize
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

            Button saveButton = new Button() { Content = "Сохранить", FontSize = 40, Height = 100 }; //TODO localize
            saveButton.Click += (sender, args) => EventHandler.SaveUsersPositions();
            controls.Children.Add(saveButton);
            Grid.SetColumn(saveButton, 2);

            userControls.Children.Add(controls);
            Grid.SetRow(controls, 2);
        }
    }
}