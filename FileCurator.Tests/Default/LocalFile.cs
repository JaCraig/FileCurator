using FileCurator.Default;
using FileCurator.Tests.BaseClasses;
using System.Text;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class LocalFileTests : TestBaseClass<LocalFile>
    {
        public LocalFileTests()
        {
            TestObject = new LocalFile();
        }

        [Fact]
        public void Creation()
        {
            var File = new LocalFile("./Test.txt");
            Assert.NotNull(File);
            Assert.False(File.Exists);
        }

        [Fact]
        public void ReadWrite()
        {
            var File = new LocalFile("./Test.txt");
            File.Write("Testing this out");
            Assert.True(File.Exists);
            Assert.Equal("Testing this out", File.Read());
            Assert.Equal("Testing this out", File);
            Assert.Equal(Encoding.ASCII.GetBytes("Testing this out"), File.ReadBinary());
            Assert.Equal(Encoding.ASCII.GetBytes("Testing this out"), File);
            File.Delete();
        }
    }
}