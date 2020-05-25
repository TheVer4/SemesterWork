namespace SemesterWork
{
    public static class Environment
    {
        public static BarcodeReader _barcodeReader;
        public static void Initialize()
        {           
            new LanguageEngine(); //TODO пересмотреть механизм инициализации
            EventHandler.UpdateFromCFG();
            _barcodeReader = new BarcodeReader(Variables.BarcodeScannerPort, 9600);
        }

        public static void Destroy()
        {
            _barcodeReader.Dispose();
        }
    }
}