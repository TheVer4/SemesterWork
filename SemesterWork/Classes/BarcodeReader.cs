﻿using System.IO.Ports;

namespace SemesterWork
{
    public class BarcodeReader
    {
        private readonly SerialPort _port;
        
        public BarcodeReader(string comPort, int baudRate)
        {
            _port = new SerialPort(comPort, baudRate);
            try { _port.Open(); }
            catch { }
        }

        public void AddReader(SerialDataReceivedEventHandler deleg)
        {
            _port.DataReceived += deleg;
        }
    }
}