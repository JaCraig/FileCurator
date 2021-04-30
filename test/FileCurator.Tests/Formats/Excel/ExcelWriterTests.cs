using FileCurator.Formats.Data;
using FileCurator.Formats.Excel;
using FileCurator.Formats.Txt;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.Excel
{
    public class ExcelWriterTests : TestBaseClass<ExcelWriter>
    {
        public ExcelWriterTests()
        {
            TestObject = new ExcelWriter();
        }

        [Fact]
        public void GetColumnName()
        {
            Assert.Equal("AA", ExcelWriter.Column(27));
            Assert.Equal("IG", ExcelWriter.Column(241));
            Assert.Equal("IH", ExcelWriter.Column(242));
            Assert.Equal("HG", ExcelWriter.Column(215));
            Assert.Equal("JG", ExcelWriter.Column(267));
            Assert.Equal("JH", ExcelWriter.Column(268));
            Assert.Equal("JI", ExcelWriter.Column(269));
            Assert.Equal("KG", ExcelWriter.Column(293));
            Assert.Equal("KH", ExcelWriter.Column(294));
            Assert.Equal("KI", ExcelWriter.Column(295));
            Assert.Equal("KJ", ExcelWriter.Column(296));
        }

        [Fact]
        public void WriteAGenericTable()
        {
            var TestObject = new ExcelWriter();
            var TestReader = new ExcelReader();
            var TestTable = new GenericTable();
            TestTable.Columns.Add("User Name");
            TestTable.Columns.Add("Start Date");
            TestTable.Columns.Add("End Date");
            TestTable.Columns.Add("Employee Number");
            TestTable.Columns.Add("Orientation Date");
            TestTable.Columns.Add("First Name");
            TestTable.Columns.Add("Last Name");
            TestTable.Columns.Add("Title");
            TestTable.Columns.Add("Middle Name");
            TestTable.Columns.Add("Nick Name");
            TestTable.Columns.Add("Prefix");
            TestTable.Columns.Add("Suffix");
            TestTable.Columns.Add("Active");
            TestTable.Columns.Add("Date Created");
            TestTable.Columns.Add("Date Modified");
            TestTable.Columns.Add("ID");
            var TempRow = new GenericRow();
            TempRow.Cells.Add(new GenericCell("craig"));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell("craig craig"));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell("True"));
            TempRow.Cells.Add(new GenericCell("5/2/2018 4:00:04 PM"));
            TempRow.Cells.Add(new GenericCell("5/2/2018 4:00:04 PM"));
            TempRow.Cells.Add(new GenericCell("1"));
            TestTable.Rows.Add(TempRow);
            TempRow = new GenericRow();
            TempRow.Cells.Add(new GenericCell("rowland"));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell("bee"));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell("True"));
            TempRow.Cells.Add(new GenericCell("5/2/2018 4:00:04 PM"));
            TempRow.Cells.Add(new GenericCell("5/2/2018 4:00:04 PM"));
            TempRow.Cells.Add(new GenericCell("2"));
            TestTable.Rows.Add(TempRow);
            TempRow = new GenericRow();
            TempRow.Cells.Add(new GenericCell("system"));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell("1/1/0001 12:00:00 AM"));
            TempRow.Cells.Add(new GenericCell("account"));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell(""));
            TempRow.Cells.Add(new GenericCell("True"));
            TempRow.Cells.Add(new GenericCell("5/2/2018 4:00:04 PM"));
            TempRow.Cells.Add(new GenericCell("5/2/2018 4:00:04 PM"));
            TempRow.Cells.Add(new GenericCell("3"));
            TestTable.Rows.Add(TempRow);
            using (var ResultFile = File.Open("./Results/WriteAGenericTable.xlsx", FileMode.OpenOrCreate))
            {
                Assert.True(TestObject.Write(ResultFile, TestTable));
            }

            using (var ResultFile = File.Open("./Results/WriteAGenericTable.xlsx", FileMode.OpenOrCreate))
            {
                var Result = TestReader.Read(ResultFile);
                Assert.Equal(Result, TestTable);
            }
        }

        [Fact]
        public void WriteATable()
        {
            var TestObject = new ExcelWriter();
            var TestReader = new ExcelReader();
            using (var ResultFile = File.Open("./Results/WriteATable.xlsx", FileMode.OpenOrCreate))
            {
                using (var TestFile = File.OpenRead("../../../TestData/TestXLSX.xlsx"))
                {
                    Assert.True(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
                }
            }
            using (var ResultFile = File.Open("./Results/WriteATable.xlsx", FileMode.OpenOrCreate))
            {
                var Result = TestReader.Read(ResultFile);
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
            var TestObject = new ExcelWriter();
            var TestReader = new TxtFormat();
            var ResultReader = new ExcelReader();
            using (var ResultFile = File.Open("./Results/WriteNotATable.xlsx", FileMode.OpenOrCreate))
            {
                using (var TestFile = File.OpenRead("../../../TestData/TestTXT.txt"))
                {
                    Assert.True(TestObject.Write(ResultFile, TestReader.Read(TestFile)));
                }
            }
            using (var ResultFile = File.Open("./Results/WriteNotATable.xlsx", FileMode.OpenOrCreate))
            {
                var Result = ResultReader.Read(ResultFile);
                Assert.Equal(1, Result.Columns.Count);
                Assert.Equal("This is a test docx", Result.Columns[0]);
            }
        }
    }
}