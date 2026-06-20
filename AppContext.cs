using System;
using System.Collections.Generic;
using System.Text;

namespace ChaosSeparatedValues
{
    public class AppContext
    {
        public DateTime RunDateTime { get; } = DateTime.Now;

        public string RunName => $"run-{RunDateTime:yyyy-MM-dd_HH-mm-ss}";
    }
}
