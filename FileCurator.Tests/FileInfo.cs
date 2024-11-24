using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Tests.BaseClasses;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace FileCurator.Tests
{
    public class FileInfoTests : TestBaseClass<FileInfo>
    {
        public FileInfoTests()
        {
            TestObject = null;
        }

        [Fact]
        public void Creation()
        {
            var File = new FileInfo("./Test.txt");
            Assert.NotNull(File);
            Assert.False(File.Exists);
        }

        [Fact]
        public void DeleteExtension()
        {
            var Temp = new DirectoryInfo("./Test");
            Temp.Create();
            for (int x = 0; x < 10; ++x)
            {
                new FileInfo("./Test/" + x + ".txt").Write("Testing this out");
            }
            Temp.EnumerateFiles().Delete();
            Temp.Delete();
        }

        [Fact]
        public async Task DeleteExtensionAsync()
        {
            var Temp = new DirectoryInfo("./Test");
            await Temp.CreateAsync().ConfigureAwait(false);
            for (int x = 0; x < 10; ++x)
            {
                await new FileInfo("./Test/" + x + ".txt").WriteAsync("Testing this out").ConfigureAwait(false);
            }
            Temp.EnumerateFiles().Delete();
            await Temp.DeleteAsync().ConfigureAwait(false);
        }

        [Fact]
        public void Parse()
        {
            ITable Result = null;
            try
            {
                Result = new FileInfo("./TestData/TestCSV.csv").Parse<ITable>();
            }
            catch { return; }
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

        [Fact]
        public void ParseAsGenericFile()
        {
            var Result = new FileInfo("./TestData/TestXLSX.xlsx").Parse();
            if (Result.Content.StartsWith("PK"))
                return;
            Assert.NotNull(Result);
            Assert.Equal("Test Data\nGoes here\n 1", Result.Content);
        }

        [Fact]
        public async Task ParseAsGenericFileAsync()
        {
            var Result = await new FileInfo("./TestData/TestXLSX.xlsx").ParseAsync().ConfigureAwait(false);
            if (Result.Content.StartsWith("PK"))
                return;
            Assert.NotNull(Result);
            Assert.Equal("Test Data\nGoes here\n 1", Result.Content);
        }

        [Fact]
        public async Task ParseAsync()
        {
            ITable Result = null;
            try
            {
                Result = await new FileInfo("./TestData/TestCSV.csv").ParseAsync<ITable>().ConfigureAwait(false);
            }
            catch { return; }
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

        [Fact]
        public void ParseXLSX()
        {
            ITable Result = null;
            try
            {
                Result = new FileInfo("./TestData/TestXLSX.xlsx").Parse<ITable>();
            }
            catch { return; }
            Assert.Equal(2, Result.Rows.Count);
            Assert.Equal(2, Result.Columns.Count);
            Assert.Equal("Test", Result.Columns[0]);
            Assert.Equal("Data", Result.Columns[1]);
            Assert.Equal("Goes", Result.Rows[0].Cells[0].Content);
            Assert.Equal("here", Result.Rows[0].Cells[1].Content);
            Assert.Equal("1", Result.Rows[1].Cells[1].Content);
        }

        [Fact]
        public async Task ParseXLSXAsync()
        {
            ITable Result = null;
            try
            {
                Result = await new FileInfo("./TestData/TestXLSX.xlsx").ParseAsync<ITable>().ConfigureAwait(false);
            }
            catch { return; }
            Assert.Equal(2, Result.Rows.Count);
            Assert.Equal(2, Result.Columns.Count);
            Assert.Equal("Test", Result.Columns[0]);
            Assert.Equal("Data", Result.Columns[1]);
            Assert.Equal("Goes", Result.Rows[0].Cells[0].Content);
            Assert.Equal("here", Result.Rows[0].Cells[1].Content);
            Assert.Equal("1", Result.Rows[1].Cells[1].Content);
        }

        [Fact]
        public void ReadWrite()
        {
            var File = new FileInfo("./Test2.txt");
            File.Write("Testing this out");
            Assert.True(File.Exists);
            Assert.Equal("Testing this out", File.Read());
            Assert.Equal("Testing this out", File);
            Assert.Equal(Encoding.ASCII.GetBytes("Testing this out"), File.ReadBinary());
            Assert.Equal(Encoding.ASCII.GetBytes("Testing this out"), File);
            File.Delete();
        }

        [Fact]
        public async Task ReadWriteAsync()
        {
            var File = new FileInfo("./Test2.txt");
            await File.WriteAsync("Testing this out").ConfigureAwait(false);
            Assert.True(File.Exists);
            Assert.Equal("Testing this out", await File.ReadAsync().ConfigureAwait(false));
            Assert.Equal("Testing this out", File);
            Assert.Equal(Encoding.ASCII.GetBytes("Testing this out"), await File.ReadBinaryAsync().ConfigureAwait(false));
            Assert.Equal(Encoding.ASCII.GetBytes("Testing this out"), File);
            await File.DeleteAsync().ConfigureAwait(false);
        }

        [Fact]
        public void WriteGenericFile()
        {
            var TempFile = new FileInfo("./Results/ExtensionMethodTestDocx.docx");
            Assert.True(TempFile.Write(new GenericFile("Paragraph 1 text.\nParagraph 2 text.", "My title", "")));
            var Result = TempFile.Parse();
            Assert.Equal("Paragraph 1 text.\nParagraph 2 text.", Result.ToString());
            if (string.IsNullOrEmpty(Result.Title))
                return;
            Assert.Equal("My title", Result.Title);
        }

        [Fact]
        public async Task WriteGenericFileAsync()
        {
            var TempFile = new FileInfo("./Results/ExtensionMethodTestDocx.docx");
            Assert.True(await TempFile.WriteAsync(new GenericFile("Paragraph 1 text.\nParagraph 2 text.", "My title", "")).ConfigureAwait(false));
            var Result = await TempFile.ParseAsync().ConfigureAwait(false);
            Assert.Equal("Paragraph 1 text.\nParagraph 2 text.", Result.ToString());
            if (string.IsNullOrEmpty(Result.Title))
                return;
            Assert.Equal("My title", Result.Title);
        }
    }
}