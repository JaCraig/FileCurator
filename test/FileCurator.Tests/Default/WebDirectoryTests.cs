using FileCurator.Default;
using FileCurator.Interfaces;
using FileCurator.Tests.BaseClasses;
using System;
using Xunit;

namespace FileCurator.Tests.Default
{
    public class WebDirectoryTests : TestingDirectoryFixture
    {
        [Fact]
        public void Copy()
        {
            var Temp = new WebDirectory("http://www.google.com");
            var Temp2 = new LocalDirectory("./Testing/");
            Temp2.Create();
            while (!Temp2.Exists) { }
            Temp.CopyTo(Temp2);
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
        }

        [Fact]
        public void CreateAndDelete()
        {
            var Temp = new WebDirectory("http://www.google.com");
            Assert.Throws<AggregateException>(() => Temp.Create());
            Assert.True(Temp.Exists);
            Assert.Throws<AggregateException>(() => Temp.Delete());
            Assert.True(Temp.Exists);
        }

        [Fact]
        public void Creation()
        {
            var Temp = new WebDirectory("http://www.google.com");
            Assert.NotNull(Temp);
            Assert.True(Temp.Exists);
            Temp = new WebDirectory(new Uri("http://www.google.com"));
            Assert.NotNull(Temp);
            Assert.True(Temp.Exists);
        }

        [Fact]
        public void Enumeration()
        {
            var Temp = new WebDirectory("http://www.google.com");
            foreach (IFile File in Temp) { }
        }

        [Fact]
        public void Equality()
        {
            var Temp = new WebDirectory("http://www.google.com");
            var Temp2 = new WebDirectory("http://www.google.com");
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
            var Temp = new WebDirectory("http://www.google.com");
            var Temp2 = new LocalDirectory("./Testing/");
            Temp2.Create();
            while (!Temp2.Exists) { }
            Assert.Throws<AggregateException>(() => Temp.MoveTo(Temp2));
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
        }
    }
}