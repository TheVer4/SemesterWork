namespace SemesterWork
{
    public static class Variables
    {
        public static string MachineName { get => System.Environment.MachineName; }
        public static string PrinterPath { get; set; } = @"\\WINDOWS\POS";
        public static string WelcomeMotd { get; set; } = "!НЕ ЯВЛЯЕТСЯ ДОКУМЕНТОМ!";
        public static string ProgramName { get => "SemesterWork"; }
        public static string BarcodeScannerPort { get; set; } = "COM4";
        public static string InstitutionName { get; set; }
        public static string InstitutionAddress { get; set; }
        public static string DBConnectionString { get => @"Data Source = .\DataBases\DataBase.db; Version=3"; private set { } }
        public static string CFGPath { get => @".\config.cfg"; private set { } }
    }
}