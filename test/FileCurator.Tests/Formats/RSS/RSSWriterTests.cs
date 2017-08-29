using FileCurator.Formats.RSS;
using FileCurator.Formats.Txt;
using FileCurator.Tests.BaseClasses;
using System.IO;
using System.Linq;
using Xunit;

namespace FileCurator.Tests.Formats.RSS
{
    public class RSSWriterTests : TestingDirectoryFixture
    {
        [Fact]
        public void WriteAFeed()
        {
            var TestObject = new RSSWriter();
            var TestReader = new RSSReader();
            var ResultReader = new RSSReader();
            using (var ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate))
            {
                using (var TestFile = File.OpenRead("../../../TestData/TestRSS.rss"))
                {
                    Assert.True(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
                }
            }
            using (var ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate))
            {
                var Result = ResultReader.Read(ResultFile);
                Assert.Equal(1, Result.Count);
                Assert.Equal(10, Result.Channels.First().Count);
                Assert.Equal(11957, Result.Content.Length);
            }
        }

        [Fact]
        public void WriteNotAFeed()
        {
            var TestObject = new RSSWriter();
            var TestReader = new TxtReader();
            using (var ResultFile = File.Open("./Results/WriteAFeed.rss", FileMode.OpenOrCreate))
            {
                using (var TestFile = File.OpenRead("../../../TestData/TestTxt.txt"))
                {
                    Assert.False(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
                }
            }
        }
    }
}