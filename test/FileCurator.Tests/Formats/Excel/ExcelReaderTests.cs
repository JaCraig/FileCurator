using FileCurator.Formats.Excel;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.Excel
{
    public class ExcelReaderTests : TestingDirectoryFixture
    {
        [Fact]
        public void Read()
        {
            var TestObject = new ExcelReader();
            var Result = TestObject.Read(File.OpenRead("../../../TestData/TestXLSX.xlsx"));
            Assert.Equal(2, Result.Rows.Count);
            Assert.Equal(2, Result.Columns.Count);
            Assert.Equal("Test", Result.Columns[0]);
            Assert.Equal("Data", Result.Columns[1]);
            Assert.Equal("Goes", Result.Rows[0].Cells[0].Content);
            Assert.Equal("here", Result.Rows[0].Cells[1].Content);
            Assert.Equal("1", Result.Rows[1].Cells[1].Content);
        }
    }
}