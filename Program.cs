using System;
using System.IO;
using Bogus;
using CsvHelper;
using System.Globalization;
using System.CodeDom.Compiler;

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

            GenerateCleanDataFile(logger, cleanFilePath, recordCount);

            logger.Info($"Finished generating {recordCount:N0} records");
            logger.Info($"Beginning data degradation");
            Thread.Sleep(2000);

            string degradedFilePath = Path.Combine(outputDirectory, "degraded_list.csv");
            logger.Info($"Degraded output file: {degradedFilePath}");

            GenerateDegradedDataFile(logger, cleanFilePath, degradedFilePath);
        }

        private static void GenerateCleanDataFile(Logger logger, string cleanFilePath, int recordCount)
        {
            logger.Info($"Requested record count: {recordCount:N0}");
            using var writer = new StreamWriter(cleanFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            CsvExporter.WriteHeader(csv);

            for (int i = 0; i < recordCount; i++)
            {
                var record = Generator.Generate();
                CsvExporter.WriteRecord(csv, record);

                if (i % 1000 == 0 && i != 0)
                {
                    logger.Info($"Generated {i:N0} records");
                }
            }
        }

        private static void GenerateDegradedDataFile(Logger logger, string cleanFilePath, string degradedFilePath)
        {
            using var reader = new StreamReader(cleanFilePath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            using var writer = new StreamWriter(degradedFilePath);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteHeader<MailingRecord>();
            csv.NextRecord();

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
                csv.WriteRecord(record);
                csv.NextRecord();
            }
            logger.Info($"Degradation complete for {degradedRecordCount} records");
        }
    }
}
