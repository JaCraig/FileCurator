using FileCurator.Formats.Txt;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.Txt
{
    public class TxtFormatTests : TestBaseClass<TxtFormat>
    {
        public TxtFormatTests()
        {
            TestObject = new TxtFormat();
        }

        [Fact]
        public void ReadWrite()
        {
            var TestObject = new TxtFormat();
            using (var ResultFile = File.Open("./Results/TxtWrite.txt", FileMode.OpenOrCreate))
            {
                using (var TestFile = File.OpenRead("../../../TestData/TestTXT.txt"))
                {
                    Assert.True(TestObject.Write(ResultFile, TestObject.Read(TestFile)));
                }
            }
            using (var ResultFile = File.Open("./Results/TxtWrite.txt", FileMode.OpenOrCreate))
            {
                var Result = TestObject.Read(ResultFile);
                Assert.Equal("This is a test docx", Result.ToString());
            }
        }
    }
}