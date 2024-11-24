using FileCurator.Formats.RSS;
using FileCurator.Formats.Txt;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.RSS
{
    public class RSSWriterTests : TestBaseClass<RSSWriter>
    {
        public RSSWriterTests()
        {
            TestObject = new RSSWriter();
        }

        [Fact]
        public void WriteAFeed()
        {
            _ = Directory.CreateDirectory("./Results");
            var TestObject = new RSSWriter();
            var TestReader = new RSSReader();
            var ResultReader = new RSSReader();
            using (FileStream ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate))
            {
                using FileStream TestFile = File.OpenRead("./TestData/TestRSS.rss");
                Assert.True(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
            }
            using (FileStream ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate))
            {
                FileCurator.Formats.Data.Interfaces.IFeed Result = ResultReader.Read(ResultFile);
                _ = Assert.Single(Result);
                Assert.Equal(10, Result.Channels[0].Count);
                Assert.Equal(11957, Result.Content.ReplaceLineEndings("\n").Length);
            }
        }

        [Fact]
        public void WriteNotAFeed()
        {
            _ = Directory.CreateDirectory("./Results");
            var TestObject = new RSSWriter();
            var TestReader = new TxtReader();
            using FileStream ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate);
            using FileStream TestFile = File.OpenRead("./TestData/TestTXT.txt");
            Assert.False(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
        }
    }
}