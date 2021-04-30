using FileCurator.Default.Memory;
using FileCurator.Tests.BaseClasses;
using System.Text;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class MemoryFileTests : TestBaseClass<MemoryFile>
    {
        public MemoryFileTests()
        {
            TestObject = new MemoryFile();
        }

        [Fact]
        public void Clone()
        {
            var Temp = new MemoryFile("mem://code.jquery.com/jquery-1.10.2.min.js", null);
            Temp.Write("Testing this out");
            var Temp2 = new MemoryFile("mem://code.jquery.com/jquery-1.10.2.min.js", null);
            Temp2.Write("Testing this out");
            Assert.True(Temp == Temp2);
            Assert.True(Temp.Equals(Temp2));
            Assert.Equal(0, Temp.CompareTo(Temp2));
            Assert.False(Temp < Temp2);
            Assert.False(Temp > Temp2);
            Assert.True(Temp <= Temp2);
            Assert.True(Temp >= Temp2);
            Assert.False(Temp != Temp2);
        }

        [Fact]
        public void Creation()
        {
            var File = new MemoryFile("mem://code.jquery.com/jquery-1.10.2.min.js", null);
            Assert.NotNull(File);
            Assert.True(File.Exists);
        }

        [Fact]
        public void ReadWrite()
        {
            var File = new MemoryFile("mem://Test.txt", null);
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