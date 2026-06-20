using System;
using System.Collections.Generic;
using System.Text;

namespace ChaosSeparatedValues
{
    public static class DataDegrader
    {
        public static void Degrade(MailingRecord record)
        {
            record.City = null;
        }
    }
}
