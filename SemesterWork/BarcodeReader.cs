﻿using System.IO.Ports;

namespace SemesterWork
{
    public class BarcodeReader
    {
        private readonly SerialPort _port;
        
        public BarcodeReader(string comPort, int baudRate)
        {
            _port = new SerialPort(comPort, baudRate);
            _port.Open();
        }

        public void AddReader(SerialDataReceivedEventHandler deleg)
        {
            _port.DataReceived += deleg;
        }

        /*~BarcodeReader()
        {
            _port.Close();
        }*/
    }
}