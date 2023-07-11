using FileCurator.Formats.Mime;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.Mime
{
    public class MimeFormatTests : TestBaseClass<MimeFormat>
    {
        public MimeFormatTests()
        {
            TestObject = new MimeFormat();
        }

        [Fact]
        public void Read()
        {
            var TestObject = new MimeFormat();
            using var TestFile = File.OpenRead("./TestData/TestEml.eml");
            var Result = TestObject.Read(TestFile);
            Assert.NotNull(Result.ToString());
        }

        [Fact]
        public void Write()
        {
            Directory.CreateDirectory("./Results");
            var TestObject = new MimeFormat();
            using var ResultFile = File.Open("./Results/TestEML.eml", FileMode.OpenOrCreate);
            using var TestFile = File.OpenRead("./TestData/TestEml.eml");
            Assert.False(TestObject.Write(ResultFile, TestObject.Read(TestFile)));
        }
    }
}