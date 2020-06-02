﻿using System;
 using System.Collections;
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
            List<string> perTimeSource = new List<string>() { "Today", "Yesterday", "Week", "Month", "Season", "Year", "All time", "Custom..." };

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
                    new DataGridTextColumn() {Header = "Cashier", Binding = new Binding("CashierName"), CanUserSort = false, MinWidth = 300},
                    new DataGridTextColumn() {Header = "Invoices", Binding = new Binding("Invoices"), CanUserSort = false },
                    new DataGridTextColumn() {Header = "Average", Binding = new Binding("Average"), CanUserSort = false },
                    new DataGridTextColumn() {Header = "Total", Binding = new Binding("Total"), CanUserSort = false },
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
            
            Button export = new Button() {Content = "Export as .CSV", FontSize = 20}; //TODO LOCALIZE
            export.Click += EventHandler.ExportOnClick;
            bottomControls.Children.Add(export);

            ComboBox employee = new ComboBox() { FontSize = 40 };
            employee.ItemsSource = EventHandler.UsersList().Append("All");
            employee.SelectedItem = "All";
            employee.SelectedIndex = 0;
            employee.DropDownClosed += (sender, args) =>
            {
                string name = ((ComboBox) sender).Text;
                int oneDay = 24 * 60 * 60 - 1;
                long startTime = new DateTimeOffset(start.SelectedDate ?? DateTime.Today).ToUnixTimeSeconds();
                long endTime = new DateTimeOffset(end.SelectedDate ?? DateTime.Today).ToUnixTimeSeconds() + oneDay;
                BackgroundWorker worker = new BackgroundWorker();
                worker.DoWork += (o, eventArgs) => 
                    EventHandler.AddStatisticsPositions(name,
                        startTime,
                        endTime);
                worker.RunWorkerCompleted += (o, eventArgs) => 
                    UpdateDynamics();
                worker.RunWorkerAsync();
            };
            topControls.Children.Add(employee);
            
            ComboBox pertime = new ComboBox() { FontSize = 40 };
            pertime.ItemsSource = perTimeSource;
            pertime.DropDownClosed += (sender, args) => { };
            topControls.Children.Add(pertime);
            Grid.SetColumn(pertime, 1);

            start.Focusable = false;
            start.DisplayDateStart = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateLeftLimit).DateTime;
            start.DisplayDateEnd = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateRightLimit).DateTime;
            start.SelectedDate = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateLeftLimit).DateTime;
            topControls.Children.Add(start);
            Grid.SetColumn(start, 2);
            
            end.Focusable = false;
            end.DisplayDateStart = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateLeftLimit).DateTime;
            end.DisplayDateEnd = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateRightLimit).DateTime;
            end.SelectedDate = DateTimeOffset.FromUnixTimeSeconds(DocumentsDBController.DateRightLimit).DateTime;
            topControls.Children.Add(end);
            Grid.SetColumn(end, 3);
        }

    }
}