﻿using System.IO.Ports;
 using System.Windows;

 namespace SemesterWork
{
    public class BarcodeReader
    {
        private readonly SerialPort _port;
        
        public BarcodeReader(string comPort, int baudRate)
        {
            _port = new SerialPort(comPort, baudRate);
            try
            {
                _port.Open();
            }
            catch
            {
                MessageBox.Show(
                    MainWindow.Lang["BarcodeReader NotFoundException"],
                    MainWindow.Lang["BarcodeReader NotFoundExceptionTitle"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void AddReader(SerialDataReceivedEventHandler deleg)
        {
            _port.DataReceived += deleg;
        }
        
        public void Dispose() 
        {
            _port.Close();
        }       
    }
}