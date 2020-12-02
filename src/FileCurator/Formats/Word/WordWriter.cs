/*
Copyright 2017 James Craig

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using System.IO;
using System.Threading.Tasks;

namespace FileCurator.Formats.Word
{
    /// <summary>
    /// Word writer
    /// </summary>
    /// <seealso cref="IGenericFileWriter"/>
    public class WordWriter : IGenericFileWriter
    {
        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public bool Write(Stream writer, IGenericFile file)
        {
            using (var Doc = WordprocessingDocument.Create(writer, DocumentFormat.OpenXml.WordprocessingDocumentType.Document))
            {
                Doc.PackageProperties.Title = file.Title;
                Doc.AddMainDocumentPart();
                Doc.MainDocumentPart.Document = new Document
                {
                    Body = new Body()
                };
                if (file is ITable TableFile)
                    AppendTable(Doc, TableFile);
                else
                    AppendFile(Doc, file);
            }
            return true;
        }

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public Task<bool> WriteAsync(Stream writer, IGenericFile file)
        {
            return Task.FromResult(Write(writer, file));
        }

        private void AppendFile(WordprocessingDocument doc, IGenericFile file)
        {
            foreach (var ParagraphText in file.ToString().Split('\n'))
            {
                doc.MainDocumentPart.Document.Body.Append(new Paragraph(
                                    new Run(
                                    new Text(ParagraphText))));
            }
        }

        private void AppendTable(WordprocessingDocument doc, ITable table)
        {
            var TempTable = new Table();
            TempTable.Append(new TableWidth
            {
                Width = "5000",
                Type = TableWidthUnitValues.Pct
            });
            if (table.Columns.Count > 0)
            {
                var TempHeaderRow = new TableRow();
                var ColumnWidth = 100f / table.Columns.Count;
                foreach (var CurrentColumn in table.Columns)
                {
                    var TempCell = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellWidth = new TableCellWidth
                            {
                                Width = ColumnWidth + "%",
                                Type = TableWidthUnitValues.Pct
                            },
                            TableCellBorders = new TableCellBorders
                            {
                                BottomBorder = new BottomBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Thick),
                                    Color = "000000"
                                }
                            }
                        }
                    };
                    TempCell.Append(new Paragraph(new Run(new Text(CurrentColumn))));
                    TempHeaderRow.Append(TempCell);
                }
                TempTable.Append(TempHeaderRow);
            }
            foreach (var Row in table.Rows)
            {
                var TempRow = new TableRow();
                var ColumnWidth = 100f / Row.Cells.Count;
                foreach (var Cell in Row.Cells)
                {
                    var TempCell = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellWidth = new TableCellWidth
                            {
                                Width = ColumnWidth + "%",
                                Type = TableWidthUnitValues.Pct
                            },
                            TableCellBorders = new TableCellBorders
                            {
                                TopBorder = new TopBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Thick),
                                    Color = "000000"
                                }
                            }
                        }
                    };
                    TempCell.Append(new Paragraph(new Run(new Text(Cell.Content))));
                    TempRow.Append(TempCell);
                }
                TempTable.Append(TempRow);
            }
            doc.MainDocumentPart.Document.Body.Append(TempTable);
        }
    }
}