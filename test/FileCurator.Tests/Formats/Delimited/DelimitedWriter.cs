using FileCurator.Formats.Delimited;
using FileCurator.Formats.Excel;
using FileCurator.Formats.Txt;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.Delimited
{
    public class DelimitedWriterTests : TestBaseClass<DelimitedWriter>
    {
        public DelimitedWriterTests()
        {
            TestObject = new DelimitedWriter();
        }

        [Fact]
        public void WriteATable()
        {
            var TestObject = new DelimitedWriter();
            var TestReader = new ExcelReader();
            var ResultReader = new DelimitedReader();
            using (var ResultFile = File.Open("./Results/WriteATable.csv", FileMode.OpenOrCreate))
            {
                using (var TestFile = File.OpenRead("../../../TestData/TestXLSX.xlsx"))
                {
                    Assert.True(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
                }
            }
            using (var ResultFile = File.Open("./Results/WriteATable.csv", FileMode.OpenOrCreate))
            {
                var Result = ResultReader.Read(ResultFile);
                Assert.Equal(2, Result.Rows.Count);
                Assert.Equal(2, Result.Columns.Count);
                Assert.Equal("Test", Result.Columns[0]);
                Assert.Equal("Data", Result.Columns[1]);
                Assert.Equal("Goes", Result.Rows[0].Cells[0].Content);
                Assert.Equal("here", Result.Rows[0].Cells[1].Content);
                Assert.Equal("1", Result.Rows[1].Cells[1].Content);
            }
        }

        [Fact]
        public void WriteNotATable()
        {
            var TestObject = new DelimitedWriter();
            var TestReader = new TxtFormat();
            var ResultReader = new DelimitedReader();
            using (var ResultFile = File.Open("./Results/WriteNotATable.csv", FileMode.OpenOrCreate))
            {
                using (var TestFile = File.OpenRead("../../../TestData/TestTXT.txt"))
                {
                    Assert.True(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
                }
            }
            using (var ResultFile = File.Open("./Results/WriteNotATable.csv", FileMode.OpenOrCreate))
            {
                var Result = ResultReader.Read(ResultFile);
                Assert.Equal(1, Result.Columns.Count);
                Assert.Equal("This is a test docx", Result.Columns[0]);
            }
        }
    }
}