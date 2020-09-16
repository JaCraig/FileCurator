using FileCurator.Formats.VCard;
using FileCurator.Tests.BaseClasses;
using System.IO;
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
            Assert.Contains(Result.DirectDial, x => x.Type == "HOME,VOICE" && x.Number == "555-555-1111");
            Assert.Contains(Result.DirectDial, x => x.Type == "WORK,VOICE" && x.Number == "555-555-1112");
            Assert.Contains(Result.DirectDial, x => x.Type == "CELL,VOICE" && x.Number == "555-555-1113");
            Assert.Contains(Result.Email, x => x.Type == "HOME" && x.EmailAddress == "home@example.com");
            Assert.Contains(Result.Email, x => x.Type == "WORK" && x.EmailAddress == "work@example.com");
            Assert.Contains(Result.Addresses, x => x.Type == "WORK" && x.Street == "WorkStreet");
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