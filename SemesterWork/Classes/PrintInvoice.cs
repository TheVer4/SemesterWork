using System;
using System.Collections.Generic;
using System.Linq;
using PrinterUtility;
using PrinterUtility.EscPosEpsonCommands;

namespace SemesterWork
{
    public class PrintInvoice
    {
        private Printer printer;

        public PrintInvoice()
        {
            printer = new Printer();
        }
        
        public bool Print(List<CheckLine> list)
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
                Encoder866.Recode(DateTime.Now.ToString()),
                epson.Lf(),
                Encoder866.Recode("ПРИХОД"),
                epson.Lf(),
                FormatCheckLines(list),
                epson.Lf(),
                Encoder866.Recode($"ИТОГО: {list.Select(x => x.FullPrice).Sum()} руб."),
                epson.Lf(),
                epson.Lf()
                ));
            return true;
        }

        private byte[] FormatCheckLines(List<CheckLine> list)
        {
            byte[] result = new byte[] {};
            foreach (var line in list)
                result = PrintExtensions.AddBytes(result, Encoder866.Recode(line.ToString()));
            return result;
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