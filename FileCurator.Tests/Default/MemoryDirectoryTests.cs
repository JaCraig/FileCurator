using FileCurator.Default.Memory;
using FileCurator.Interfaces;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class MemoryDirectoryTests : TestBaseClass<MemoryDirectory>
    {
        public MemoryDirectoryTests()
        {
            TestObject = new MemoryDirectory();
        }

        [Fact]
        public void Copy()
        {
            var Temp = new MemoryDirectory("mem://Test");
            var Temp2 = new MemoryDirectory("mem://Test2");
            Temp.Create();
            Temp2.Create();
            var Temp3 = Temp2.CopyTo(Temp);
            Assert.True(Temp.Exists);
            Assert.True(Temp2.Exists);
            Assert.True(Temp3.Exists);
            Assert.Equal(Temp, Temp3);
            Assert.NotSame(Temp, Temp2);
            Assert.NotSame(Temp2, Temp3);
            Temp.Delete();
            Temp2.Delete();
            Assert.False(Temp.Exists);
        }

        [Fact]
        public void CreateAndDelete()
        {
            var Temp = new MemoryDirectory("mem://Test");
            Temp.Create();
            Assert.True(Temp.Exists);
            Temp.Delete();
            Assert.False(Temp.Exists);
        }

        [Fact]
        public void Creation()
        {
            var Temp = new MemoryDirectory("mem://Test");
            Assert.NotNull(Temp);
            Assert.False(Temp.Exists);
        }

        [Fact]
        public void Enumeration()
        {
            foreach (IFile File in new MemoryDirectory("mem://Test")) { }
        }

        [Fact]
        public void Equality()
        {
            var Temp = new MemoryDirectory("mem://A");
            var Temp2 = new MemoryDirectory("mem://A");
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
        public void Move()
        {
            IDirectory Temp = new MemoryDirectory("mem://Test/A");
            IDirectory Temp2 = new MemoryDirectory("mem://Test2/");
            Temp.Create();
            Temp2.Create();
            Temp2 = Temp2.MoveTo(Temp);
            Assert.True(Temp.Exists);
            Assert.True(Temp2.Exists);
            Assert.Equal(Temp.FullName, Temp2.Parent.FullName);
            Temp.Delete();
            Assert.False(Temp.Exists);
        }
    }
}