using FileCurator.Formats.VCalendar;
using FileCurator.Tests.BaseClasses;
using System;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.VCalendar
{
    public class VCalReader : TestingDirectoryFixture
    {
        [Fact]
        public void Read()
        {
            var TestObject = new VCalendarReader();
            var Result = TestObject.Read(File.OpenRead("../../../TestData/TestVCal.vcs"));
            Assert.Equal(0, Result.AttendeeList.Count);
            Assert.Equal(false, Result.Cancel);
            Assert.Equal("Networld+Interop Conference and Exhibit\nAtlanta World Congress Center\n Atlanta, Georgia", Result.Content);
            Assert.Equal("Networld+Interop Conference and Exhibit\nAtlanta World Congress Center\n Atlanta, Georgia", Result.Description);
            Assert.Equal(new DateTime(1996, 9, 20, 17, 0, 0), Result.EndTime);
            Assert.Equal(null, Result.Location);
            Assert.Equal("", Result.Meta);
            Assert.Equal(null, Result.Organizer);
            Assert.Equal(new DateTime(1996, 9, 18, 9, 30, 0), Result.StartTime);
            Assert.Equal("BUSY", Result.Status);
            Assert.Equal("Networld+Interop Conference", Result.Subject);
            Assert.Equal("Networld+Interop Conference", Result.Title);
        }
    }
}