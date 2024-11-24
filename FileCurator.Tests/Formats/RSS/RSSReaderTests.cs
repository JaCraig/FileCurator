using FileCurator.Formats.RSS;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.RSS
{
    public class RSSReaderTests : TestBaseClass<RSSReader>
    {
        public RSSReaderTests()
        {
            TestObject = new RSSReader();
        }

        [Fact]
        public void Read()
        {
            var TestObject = new RSSReader();
            FileCurator.Formats.Data.Interfaces.IFeed Result = TestObject.Read(File.OpenRead("./TestData/TestRSS.rss"));
            _ = Assert.Single(Result);
            Assert.Equal(10, Result.Channels[0].Count);
            Assert.Equal(11957, Result.Content.ReplaceLineEndings("\n").Length);
        }

        [Fact]
        public void Read2()
        {
            var TestObject = new RSSReader();
            FileCurator.Formats.Data.Interfaces.IFeed Result = TestObject.Read(File.OpenRead("./TestData/TestRSS2.rss"));
            _ = Assert.Single(Result);
            Assert.Equal(50, Result.Channels[0].Count);
            Assert.Equal(15485, Result.Content.ReplaceLineEndings("\n").Length);
        }
    }
}