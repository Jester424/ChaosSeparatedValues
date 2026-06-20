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
            var appContext = new AppContext();

            string outputDirectory = appContext.RunName;
            Directory.CreateDirectory(outputDirectory);
            
            var logger = new Logger(appContext);

            logger.Info("Program started");

            string cleanFilePath = Path.Combine(outputDirectory, "clean_list.csv");
            logger.Info($"Clean output file: {cleanFilePath}");


            int recordCount = 10000;
            logger.Info($"Requested record count: {recordCount:N0}");

            CsvExporter.Export(cleanFilePath, recordCount, logger);

            logger.Info($"Finished generating {recordCount:N0} records");
            logger.Info($"Beginning data degradation");
            Thread.Sleep(2000);

            string degradedFilePath = Path.Combine(outputDirectory, "degraded_list.csv");
            logger.Info($"Degraded output file: {degradedFilePath}");

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

            logger.Info($"Degradation complete for {degradedRecordCount} records");
        }
    }
}
