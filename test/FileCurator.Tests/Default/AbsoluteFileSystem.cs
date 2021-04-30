using FileCurator.Default;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class AbsoluteFileSystemTests : TestBaseClass<AbsoluteLocalFileSystem>
    {
        public AbsoluteFileSystemTests()
        {
            TestObject = new AbsoluteLocalFileSystem();
        }

        [Fact]
        public void CanHandle()
        {
            var Temp = new AbsoluteLocalFileSystem();
            Assert.True(Temp.CanHandle(@"C:\TestPath\Yay"));
            Assert.True(Temp.CanHandle(@"F:\TestPath\Yay"));
            Assert.True(Temp.CanHandle(@"Q:\TestPath\Yay"));
        }

        [Fact]
        public void Creation()
        {
            var Temp = new AbsoluteLocalFileSystem();
            Assert.NotNull(Temp);
            Assert.Equal("Absolute Local", Temp.Name);
        }

        [Fact]
        public void Directory()
        {
            var Temp = new AbsoluteLocalFileSystem();
            var Dir = Temp.Directory(@"C:\");
            Assert.NotNull(Dir);
            Assert.IsType<LocalDirectory>(Dir);
            Assert.True(Dir.Exists);
        }

        [Fact]
        public void File()
        {
            var Temp = new AbsoluteLocalFileSystem();
            var TestFile = Temp.File(@"C:\Test.txt");
            Assert.NotNull(TestFile);
            Assert.IsType<LocalFile>(TestFile);
            Assert.False(TestFile.Exists);
        }
    }
}