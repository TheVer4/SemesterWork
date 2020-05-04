﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SemesterWork
{
    public static class Encoder
    {
        public static Encoding Encoding = Encoding.GetEncoding(866);

        public static byte[] Recode(string str)
            => Encoding.GetBytes(str);

        public static string Recode(byte[] arr)
            => Encoding.GetString(arr);
    }
}