using FileCurator.Formats.Data;
using FileCurator.Formats.VCalendar;
using FileCurator.Tests.BaseClasses;
using System;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.VCalendar
{
    public class VCalWriterTests : TestBaseClass<VCalendarWriter>
    {
        public VCalWriterTests()
        {
            TestObject = new VCalendarWriter();
        }

        [Fact]
        public void WriteACalendar()
        {
            Directory.CreateDirectory("./Results");
            var TestObject = new VCalendarWriter();
            var ResultReader = new VCalendarReader();
            using (var ResultFile = File.Open("./Results/WriteACalendar.vcs", FileMode.OpenOrCreate))
            {
                Assert.True(TestObject.Write(ResultFile, new GenericCalendar
                {
                    Description = "This is my description of the event",
                    EndTime = new DateTime(2017, 1, 1, 15, 0, 0),
                    StartTime = new DateTime(2017, 1, 1, 12, 20, 0),
                    Subject = "This is my subject",
                    Location = "That Place"
                }));
            }
            using (var ResultFile = File.Open("./Results/WriteACalendar.vcs", FileMode.OpenOrCreate))
            {
                var Result = ResultReader.Read(ResultFile);
                Assert.Equal("This is my description of the event", Result.Description);
                Assert.Equal(new DateTime(2017, 1, 1, 15, 0, 0), Result.EndTime);
                Assert.Equal(new DateTime(2017, 1, 1, 12, 20, 0), Result.StartTime);
                Assert.Equal("This is my subject", Result.Subject);
                Assert.Equal("That Place", Result.Location);
            }
        }
    }
}