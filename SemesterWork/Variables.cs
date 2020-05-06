using System;

namespace SemesterWork
{
    public static class Variables
    {
        private static string _machineName = Environment.MachineName;
        public static string MachineName { get => _machineName; }

        private static string _printerName;
        public static string PrinterName
        {
            get => _printerName;
            set { _printerName = value; }
        }

        private static string _printerPath = "\\\\Windows\\POS";
        public static string PrinterPath { get => _printerPath; }

        private static string _welcomeMotd = "!НЕ ЯВЛЯЕТСЯ ДОКУМЕНТОМ!";
        public static string WelcomeMotd { get => _welcomeMotd; set => _welcomeMotd = value; }

        public static string ProgramName { get => "SemesterWork"; }
        public static string BarcodeScannerPort { get; set; }
        public static string InstitutionName { get; set; }
        public static string InstitutionAddress { get; set; }
        public static string DBConnectionString { get => @"Data Source = ..\..\DataBases\DataBase.db; Version=3"; private set { } }
    }
}