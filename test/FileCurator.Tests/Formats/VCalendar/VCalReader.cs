using FileCurator.Formats.VCalendar;
using FileCurator.Tests.BaseClasses;
using System;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.VCalendar
{
    public class VCalReader : TestBaseClass<VCalendarReader>
    {
        public VCalReader()
        {
            TestObject = new VCalendarReader();
        }

        [Fact]
        public void Read()
        {
            var TimeZone = TimeZoneInfo.Local;
            var TestObject = new VCalendarReader();
            var Result = TestObject.Read(File.OpenRead("../../../TestData/TestVCal.vcs"));
            Assert.Equal(0, Result.AttendeeList.Count);
            Assert.False(Result.Cancel);
            Assert.Equal("Networld+Interop Conference and Exhibit\nAtlanta World Congress Center\n Atlanta, Georgia", Result.Content);
            Assert.Equal("Networld+Interop Conference and Exhibit\nAtlanta World Congress Center\n Atlanta, Georgia", Result.Description);
            Assert.Equal(new DateTime(1996, 9, 20, 22, 0, 0) + TimeZone.BaseUtcOffset, Result.EndTime);
            Assert.Null(Result.Location);
            Assert.Equal("", Result.Meta);
            Assert.Null(Result.Organizer);
            Assert.Equal(new DateTime(1996, 9, 18, 14, 30, 0) + TimeZone.BaseUtcOffset, Result.StartTime);
            Assert.Equal("BUSY", Result.Status);
            Assert.Equal("Networld+Interop Conference", Result.Subject);
            Assert.Equal("Networld+Interop Conference", Result.Title);
        }
    }
}