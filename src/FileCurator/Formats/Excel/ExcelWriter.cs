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
using DocumentFormat.OpenXml.Spreadsheet;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using System.IO;
using System.Linq;

namespace FileCurator.Formats.Excel
{
    /// <summary>
    /// Excel writer
    /// </summary>
    /// <seealso cref="IGenericFileWriter"/>
    public class ExcelWriter : IGenericFileWriter
    {
        /// <summary>
        /// Gets the specified column name.
        /// </summary>
        /// <param name="column">The column.</param>
        /// <returns>The column name</returns>
        public static string Column(int column)
        {
            var ColumnLetter = "";
            var Mod = 0;

            while (column > 0)
            {
                Mod = (column - 1) % 26;
                ColumnLetter = (char)(65 + Mod) + ColumnLetter;
                column = (column - Mod) / 26;
            }
            return ColumnLetter;
        }

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public bool Write(Stream writer, IGenericFile file)
        {
            var TableFile = file as ITable;
            using (var Document = SpreadsheetDocument.Create(writer, SpreadsheetDocumentType.Workbook))
            {
                Document.AddWorkbookPart();
                Document.WorkbookPart.Workbook = new Workbook();
                Document.WorkbookPart.Workbook.AppendChild(new Sheets());
                var WorksheetPart = InsertSheetInWorksheet(Document.WorkbookPart);
                var Worksheet = WorksheetPart.Worksheet;
                var SheetData = Worksheet.GetFirstChild<SheetData>();
                if (TableFile == null)
                {
                    var Row = new Row { RowIndex = 1 };
                    Row.AppendChild(new Cell
                    {
                        CellValue = new CellValue(file.ToString()),
                        DataType = new EnumValue<CellValues>(CellValues.String),
                        CellReference = "A1"
                    });
                    SheetData.AppendChild(Row);
                }
                else
                {
                    var headerOffset = 1;
                    if (TableFile.Columns.Count > 0)
                    {
                        headerOffset = 2;
                        var Row = new Row { RowIndex = 1 };
                        for (var x = 0; x < TableFile.Columns.Count; ++x)
                        {
                            Row.AppendChild(new Cell
                            {
                                CellValue = new CellValue(TableFile.Columns[x]),
                                DataType = new EnumValue<CellValues>(CellValues.String),
                                CellReference = Column(x + 1) + 1
                            });
                        }
                        SheetData.AppendChild(Row);
                    }
                    for (var x = 0; x < TableFile.Rows.Count; ++x)
                    {
                        var Row = new Row { RowIndex = (uint)(x + 2) };
                        SheetData.AppendChild(Row);
                        for (var y = 0; y < TableFile.Rows[x].Cells.Count; ++y)
                        {
                            Row.AppendChild(new Cell
                            {
                                CellValue = new CellValue(TableFile.Rows[x].Cells[y].Content),
                                DataType = new EnumValue<CellValues>(CellValues.String),
                                CellReference = Column(y + 1) + (x + headerOffset)
                            });
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Inserts the sheet in worksheet.
        /// </summary>
        /// <param name="workbookPart">The workbook part.</param>
        /// <returns>The worksheet</returns>
        private WorksheetPart InsertSheetInWorksheet(WorkbookPart workbookPart)
        {
            // Add a new worksheet part to the workbook.
            var newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            var sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            var relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Any())
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            var sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            var sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }
    }
}