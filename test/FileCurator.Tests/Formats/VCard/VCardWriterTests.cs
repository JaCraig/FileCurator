using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interface;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.VCard;
using FileCurator.Tests.BaseClasses;
using System.IO;
using System.Linq;
using Xunit;

namespace FileCurator.Tests.Formats.VCard
{
    public class VCardWriterTests : TestBaseClass<VCardWriter>
    {
        public VCardWriterTests()
        {
            TestObject = new VCardWriter();
        }

        [Fact]
        public void WriteACard()
        {
            Directory.CreateDirectory("./Results");
            var TestObject = new VCardWriter();
            var ResultReader = new VCardReader();
            using (var ResultFile = File.Open("./Results/WriteACard.vcf", FileMode.OpenOrCreate))
            {
                Assert.True(TestObject.Write(ResultFile, new GenericCard
                {
                    Addresses = new IAddress[]
                    {
                        new Address
                        {
                            Street="111 Floyd Ave.",
                            City="NumberPlace",
                            StateOrProvence="ThatOne",
                            ZipCode="11122",
                            Type="home"
                        }
                    }.ToList(),
                    DirectDial = new IPhoneNumber[]
                    {
                        new PhoneNumber
                        {
                            Number="111-1111",
                            Type="Work"
                        }
                    }.ToList(),
                    Email = new IMailAddress[]
                    {
                        new MailAddress
                        {
                            EmailAddress="something@someplace.com",
                            Type="Work"
                        }
                    }.ToList(),
                    FirstName = "FirstName",
                    LastName = "LastName",
                    MiddleName = "MiddleName",
                    Organization = "OrgName",
                    Prefix = "Prefix",
                    Suffix = "Suffix",
                    Title = "Title",
                    Url = "http://www.somewhere.com"
                }));
            }
            using (var ResultFile = File.Open("./Results/WriteACard.vcf", FileMode.OpenOrCreate))
            {
                var Result = ResultReader.Read(ResultFile);
                Assert.Contains(Result.DirectDial, x => x.Type == "WORK" && x.Number == "111-1111");
                Assert.Contains(Result.Email, x => x.Type == "WORK" && x.EmailAddress == "something@someplace.com");
                Assert.Contains(Result.Addresses, x => x.Type == "HOME" && x.Street == "111 Floyd Ave." && x.City == "NumberPlace" && x.StateOrProvence == "ThatOne" && x.ZipCode == "11122");
                Assert.Equal("FirstName", Result.FirstName);
                Assert.Equal("Prefix FirstName MiddleName LastName Suffix", Result.FullName);
                Assert.Equal("LastName", Result.LastName);
                Assert.Equal("MiddleName", Result.MiddleName);
                Assert.Equal("OrgName", Result.Organization);
                Assert.Equal("Prefix", Result.Prefix);
                Assert.Empty(Result.Relationships);
                Assert.Equal("Suffix", Result.Suffix);
                Assert.Equal("Title", Result.Title);
                Assert.Equal("http://www.somewhere.com", Result.Url);
            }
        }
    }
}