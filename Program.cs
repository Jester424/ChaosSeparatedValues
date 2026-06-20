using System;
using System.IO;
using Bogus;
using CsvHelper;
using System.Globalization;

namespace ChaosSeparatedValues
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Logger.Info("Program started");

            string outputDirectory = "Data";
            Directory.CreateDirectory(outputDirectory);

            string cleanFilePath = Path.Combine(outputDirectory, "clean_list.csv");
            Logger.Info($"Clean output file: {cleanFilePath}");


            int recordCount = 10000;
            Logger.Info($"Requested record count: {recordCount:N0}");

            CsvExporter.Export(cleanFilePath, recordCount);

            Logger.Info($"Finished generating {recordCount:N0} records");
            Logger.Info($"Beginning data degradation");
            Thread.Sleep(2000);

            string degradedFilePath = Path.Combine(outputDirectory, "degraded_list.csv");
            Logger.Info($"Degraded output file: {degradedFilePath}");

            using var reader = new StreamReader(cleanFilePath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            using var writer = new StreamWriter(degradedFilePath);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csvWriter.WriteHeader<MailingRecord>();
            csvWriter.NextRecord();

            double degradationRate = 0.10;

            var random = new Random();
            int degradedRecordCount = 0;
            foreach (var record in csvReader.GetRecords<MailingRecord>())
            {
                if (random.NextDouble() < degradationRate)
                {
                    degradedRecordCount++;
                    DataDegrader.Degrade(record);
                }
                csvWriter.WriteRecord(record);
                csvWriter.NextRecord();
            }

            Logger.Info($"Degradation complete for {degradedRecordCount} records");
        }
    }
}
