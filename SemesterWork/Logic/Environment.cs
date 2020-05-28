namespace SemesterWork
{
    public static class Environment
    {
        public static BarcodeReader BarcodeReader { get; private set; }

        public static void Initialize()
        {           
            new LanguageEngine(); //TODO пересмотреть механизм инициализации
            EventHandler.UpdateFromCFG();          
        }

        public static void InitBarcodeReader()
        {
            BarcodeReader = new BarcodeReader(Variables.BarcodeScannerPort, 9600);
        }

        public static void Destroy()
        {
            BarcodeReader?.Dispose();
        }
    }
}