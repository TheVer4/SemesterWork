using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;

namespace SemesterWork
{
    public partial class MainWindow
    {
        public MainWindow()
        {           
            InitializeComponent();
            Environment.Initialize();
            LoginActivity();
            Closed += (sender, args) => Environment.Destroy();
        }
        
    }
}