using System;

namespace SemesterWork
{
    public static class Variables
    {
        public static string ProgramName { get => "SemesterWork"; }
        private static string machineName = Environment.MachineName;
        private static string printerName;
        private static string printerPath = "\\\\Windows\\POS";
        private static string welcomeMotd = "!НЕ ЯВЛЯЕТСЯ ДОКУМЕНТОМ!";
        
        public static string MachineName { get => machineName; }
        
        public static string PrinterName
        {
            get => printerName;
            set { printerName = value; }
        }
        
        public static string PrinterPath { get => printerPath; }

        public static string InstitutionName { get; set; }
        public static string InstitutionAddress { get; set; }
        public static string WelcomeMotd { get => welcomeMotd; set => welcomeMotd = value; }

    }
}