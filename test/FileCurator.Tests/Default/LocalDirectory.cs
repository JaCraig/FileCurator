using FileCurator.Default;
using FileCurator.Interfaces;
using FileCurator.Tests.BaseClasses;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class LocalDirectoryTests : TestBaseClass<LocalDirectory>
    {
        public LocalDirectoryTests()
        {
            TestObject = new LocalDirectory();
        }

        [Fact]
        public void Copy()
        {
            var Temp = new LocalDirectory("./Test");
            var Temp2 = new LocalDirectory("./Test2");
            Temp.Create();
            Temp2.Create();
            var Temp3 = Temp2.CopyTo(Temp);
            Assert.True(Temp.Exists);
            Assert.True(Temp2.Exists);
            Assert.True(Temp3.Exists);
            Assert.Equal(Temp, Temp3);
            Assert.NotEqual(Temp, Temp2);
            Assert.NotEqual(Temp2, Temp3);
            Temp.Delete();
            Temp2.Delete();
            Assert.False(Temp.Exists);
        }

        [Fact]
        public void CreateAndDelete()
        {
            var Temp = new LocalDirectory("./Test");
            Temp.Create();
            Assert.True(Temp.Exists);
            Temp.Delete();
            Assert.False(Temp.Exists);
        }

        [Fact]
        public void Creation()
        {
            var Temp = new LocalDirectory(".");
            Assert.NotNull(Temp);
            Assert.True(Temp.Exists);
            Temp = new LocalDirectory(new System.IO.DirectoryInfo("."));
            Assert.NotNull(Temp);
            Assert.True(Temp.Exists);
        }

        [Fact]
        public void Enumeration()
        {
            var Temp = new LocalDirectory(".");
            foreach (IFile File in Temp) { }
        }

        [Fact]
        public void Equality()
        {
            var Temp = new LocalDirectory(".");
            var Temp2 = new LocalDirectory(".");
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
            IDirectory Temp = new LocalDirectory("./Test");
            IDirectory Temp2 = new LocalDirectory("./Test2");
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