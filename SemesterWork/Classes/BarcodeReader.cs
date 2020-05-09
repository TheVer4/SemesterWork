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
                    "Не удалось открыть взаимодействие со сканером. Проверьте соединение и перезапустите режим из главного меню",
                    "Внимание!", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        public void AddReader(SerialDataReceivedEventHandler deleg)
        {
            _port.DataReceived += deleg;
        }
        
        public void Dispose() {
            _port.Close();
        }
        
    }
}