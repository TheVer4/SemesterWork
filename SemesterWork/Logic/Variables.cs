namespace SemesterWork
{
    public static class Variables
    {
        public static string MachineName { get => System.Environment.MachineName; }
        public static string PrinterPath { get; set; }
        public static string WelcomeMotd { get; set; }
        public static string ProgramName { get => "SemesterWork"; }
        public static string BarcodeScannerPort { get; set; }
        public static string InstitutionName { get; set; }
        public static string DBConnectionString { get => @"Data Source = .\DataBases\DataBase.db; Version=3"; }
        public static string CFGPath { get => @".\config.cfg"; }
    }
}