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
            Directory.CreateDirectory("./Results");
            var TestObject = new RSSWriter();
            var TestReader = new RSSReader();
            var ResultReader = new RSSReader();
            using (var ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate))
            {
                using var TestFile = File.OpenRead("./TestData/TestRSS.rss");
                Assert.True(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
            }
            using (var ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate))
            {
                var Result = ResultReader.Read(ResultFile);
                Assert.Single(Result);
                Assert.Equal(10, Result.Channels[0].Count);
                Assert.Equal(12056, Result.Content.Length);
            }
        }

        [Fact]
        public void WriteNotAFeed()
        {
            Directory.CreateDirectory("./Results");
            var TestObject = new RSSWriter();
            var TestReader = new TxtReader();
            using var ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate);
            using var TestFile = File.OpenRead("./TestData/TestTxt.txt");
            Assert.False(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
        }
    }
}