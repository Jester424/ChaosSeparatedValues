using System.IO;
using System.Collections.Generic;
using CsvHelper;
using System.Globalization;

namespace ChaosSeparatedValues
{
    public static class CsvExporter
    {
        public static void Export(string filePath, int recordCount, Logger logger)
        {
            using var writer = new StreamWriter(filePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<MailingRecord>();
            csv.NextRecord();

            for (int i = 0; i < recordCount; i++)
            {
                var record = Generator.Generate();

                csv.WriteRecord(record);
                csv.NextRecord();

                if (i % 1000 == 0 && i != 0)
                {
                    logger.Info($"Generated {i:N0} records");
                }
            }
        }
    }
}