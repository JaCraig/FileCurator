using FileCurator.Default;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class ResourceFileTests : TestingDirectoryFixture
    {
        [Fact]
        public void Creation()
        {
            var File = new ResourceFile("resource://FileCurator.Tests/FileCurator.Tests.Resources.TextFile1.txt");
            Assert.NotNull(File);
            Assert.True(File.Exists);
            Assert.NotNull(File.Directory);
            Assert.Equal(".txt", File.Extension);
            Assert.Equal("resource://FileCurator.Tests/FileCurator.Tests.Resources.TextFile1.txt", File.FullName);
            Assert.Equal(32, File.Length);
            Assert.Equal("FileCurator.Tests.Resources.TextFile1.txt", File.Name);
        }

        [Fact]
        public void ReadWrite()
        {
            var File = new ResourceFile("resource://FileCurator.Tests/FileCurator.Tests.Resources.TextFile1.txt");
            Assert.Equal("This is a resource file test.", File.Read());
            Assert.Equal("This is a resource file test.", (string)File);
        }
    }
}