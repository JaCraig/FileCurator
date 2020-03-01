using FileCurator.Default;
using FileCurator.Interfaces;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests
{
    public class FileCuratorTests : TestingDirectoryFixture
    {
        [Fact]
        public void Creation()
        {
            var Temp = new FileSystem(new IFileSystem[] { new AbsoluteLocalFileSystem(), new NetworkFileSystem(), new RelativeLocalFileSystem() });
            Assert.NotNull(Temp);
        }

        [Fact]
        public void Directory()
        {
            var Temp = new FileSystem(new IFileSystem[] { new AbsoluteLocalFileSystem(), new NetworkFileSystem(), new RelativeLocalFileSystem() });
            var Dir = Temp.Directory(@"C:\");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
            Dir = Temp.Directory(@"\\localhost\C$\");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
            Dir = Temp.Directory("./");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
            Dir = Temp.Directory("../");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
            Dir = Temp.Directory("~/");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
        }

        [Fact]
        public void File()
        {
            var Temp = new FileSystem(new IFileSystem[] { new AbsoluteLocalFileSystem(), new NetworkFileSystem(), new RelativeLocalFileSystem() });
            var TestFile = Temp.File(@"C:\Test.txt");
            Assert.NotNull(TestFile);
            Assert.IsType<LocalFile>(TestFile);
            Assert.False(TestFile.Exists);
            TestFile = Temp.File(@"~\Test.txt");
            Assert.NotNull(TestFile);
            Assert.IsType<LocalFile>(TestFile);
            Assert.False(TestFile.Exists);
            TestFile = Temp.File(@"\\localhost\C$\Test.txt");
            Assert.NotNull(TestFile);
            Assert.IsType<LocalFile>(TestFile);
            Assert.False(TestFile.Exists);
        }
    }
}