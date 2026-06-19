using Bogus;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChaosSeparatedValues
{
    public static class Generator
    {
        private static readonly Faker<MailingRecord> faker = new Faker<MailingRecord>()
            .RuleFor(x => x.Id, f => f.IndexFaker + 1)
            .RuleFor(x => x.FirstName, f => f.Name.FirstName())
            .RuleFor(x => x.LastName, f => f.Name.LastName())
            .RuleFor(x => x.Address, f => f.Address.StreetAddress())
            .RuleFor(x => x.City, f => f.Address.City())
            .RuleFor(x => x.State, f => f.Address.StateAbbr())
            .RuleFor(x => x.Zip, f => f.Address.ZipCode());

        public static MailingRecord Generate()
        {
            return faker.Generate();
        }
    }
}
