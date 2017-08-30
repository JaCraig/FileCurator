﻿using FileCurator.Formats.VCard;
using FileCurator.Tests.BaseClasses;
using System.IO;
using System.Linq;
using Xunit;

namespace FileCurator.Tests.Formats.VCard
{
    public class VCardReaderTests : TestingDirectoryFixture
    {
        [Fact]
        public void Read()
        {
            var TestObject = new VCardReader();
            var Result = TestObject.Read(File.OpenRead("../../../TestData/TestVCF.vcf"));
            Assert.True(Result.DirectDial.Any(x => x.Type == "HOME,VOICE" && x.Number == "555-555-1111"));
            Assert.True(Result.DirectDial.Any(x => x.Type == "WORK,VOICE" && x.Number == "555-555-1112"));
            Assert.True(Result.DirectDial.Any(x => x.Type == "CELL,VOICE" && x.Number == "555-555-1113"));
            Assert.True(Result.Email.Any(x => x.Type == "HOME" && x.EmailAddress == "home@example.com"));
            Assert.True(Result.Email.Any(x => x.Type == "WORK" && x.EmailAddress == "work@example.com"));
            Assert.Equal("FirstName", Result.FirstName);
            Assert.Equal("Prefix FirstName MiddleName LastName Suffix", Result.FullName);
            Assert.Equal("LastName", Result.LastName);
            Assert.Equal("MiddleName", Result.MiddleName);
            Assert.Equal("Organization2;Department2", Result.Organization);
            Assert.Equal("Prefix", Result.Prefix);
            Assert.Equal(0, Result.Relationships.Count);
            Assert.Equal("Suffix", Result.Suffix);
            Assert.Equal("Title2", Result.Title);
            Assert.Equal("http://www.custom.com", Result.Url);
        }
    }
}