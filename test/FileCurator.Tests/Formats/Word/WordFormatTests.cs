using FileCurator.Formats.Data;
using FileCurator.Formats.Word;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.Word
{
    public class WordFormatTests : TestBaseClass<WordFormat>
    {
        public WordFormatTests()
        {
            TestObject = new WordFormat();
        }

        [Fact]
        public void Read()
        {
            var TestObject = new WordFormat();
            using var TestFile = File.OpenRead("../../../TestData/TestDocx.docx");
            var Result = TestObject.Read(TestFile);
            Assert.Equal("This is a test docx", Result.ToString());
        }

        [Fact]
        public void Write()
        {
            var TestObject = new WordFormat();
            using (var ResultFile = File.Open("./Results/TestDocx.docx", FileMode.OpenOrCreate))
            {
                Assert.True(TestObject.Write(ResultFile, new GenericFile("Paragraph 1 text.\nParagraph 2 text.", "My title", "")));
            }
            using (var ResultFile = File.Open("./Results/TestDocx.docx", FileMode.OpenOrCreate))
            {
                var Result = TestObject.Read(ResultFile);
                Assert.Equal("Paragraph 1 text.\nParagraph 2 text.", Result.ToString());
                Assert.Equal("My title", Result.Title);
            }
        }

        [Fact]
        public void WriteTable()
        {
            var TestObject = new WordFormat();
            using (var ResultFile = File.Open("./Results/TestDocx.docx", FileMode.OpenOrCreate))
            {
                var Table = new GenericTable
                {
                    Title = "My title"
                };
                var Row1 = new GenericRow();
                Row1.Cells.Add(new GenericCell("This"));
                Row1.Cells.Add(new GenericCell("is"));
                Row1.Cells.Add(new GenericCell("a"));
                var Row2 = new GenericRow();
                Row2.Cells.Add(new GenericCell("test"));
                Row2.Cells.Add(new GenericCell("doc"));
                Row2.Cells.Add(new GenericCell("with"));
                var Row3 = new GenericRow();
                Row3.Cells.Add(new GenericCell("a"));
                Row3.Cells.Add(new GenericCell("table"));
                Row3.Cells.Add(new GenericCell("inside"));
                Table.Columns.Add("A");
                Table.Columns.Add("B");
                Table.Columns.Add("C");
                Table.Rows.Add(Row1);
                Table.Rows.Add(Row2);
                Table.Rows.Add(Row3);
                Assert.True(TestObject.Write(ResultFile, Table));
            }
            using (var ResultFile = File.Open("./Results/TestDocx.docx", FileMode.OpenOrCreate))
            {
                var Result = TestObject.Read(ResultFile);
                Assert.Equal("A\nB\nC\nThis\nis\na\ntest\ndoc\nwith\na\ntable\ninside", Result.ToString());
                Assert.Equal("My title", Result.Title);
            }
        }
    }
}