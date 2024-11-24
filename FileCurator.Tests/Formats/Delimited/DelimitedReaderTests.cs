using FileCurator.Formats.Delimited;
using FileCurator.Tests.BaseClasses;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.Delimited
{
    public class DelimitedReaderTests : TestBaseClass<DelimitedReader>
    {
        public DelimitedReaderTests()
        {
            TestObject = new DelimitedReader();
        }

        [Fact]
        public void Read()
        {
            var TestObject = new DelimitedReader();
            var Result = TestObject.Read(File.OpenRead("./TestData/TestCSV.csv"));
            Assert.Equal(3, Result.Rows.Count);
            Assert.Equal(6, Result.Columns.Count);
            Assert.Equal("Header 1", Result.Columns[0]);
            Assert.Equal("Header 2", Result.Columns[1]);
            Assert.Equal("Header 3", Result.Columns[2]);
            Assert.Equal("Header 4", Result.Columns[3]);
            Assert.Equal("Header 5", Result.Columns[4]);
            Assert.Equal("Header 6", Result.Columns[5]);
            var TempData = new List<List<string>>
            {
                new List<string>(),
                new List<string>(),
                new List<string>()
            };
            TempData[0].AddRange(new[] { "This", "is", "a", "test", "CSV", "file" });
            TempData[1].AddRange(new[] { "Tons", "of", "data", "in here", "is", "super" });
            TempData[2].Add("important");
            for (int x = 0; x < TempData.Count; ++x)
            {
                for (int y = 0; y < TempData[x].Count; ++y)
                {
                    Assert.Equal(TempData[x][y], Result.Rows[x].Cells[y].Content);
                }
            }
        }
    }
}