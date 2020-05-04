using System;
using PrinterUtility;
using PrinterUtility.EscPosEpsonCommands;

namespace SemesterWork
{
    public class PrintInvoice
    {
        private int InvoicesCount = 0;
        private Printer printer;

        public PrintInvoice()
        {
            printer = new Printer();
        }
        
        public bool Print()
        {
            EscPosEpson epson = new EscPosEpson();
            printer.Send(FormatPaper(
                epson.Alignment.Center(),
                Encoder866.Recode("КАССОВЫЙ ЧЕК"),
                epson.Lf(),
                Encoder866.Recode(Variables.WelcomeMotd),
                epson.Lf(),
                Encoder866.Recode(Variables.InstitutionName),
                epson.Lf(),
                epson.Alignment.Left(),
                epson.Lf(),
                Encoder866.Recode($"Чек                            {InvoicesCount++}"), 
                epson.Lf(),
                Encoder866.Recode(DateTime.Now.ToString()),
                epson.Lf(),
                Encoder866.Recode("ПРИХОД"),
                epson.Lf(),
                Encoder866.Recode("ИТОГО: 420"),
                epson.Lf(),
                epson.Lf()
                ));
            return true;
        }

        private byte[] FormatPaper(params byte[][] byteset)
        {
            byte[] data = new byte[] {};
            foreach (var line in byteset)
                data = PrintExtensions.AddBytes(data, line);
            return data;
        }
        
    }
    
}