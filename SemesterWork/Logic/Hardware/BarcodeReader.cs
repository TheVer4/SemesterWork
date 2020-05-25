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
                    LanguageEngine.Language["BarcodeReader NotFoundException"],
                    LanguageEngine.Language["BarcodeReader NotFoundExceptionTitle"], MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            _port.DataReceived += EventHandler.BarcodeRead;
        }

        public void Dispose() 
        {
            _port.Close();
        }       
    }
}