using System;
using System.IO;
using Bogus;
using CsvHelper;
using System.Globalization;

public class MailingRecord
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Zip { get; set; } = string.Empty;
}

public class Program
{
    public static void Main(string[] args)
    {
        // Phase 1: Generate synthetic mailing data and export to CSV

        Logger.Info("Program started");

        string outputDirectory = "Data";
        Directory.CreateDirectory(outputDirectory);
        string filePath = Path.Combine(outputDirectory, "mailing.csv");

        Logger.Info($"Output file: {filePath}");


        var faker = new Faker<MailingRecord>()
            .RuleFor(x => x.Id, f => f.IndexFaker + 1)
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Address, f => f.Address.StreetAddress())
            .RuleFor(x => x.City, f => f.Address.City())
            .RuleFor(x => x.State, f => f.Address.StateAbbr())
            .RuleFor(x => x.Zip, f => f.Address.ZipCode());

        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        csv.WriteHeader<MailingRecord>();
        csv.NextRecord();

        int recordCount = 10000;

        Logger.Info($"Requested record count: {recordCount:N0}");

        for (int i = 0; i < recordCount; i++)
        {
            var record = faker.Generate();
            csv.WriteRecord(record);
            csv.NextRecord();

            if (i % 1000 == 0 && i != 0)
            {
                Logger.Info($"Generated {i:N0} records");
            }
        }

        Logger.Info($"Finished generating {recordCount:N0} records");
    }

    public static class Logger
    {
        private static readonly string logDirectory = "Logs";
        private static readonly string logFile = Path.Combine(logDirectory, $"run-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.log");

        static Logger()
        {
            Directory.CreateDirectory(logDirectory);
        }

        public static void Info(string message)
        {
            Write("INFO", message, ConsoleColor.Green);
        }

        public static void Error(string message)
        {
            Write("ERROR", message, ConsoleColor.Red);
        }

        private static void Write(string level, string message, ConsoleColor color)
        {
            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string logLine = $"{time} {level,-5} {message}";

            // Console output
            Console.Write($"{time} ");

            Console.ForegroundColor = color;
            Console.Write($"{level,-5}");
            Console.ResetColor();

            Console.WriteLine($" {message}");

            // File output
            File.AppendAllText(logFile, logLine + Environment.NewLine);
        }
    }
}
