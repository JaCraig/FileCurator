using FileCurator.Formats.PowerPoint;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.PowerPoint
{
    public class PowerPointFormatTests : TestBaseClass<PowerPointFormat>
    {
        public PowerPointFormatTests()
        {
            TestObject = new PowerPointFormat();
        }

        [Fact]
        public void ReadPPSX()
        {
            var TestObject = new PowerPointFormat();
            using var TestFile = File.OpenRead("../../../TestData/TestPPSX.ppsx");
            var Result = TestObject.Read(TestFile);
            Assert.Equal("  rewqqawer vcxzasdf\n  Asdfpof\t\t fadsasasdfasdf\n  This is a test Testing", Result.ToString());
            Assert.Equal("Asdfpof\t\t", Result.Title);
        }

        [Fact]
        public void ReadPPTX()
        {
            var TestObject = new PowerPointFormat();
            using var TestFile = File.OpenRead("../../../TestData/TestPPTX.pptx");
            var Result = TestObject.Read(TestFile);
            Assert.Equal("  YAYYYAYAYAYA asdfoidpasfoasdfhjoidpasfhhf\n  Title1 Something something darkside", Result.ToString());
            Assert.Equal("Title1", Result.Title);
        }
    }
}