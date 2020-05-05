using System.IO.Ports;

namespace SemesterWork
{
    public class BarcodeReader
    {
        private readonly SerialPort _port;
        
        public BarcodeReader(string comPort, int baudRate)
        {
            this._port = new SerialPort(comPort, baudRate);
            _port.Open();
        }

        public void AddReader(SerialDataReceivedEventHandler deleg)
        {
            this._port.DataReceived += deleg;
        }

        ~BarcodeReader()
        {
            this._port.Close();
        }
    }
}