﻿using FileCurator.Default;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class NetworkFileSystemTests : TestBaseClass<NetworkFileSystem>
    {
        public NetworkFileSystemTests()
        {
            TestObject = new NetworkFileSystem();
        }

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
        }

        [Fact]
        public void File()
        {
            var Temp = new NetworkFileSystem();
            var TestFile = Temp.File(@"\\localhost\C$\Test.txt");
            Assert.NotNull(TestFile);
            Assert.IsType<LocalFile>(TestFile);
            Assert.False(TestFile.Exists);
        }
    }
}