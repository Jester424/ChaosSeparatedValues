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

        string outputDirectory = "Data";
        Directory.CreateDirectory(outputDirectory);
        string filePath = Path.Combine(outputDirectory, "mailing.csv");


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
        for (int i = 0; i < recordCount; i++)
        {
            var record = faker.Generate();
            csv.WriteRecord(record);
            csv.NextRecord();
        }

        Console.WriteLine($"Generated {recordCount:N0} records to {filePath}");
    }
}
