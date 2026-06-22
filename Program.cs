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
            var logger = new Logger(appContext);

            var outputDirectory = appContext.RunName;
            Directory.CreateDirectory(outputDirectory);

            logger.Info("Program started");

            var cleanFilePath = Path.Combine(outputDirectory, "clean_list.csv");
            var degradedFilePath = Path.Combine(outputDirectory, "degraded_list.csv");
            var qualityReportPath = Path.Combine(outputDirectory, "data_report.txt");

            logger.Info($"Clean output file: {cleanFilePath}");

            var recordCount = 10_000;

            GenerateCleanDataFile(logger, cleanFilePath, recordCount);

            logger.Info($"Finished generating {recordCount:N0} records");
            logger.Info($"Beginning data degradation");

            logger.Info($"Degraded output file: {degradedFilePath}");

            GenerateDegradedDataFile(logger, cleanFilePath, degradedFilePath);

            GenerateDataReport(logger, degradedFilePath, qualityReportPath);
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
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            CsvExporter.WriteHeader(csvWriter);

            double degradationRate = 0.10;

            var random = new Random();
            int degradedRecordCount = 0;
            int totalRecords = 0;
            foreach (var record in csvReader.GetRecords<MailingRecord>())
            {
                totalRecords++;

                if (random.NextDouble() < degradationRate)
                {
                    degradedRecordCount++;
                    DataDegrader.Degrade(record);
                }
                CsvExporter.WriteRecord(csvWriter, record);
            }
            logger.Info($"Read {totalRecords:N0} records");
            logger.Info($"Degradation complete for {degradedRecordCount:N0} records");
        }

        private static void GenerateDataReport(Logger logger, string degradedFilePath, string qualityReportPath)
        {
            using var reader = new StreamReader(degradedFilePath);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var issues = new List<DataQualityIssue>();

            var recordsExamined = 0;

            foreach (var record in csvReader.GetRecords<MailingRecord>())
            {
                recordsExamined++;
                if (string.IsNullOrWhiteSpace(record.City))
                {
                    issues.Add(new DataQualityIssue(
                        record.Id.ToString(),
                        nameof(record.City),
                        "City is missing"));
                }
            }

            var issuesFound = issues.Count;

            using var writer = new StreamWriter(qualityReportPath);
            using var csvWriter = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csvWriter.WriteComment($"Records examined: {recordsExamined:N0}");
            csvWriter.NextRecord();
            csvWriter.NextRecord();

            csvWriter.WriteComment($"Issues found: {issuesFound:N0}");
            csvWriter.NextRecord();
            csvWriter.NextRecord();

            csvWriter.WriteHeader<DataQualityIssue>();
            csvWriter.NextRecord();

            foreach (var issue in issues)
            {
                csvWriter.WriteRecord(issue);
                csvWriter.NextRecord();
            }
        }
    }
}
