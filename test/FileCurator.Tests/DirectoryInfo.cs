using FileCurator;
using FileCurator.Interfaces;
using FileCurator.Tests.BaseClasses;
using System;
using System.Linq;
using Xunit;

namespace UnitTests.IO
{
    public class DirectoryInfoTests : TestingDirectoryFixture
    {
        [Fact]
        public void CreateAndDelete()
        {
            var Temp = new DirectoryInfo("./Test");
            Temp.Create();
            Assert.True(Temp.Exists);
            Temp.Delete();
            Assert.False(Temp.Exists);
        }

        [Fact]
        public void Creation()
        {
            var Temp = new DirectoryInfo(".");
            Assert.NotNull(Temp);
            Assert.True(Temp.Exists);
            Temp = new DirectoryInfo(new DirectoryInfo("."));
            Assert.NotNull(Temp);
            Assert.True(Temp.Exists);
        }

        [Fact]
        public void DeleteExtension()
        {
            var Temp = new DirectoryInfo("./Test");
            Temp.Create();
            for (int x = 0; x < 10; ++x)
            {
                new DirectoryInfo("./Test/" + x).Create();
            }
            Temp.EnumerateDirectories().Delete();
            Temp.Delete();
        }

        [Fact]
        public void Enumeration()
        {
            new DirectoryInfo("~/Logs/").Delete();
            new DirectoryInfo("~/App_Data/").Delete();
            var Temp = new DirectoryInfo(".");
            foreach (IFile File in Temp) { }
            Assert.Equal(6, Temp.EnumerateDirectories().Count());
            Assert.Equal(7, Temp.EnumerateFiles().Count());
            Assert.Equal(6, Temp.EnumerateDirectories(x => x.Created < DateTime.UtcNow).Count());
            Assert.Equal(7, Temp.EnumerateFiles(x => x.Created < DateTime.UtcNow).Count());
        }

        [Fact]
        public void Equality()
        {
            var Temp = new DirectoryInfo(".");
            var Temp2 = new DirectoryInfo(".");
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
            IDirectory Temp = new DirectoryInfo("./Test");
            IDirectory Temp2 = new DirectoryInfo("./Test2");
            Temp.Create();
            Temp2.Create();
            Temp2 = Temp2.MoveTo(Temp);
            Assert.True(Temp.Exists);
            Assert.True(Temp2.Exists);
            Assert.Equal(Temp, Temp2.Parent);
            Temp.Delete();
            Assert.False(Temp.Exists);
        }
    }
}