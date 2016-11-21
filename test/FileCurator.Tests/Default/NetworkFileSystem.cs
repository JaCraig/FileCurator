using FileCurator.Default;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class NetworkFileSystemTests : TestingDirectoryFixture
    {
        [Fact]
        public void CanHandle()
        {
            var Temp = new NetworkFileSystem();
            Assert.True(Temp.CanHandle(@"\\localhost\C$\TestPath\Yay"));
        }

        [Fact]
        public void Creation()
        {
            var Temp = new NetworkFileSystem();
            Assert.NotNull(Temp);
            Assert.Equal("Network", Temp.Name);
        }

        [Fact]
        public void Directory()
        {
            var Temp = new NetworkFileSystem();
            var Dir = Temp.Directory(@"\\localhost\C$\");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
        }

        [Fact]
        public void File()
        {
            var Temp = new NetworkFileSystem();
            var File = Temp.File(@"\\localhost\C$\Test.txt");
            Assert.NotNull(File);
            Assert.IsType<LocalFile>(File);
            Assert.False(File.Exists);
        }
    }
}