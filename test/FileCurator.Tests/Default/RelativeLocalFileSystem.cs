using FileCurator.Default;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class RelativeLocalFileSystemTests : TestingDirectoryFixture
    {
        [Fact]
        public void CanHandle()
        {
            var Temp = new RelativeLocalFileSystem();
            Assert.True(Temp.CanHandle(@".\TestPath\Yay"));
            Assert.True(Temp.CanHandle(@"..\TestPath\Yay"));
            Assert.True(Temp.CanHandle(@"~\TestPath\Yay"));
        }

        [Fact]
        public void Creation()
        {
            var Temp = new RelativeLocalFileSystem();
            Assert.NotNull(Temp);
            Assert.Equal("Relative Local", Temp.Name);
        }

        [Fact]
        public void Directory()
        {
            var Temp = new RelativeLocalFileSystem();
            var Dir = Temp.Directory(@"./");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
            Dir = Temp.Directory(@"../");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
            Dir = Temp.Directory(@"~/");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
        }

        [Fact]
        public void File()
        {
            var Temp = new RelativeLocalFileSystem();
            var TestFile = Temp.File(@"~\Test.txt");
            Assert.NotNull(TestFile);
            Assert.IsType<LocalFile>(TestFile);
            Assert.False(TestFile.Exists);
        }
    }
}