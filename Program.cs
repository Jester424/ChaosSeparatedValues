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
            Logger.Info($"Output file: {cleanFilePath}");


            int recordCount = 500;
            Logger.Info($"Requested record count: {recordCount:N0}");

            CsvExporter.Export(cleanFilePath, recordCount);

            Logger.Info($"Finished generating {recordCount:N0} records");
        }
    }
}
