using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using PrinterUtility;
using PrinterUtility.EscPosEpsonCommands;

namespace SemesterWork
{
    public static class PrintInvoice
    {
        public static void Print(List<CheckLine> list)
        {
            EscPosEpson epson = new EscPosEpson();
            new Printer().Send(FormatPaper(
                epson.Alignment.Center(),
                Encoder866.Recode("КАССОВЫЙ ЧЕК"),
                epson.Lf(),
                Encoder866.Recode(Variables.WelcomeMotd),
                epson.Lf(),
                Encoder866.Recode(Variables.InstitutionName),
                epson.Lf(),
                epson.Alignment.Left(),
                epson.Lf(),
                Encoder866.Recode(DateTime.Now.ToString(CultureInfo.InvariantCulture)),
                epson.Lf(),
                Encoder866.Recode("ПРИХОД"),
                epson.Lf(),
                FormatCheckLines(list),
                epson.Lf(),
                Encoder866.Recode($"ИТОГО: {list.Select(x => x.FullPrice).Sum()} руб."),
                epson.Lf(),
                epson.Lf()
                ));
        }

        private static byte[] FormatCheckLines(List<CheckLine> list)
        {
            byte[] result = new byte[0];
            foreach (CheckLine line in list)
                result = result.AddBytes(Encoder866.Recode(line.ToString()));
            return result;
        }
        
        private static byte[] FormatPaper(params byte[][] byteset)
        {
            byte[] data = new byte[0];
            foreach (byte[] line in byteset)
                data = data.AddBytes(line);
            return data;
        }       
    }    
}