using FileCurator.Formats.Excel;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.Excel
{
    public class ExcelReaderTests : TestBaseClass<ExcelReader>
    {
        public ExcelReaderTests()
        {
            TestObject = new ExcelReader();
        }

        [Fact]
        public void Convert()
        {
            var TestObject = new ExcelReader();
            var Result = TestObject.Read(File.OpenRead("../../../TestData/TestXLSX.xlsx"));
            var Results = Result.Convert<ExcelTestData>();
            Assert.Equal(2, Results.Count);
            Assert.Equal("Goes", Results[0].Test);
            Assert.Equal("here", Results[0].Data);
            Assert.Equal(string.Empty, Results[1].Test);
            Assert.Equal("1", Results[1].Data);
        }

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
            Assert.Equal(1, Result.Rows[1].Cells[1].GetValue<int>());
        }

        public class ExcelTestData
        {
            public string Data { get; set; }
            public string Test { get; set; }
        }
    }
}