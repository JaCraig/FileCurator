using FileCurator;
using FileCurator.Tests.BaseClasses;
using System.Text;
using Xunit;

namespace UnitTests.IO
{
    public class FileInfoTests : TestingDirectoryFixture
    {
        [Fact]
        public void Creation()
        {
            var File = new FileInfo("./Test.txt");
            Assert.NotNull(File);
            Assert.False(File.Exists);
        }

        [Fact]
        public void DeleteExtension()
        {
            var Temp = new DirectoryInfo("./Test");
            Temp.Create();
            for (int x = 0; x < 10; ++x)
            {
                new FileInfo("./Test/" + x + ".txt").Write("Testing this out");
            }
            Temp.EnumerateFiles().Delete();
            Temp.Delete();
        }

        [Fact]
        public void ReadWrite()
        {
            var File = new FileInfo("./Test2.txt");
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