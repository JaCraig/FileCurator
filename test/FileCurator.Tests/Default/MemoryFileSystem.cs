using FileCurator.Default.Memory;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class MemoryFileSystemTests : TestBaseClass<MemoryFileSystem>
    {
        public MemoryFileSystemTests()
        {
            TestObject = new MemoryFileSystem();
        }

        [Fact]
        public void CanHandle()
        {
            var Temp = new MemoryFileSystem();
            Assert.True(Temp.CanHandle(@"mem://localhost\C$\TestPath\Yay"));
        }

        [Fact]
        public void Creation()
        {
            var Temp = new MemoryFileSystem();
            Assert.NotNull(Temp);
            Assert.Equal("Memory", Temp.Name);
        }

        [Fact]
        public void Directory()
        {
            var Temp = new MemoryFileSystem();
            var Dir = Temp.Directory(@"mem://localhost\C$\");
            Assert.NotNull(Dir);
            Assert.IsType<MemoryDirectory>(Dir);
            Assert.False(Dir.Exists);
        }

        [Fact]
        public void File()
        {
            var Temp = new MemoryFileSystem();
            var TestFile = Temp.File(@"mem://localhost\C$\Test.txt");
            Assert.NotNull(TestFile);
            Assert.IsType<MemoryFile>(TestFile);
            Assert.True(TestFile.Exists);
        }
    }
}