using FileCurator.Default;
using FileCurator.Interfaces;
using FileCurator.Tests.BaseClasses;
using System.Linq;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class ResourceDirectoryTests : TestBaseClass<ResourceDirectory>
    {
        public ResourceDirectoryTests()
        {
            TestObject = new ResourceDirectory();
        }

        [Fact]
        public void Copy()
        {
            IDirectory Temp = new ResourceDirectory("resource://FileCurator.Tests/");
            if (!Temp.EnumerateFiles().Any())
                return;
            IDirectory Temp2 = new LocalDirectory("./Testing/");
            Temp2.Create();
            while (!Temp2.Exists) { }
            Temp = Temp.CopyTo(Temp2);
            Assert.True(Temp.Exists);
            Assert.True(Temp2.Exists);
            int Count = 0;
            foreach (var Files in Temp2.EnumerateFiles())
            {
                Assert.NotEqual(0, Files.Length);
                ++Count;
            }
            Assert.Equal(1, Count);
            Temp2.Delete();
            while (Temp2.Exists)
            {
            }
        }

        [Fact]
        public void Creation()
        {
            var Temp = new ResourceDirectory("resource://FileCurator.Tests/");
            Assert.NotNull(Temp);
            Assert.True(Temp.Exists);
            Assert.Equal("resource://FileCurator.Tests/", Temp.FullName);
            Assert.Equal("FileCurator.Tests", Temp.Name);
            Assert.Null(Temp.Parent);
        }

        [Fact]
        public void Enumeration()
        {
            var Temp = new ResourceDirectory("resource://FileCurator.Tests/");
            foreach (IFile File in Temp) { }
            Assert.Single(Temp.EnumerateFiles());
            Temp = new ResourceDirectory("resource://FileCurator.Tests/FileCurator.Tests/Resources/");
            foreach (IFile File in Temp) { }
            Assert.Single(Temp.EnumerateFiles());
            Temp = new ResourceDirectory("resource://FileCurator.Tests/FileCurator.Tests/Resources2/");
            foreach (IFile File in Temp) { }
            Assert.Empty(Temp.EnumerateFiles());
        }

        [Fact]
        public void Equality()
        {
            var Temp = new ResourceDirectory("resource://FileCurator.Tests/");
            var Temp2 = new ResourceDirectory("resource://FileCurator.Tests/");
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
            IDirectory Temp = new ResourceDirectory("resource://FileCurator.Tests/");
            if (!Temp.EnumerateFiles().Any())
                return;
            IDirectory Temp2 = new LocalDirectory("./Testing/");
            Temp2.Create();
            while (!Temp2.Exists) { }
            Temp = Temp.MoveTo(Temp2);
            Assert.True(Temp.Exists);
            Assert.True(Temp2.Exists);
            Assert.Equal(Temp2.FullName, Temp.Parent.FullName + "\\");
            int Count = 0;
            foreach (var Files in Temp.EnumerateFiles())
            {
                Assert.NotEqual(0, Files.Length);
                ++Count;
            }
            Assert.Equal(1, Count);
            Temp2.Delete();
            while (Temp2.Exists)
            {
            }
        }
    }
}