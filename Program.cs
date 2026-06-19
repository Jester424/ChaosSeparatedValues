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

            string filePath = Path.Combine(outputDirectory, "mailing.csv");
            Logger.Info($"Output file: {filePath}");


            int recordCount = 10000;
            Logger.Info($"Requested record count: {recordCount:N0}");

            CsvExporter.Export(filePath, recordCount);

            Logger.Info($"Finished generating {recordCount:N0} records");
        }
    }
}
