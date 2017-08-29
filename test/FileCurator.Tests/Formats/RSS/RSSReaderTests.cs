using FileCurator.Formats.RSS;
using FileCurator.Tests.BaseClasses;
using System.IO;
using System.Linq;
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
            Assert.Equal(10, Result.Channels.First().Count);
            Assert.Equal(11957, Result.Content.Length);
        }
    }
}