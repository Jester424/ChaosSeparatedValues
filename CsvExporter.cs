using System.IO;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;

namespace ChaosSeparatedValues
{
    public static class CsvExporter
    {
        public static void WriteHeader(CsvWriter csv)
        {
            csv.WriteHeader<MailingRecord>();
            csv.NextRecord();
        }

        public static void WriteRecord(CsvWriter csv, MailingRecord record)
        {
            csv.WriteRecord(record);
        }
    }
}