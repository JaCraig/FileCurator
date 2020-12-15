using FileCurator.Formats.RSS;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.RSS
{
    public class RSSReaderTests : TestingDirectoryFixture
    {
        [Fact]
        public void Read()
        {
            var TestObject = new RSSReader();
            var Result = TestObject.Read(File.OpenRead("../../../TestData/TestRSS.rss"));
            Assert.Equal(1, Result.Count);
            Assert.Equal(10, Result.Channels[0].Count);
            Assert.Equal(12056, Result.Content.Length);
        }

        [Fact]
        public void Read2()
        {
            var TestObject = new RSSReader();
            var Result = TestObject.Read(File.OpenRead("../../../TestData/TestRSS2.rss"));
            Assert.Equal(1, Result.Count);
            Assert.Equal(50, Result.Channels[0].Count);
            Assert.Equal(15485, Result.Content.Length);
        }
    }
}