﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace SemesterWork
{
    public class StatisticsActivity : ActivityWithDynamics
    {
        public StatisticsActivity(MainWindow window) : base(window)
        {
            List<string> perTimeSource = new List<string>() 
            { 
                LanguageEngine.Language["StatisticsActivity Today"],
                LanguageEngine.Language["StatisticsActivity Yesterday"],
                LanguageEngine.Language["StatisticsActivity Week"],
                LanguageEngine.Language["StatisticsActivity Month"],
                LanguageEngine.Language["StatisticsActivity Season"],
                LanguageEngine.Language["StatisticsActivity Year"],
                LanguageEngine.Language["StatisticsActivity AllTime"],
                LanguageEngine.Language["StatisticsActivity Custom"]
            };

            DatePicker start = new DatePicker() { FontSize = 40 };
            DatePicker end = new DatePicker() { FontSize = 40 };
            
            Grid grid = new Grid();
            Window.Grid.Children.Add(grid);
            Grid.SetRow(grid,1);
            
            grid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star)});
            grid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(15, GridUnitType.Star)});
            grid.ColumnDefinitions.Add(new ColumnDefinition() {Width = new GridLength(1, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(1, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(1, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(10, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(1, GridUnitType.Star)});
            grid.RowDefinitions.Add(new RowDefinition(){Height = new GridLength(1, GridUnitType.Star)});

            Grid topControls = new Grid();
            topControls.ColumnDefinitions.Add(new ColumnDefinition());
            topControls.ColumnDefinitions.Add(new ColumnDefinition());
            topControls.ColumnDefinitions.Add(new ColumnDefinition());
            topControls.ColumnDefinitions.Add(new ColumnDefinition());
            grid.Children.Add(topControls);
            Grid.SetRow(topControls, 1);
            Grid.SetColumn(topControls, 1);
            
            Grid bottomControls = new Grid();
            bottomControls.ColumnDefinitions.Add(new ColumnDefinition());
            bottomControls.ColumnDefinitions.Add(new ColumnDefinition());
            grid.Children.Add(bottomControls);
            Grid.SetRow(bottomControls, 3);
            Grid.SetColumn(bottomControls, 1);
           
            _positions = new DataGrid() { 
                ItemsSource = EventHandler.ItemsPositions,
                IsReadOnly = true,
                FontSize = 20,
                AutoGenerateColumns = false,
                Columns =
                {
                    new DataGridTextColumn() 
                    {
                        Header = LanguageEngine.Language["StatisticsActivity Cashier"],
                        Binding = new Binding("CashierName"), CanUserSort = false, MinWidth = 300
                    },
                    new DataGridTextColumn() 
                    { 
                        Header = LanguageEngine.Language["StatisticsActivity Invoices"], 
                        Binding = new Binding("Invoices"), CanUserSort = false 
                    }, 
                    new DataGridTextColumn() 
                    { 
                        Header = LanguageEngine.Language["StatisticsActivity Average"], 
                        Binding = new Binding("Average"), CanUserSort = false 
                    }, 
                    new DataGridTextColumn() 
                    { 
                        Header = LanguageEngine.Language["StatisticsActivity Total"], 
                        Binding = new Binding("Total"), CanUserSort = false 
                    }
                }};
            grid.Children.Add(_positions);
            Grid.SetRow(_positions, 2);
            Grid.SetColumn(_positions, 1);
            
            var close = new Button();
            BitmapSource source = null;
            Image crossImage = new Image() { Width = 50, Height = 50 };
            var imageSourceWorker = new BackgroundWorker();
            imageSourceWorker.DoWork += (sender, args) =>
            {
                source = GetBitmapSource(@"images/cross.png");
            };
            imageSourceWorker.RunWorkerCompleted += (sender, args) =>
            {
                crossImage.Source = source;
                close.Content = crossImage;
            };
            imageSourceWorker.RunWorkerAsync();
            close.Click += (sender, args) =>
            {
                EventHandler.ItemsPositions.Clear();
                new MainMenuActivity(Window);
            };
            bottomControls.Children.Add(close);
            Grid.SetColumn(close, 1);
            
            Button export = new Button() {Content = LanguageEngine.Language["StatisticsActivity Export"] + " .CSV", FontSize = 20};
            export.Click += (sender, args) => ThreadedAction((a, b) => EventHandler.ExportOnClick());
            bottomControls.Children.Add(export);

            ComboBox employee = new ComboBox() { FontSize = 40 };
            var worker = new BackgroundWorker();
            var userList = new List<string>();
            worker.DoWork += (sender, args) => 
                userList = EventHandler.GetUsersList();
            worker.RunWorkerCompleted += (sender, args) =>
                employee.ItemsSource = userList.Append(LanguageEngine.Language["StatisticsActivity All"]);
            worker.RunWorkerAsync();
            employee.SelectedItem = LanguageEngine.Language["StatisticsActivity All"];
            employee.DropDownClosed += (sender, args) => ShowStatistics(employee, start, end);
            topControls.Children.Add(employee);
            
            ComboBox pertime = new ComboBox() { FontSize = 40 };
            pertime.ItemsSource = perTimeSource;
            pertime.DropDownClosed += (sender, args) => ChooseTimeLimits(sender as ComboBox, employee, start, end);
            pertime.SelectedIndex = (int) SelectedDate.AllTime;
            topControls.Children.Add(pertime);
            Grid.SetColumn(pertime, 1);

            start.Focusable = false;
            start.DisplayDateStart = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateLeftLimit).LocalDateTime;
            start.DisplayDateEnd = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateRightLimit).LocalDateTime;
            start.SelectedDate = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateLeftLimit).LocalDateTime;
            start.IsEnabled = false;
            start.CalendarClosed += (sender, args) => ShowStatistics(employee, start, end);
            topControls.Children.Add(start);
            Grid.SetColumn(start, 2);
            
            end.Focusable = false;
            end.DisplayDateStart = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateLeftLimit).LocalDateTime;
            end.DisplayDateEnd = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateRightLimit).LocalDateTime;
            end.SelectedDate = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateRightLimit).LocalDateTime;
            end.IsEnabled = false;
            end.CalendarClosed += (sender, args) => ShowStatistics(employee, start, end);
            topControls.Children.Add(end);
            Grid.SetColumn(end, 3);
        }

        private void ChooseTimeLimits(ComboBox sender, ComboBox employee, DatePicker start, DatePicker end)
        {
            start.IsEnabled = false;
            end.IsEnabled = false;
            switch ((SelectedDate) sender.SelectedIndex)
            {
                case SelectedDate.Today:
                    if (DocumentsDBController.DateRightLimit < new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds())
                    {
                        MessageBox.Show("Невозможно показать статистику за это время, поскольку продаж не было",
                            "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                    start.SelectedDate = DateTime.Today;
                    end.SelectedDate = DateTime.Today;
                    break;
                case SelectedDate.Yesterday:
                    if (DocumentsDBController.DateRightLimit < new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds() - 24*60*60)
                    {
                        MessageBox.Show("Невозможно показать статистику за это время, поскольку продаж не было",
                            "Предупреждение!", MessageBoxButton.OK, MessageBoxImage.Hand);
                    }
                    start.SelectedDate = DateTimeOffset.FromUnixTimeSeconds((new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds() - 24*60*60)).LocalDateTime;
                    end.SelectedDate = DateTimeOffset.FromUnixTimeSeconds((new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds() - 24*60*60)).LocalDateTime;
                    break;
                case SelectedDate.Week:
                    start.SelectedDate = DateTimeOffset.FromUnixTimeSeconds((new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds() - 7*24*60*60)).LocalDateTime;
                    end.SelectedDate = DateTime.Today;
                    break;
                case SelectedDate.Month:
                    start.SelectedDate = DateTimeOffset.FromUnixTimeSeconds((new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds() - 30*24*60*60)).LocalDateTime;
                    end.SelectedDate = DateTime.Today;
                    break;
                case SelectedDate.Season:
                    start.SelectedDate = DateTimeOffset.FromUnixTimeSeconds((new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds() - 3*30*24*60*60)).LocalDateTime;
                    end.SelectedDate = DateTime.Today;
                    break;
                case SelectedDate.Year: 
                    start.SelectedDate = DateTimeOffset.FromUnixTimeSeconds((new DateTimeOffset(DateTime.Today).ToUnixTimeSeconds() - 365*24*60*60)).LocalDateTime;
                    end.SelectedDate = DateTime.Today;
                    break;
                case SelectedDate.AllTime: 
                    start.SelectedDate = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateLeftLimit).LocalDateTime;
                    end.SelectedDate = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateRightLimit).LocalDateTime;
                    break;
                case SelectedDate.Custom:
                    start.IsEnabled = true;
                    end.IsEnabled = true;
                    break;
            }
            ShowStatistics(employee, start, end);
        }

        private void ShowStatistics(ComboBox employee, DatePicker start, DatePicker end)
        {
            string name = employee.Text;
            int oneDay = 24 * 60 * 60 - 1;
            long startTime = new DateTimeOffset(start.SelectedDate ?? DateTime.Today).ToUnixTimeSeconds();
            long endTime = new DateTimeOffset(end.SelectedDate ?? DateTime.Today).ToUnixTimeSeconds() + oneDay;
            ThreadedAction((a, b) => EventHandler.AddStatisticsPositions(name, startTime, endTime));
        }
    }

    enum SelectedDate
    {
        Today, Yesterday, Week, Month, Season, Year, AllTime, Custom
    }
}