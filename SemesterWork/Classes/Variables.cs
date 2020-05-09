using System;

namespace SemesterWork
{
    public static class Variables
    {
        public static string MachineName { get => Environment.MachineName; }
        public static string PrinterPath { get; set; } = @"\\Windows\POS";

        private static string _welcomeMotd = "!НЕ ЯВЛЯЕТСЯ ДОКУМЕНТОМ!";
        public static string WelcomeMotd { get => _welcomeMotd; set => _welcomeMotd = value; }

        public static string ProgramName { get => "SemesterWork"; }
        public static string BarcodeScannerPort { get; set; } = "COM4";
        public static string InstitutionName { get; set; }
        public static string InstitutionAddress { get; set; }
        public static string DBConnectionString { get => @"Data Source = ..\..\DataBases\DataBase.db; Version=3"; private set { } }
    }
}