using BigBook;
using FileCurator.Formats.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace FileCurator.Example
{
    /// <summary>
    /// Example program to show how to use the FileCurator library
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Defines the entry point of the application.
        /// </summary>
        /// <param name="args">The arguments.</param>
        private static void Main(string[] args)
        {
            // Create a service provider.
            var ServiceProvider = new ServiceCollection().AddCanisterModules()?.BuildServiceProvider();

            // Let's load a generic file and view it's details.
            var MyFile = new FileInfo("./MyFile.txt");
            Console.WriteLine("File Info:");
            Console.WriteLine($"File: {MyFile.FullName}");
            Console.WriteLine($"File Exists: {MyFile.Exists}");
            Console.WriteLine($"File Extension: {MyFile.Extension}");
            Console.WriteLine($"File Name: {MyFile.Name}");
            Console.WriteLine($"File length: {MyFile.Length}");
            Console.WriteLine();

            // We can also parse the file and view it's contents.
            var PDFContent = new FileInfo("./TestPDF.pdf").Parse<IGenericFile>();
            Console.WriteLine("PDF Content:");
            Console.WriteLine(PDFContent.Content);
            Console.WriteLine();

            // Including more structured files like CSVs. This will parse the CSV into a table.
            var CSVContent = new FileInfo("./TestCSV.csv").Parse<ITable>();
            Console.WriteLine("CSV Content:");
            Console.WriteLine(CSVContent.Content);
            Console.WriteLine($"Columns: {CSVContent.Columns.ToString(x => x)}");
            Console.WriteLine($"Rows: {CSVContent.Rows.ToString(x => x.Cells.ToString(x => x.Content, ", "), "\n")}");
            Console.WriteLine();

            // We can also write structured content to a file.
            var NewCSVFile = new FileInfo("./NewCSV.csv");
            NewCSVFile.Write(CSVContent);

            // And then read it as a string.
            Console.WriteLine(NewCSVFile.Read());
        }
    }
}