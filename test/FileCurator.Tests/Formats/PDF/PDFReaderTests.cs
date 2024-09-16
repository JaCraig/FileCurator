using FileCurator.Formats.PDF;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.PDF
{
    public class PDFReaderTests : TestBaseClass<PDFReader>
    {
        public PDFReaderTests()
        {
            TestObject = new PDFReader();
        }

        [Fact]
        public void Read()
        {
            var TestObject = new PDFReader();
            FileCurator.Formats.Data.Interfaces.IGenericFile Result = TestObject.Read(File.OpenRead("./TestData/TestPDF.pdf"));
            Assert.Equal("This is a test docx", Result.Content.Trim());
            Assert.Equal("Title of doc", Result.Title);
            Assert.Equal("tag 1", Result.Meta);
        }
    }
}