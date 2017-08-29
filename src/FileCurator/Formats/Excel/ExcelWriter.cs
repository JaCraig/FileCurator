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
using System;
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
            return column < 26 ?
                            ((char)('A' + column)).ToString() :
                            Column(column / 26) + Column(column % 26 + 1);
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
                WorksheetPart WorksheetPart = InsertSheetInWorksheet(Document.WorkbookPart);
                if (TableFile == null)
                {
                    Cell TempCell = InsertCellInWorksheet("A", 1, WorksheetPart);
                    TempCell.CellValue = new CellValue(file.ToString());
                    TempCell.DataType = new EnumValue<CellValues>(CellValues.String);
                }
                else
                {
                    for (int x = 0; x < TableFile.Columns.Count; ++x)
                    {
                        Cell TempCell = InsertCellInWorksheet(Column(x), 0, WorksheetPart);
                        TempCell.CellValue = new CellValue(TableFile.Columns[x]);
                        TempCell.DataType = new EnumValue<CellValues>(CellValues.String);
                    }
                    for (uint x = 0; x < TableFile.Rows.Count; ++x)
                    {
                        for (int y = 0; y < TableFile.Rows[(int)x].Cells.Count; ++y)
                        {
                            Cell TempCell = InsertCellInWorksheet(Column(y), x + 1, WorksheetPart);
                            TempCell.CellValue = new CellValue(TableFile.Rows[(int)x].Cells[y].Content);
                            TempCell.DataType = new EnumValue<CellValues>(CellValues.String);
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Inserts the cell in worksheet.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <param name="worksheetPart">The worksheet part.</param>
        /// <returns>The cell.</returns>
        private static Cell InsertCellInWorksheet(string columnName, uint rowIndex, WorksheetPart worksheetPart)
        {
            Worksheet worksheet = worksheetPart.Worksheet;
            SheetData sheetData = worksheet.GetFirstChild<SheetData>();
            string cellReference = columnName + rowIndex;

            // If the worksheet does not contain a row with the specified row index, insert one.
            Row row;
            if (sheetData.Elements<Row>().Any(r => r.RowIndex == rowIndex))
            {
                row = sheetData.Elements<Row>().First(r => r.RowIndex == rowIndex);
            }
            else
            {
                row = new Row() { RowIndex = rowIndex };
                sheetData.Append(row);
            }

            // If there is not a cell with the specified column name, insert one.
            if (row.Elements<Cell>().Any(c => c.CellReference.Value == columnName + rowIndex))
            {
                return row.Elements<Cell>().First(c => c.CellReference.Value == cellReference);
            }
            else
            {
                // Cells must be in sequential order according to CellReference. Determine where to
                // insert the new cell.
                Cell refCell = null;
                foreach (Cell cell in row.Elements<Cell>())
                {
                    if (cell.CellReference.Value.Length == cellReference.Length)
                    {
                        if (string.Compare(cell.CellReference.Value, cellReference, StringComparison.OrdinalIgnoreCase) > 0)
                        {
                            refCell = cell;
                            break;
                        }
                    }
                }

                Cell newCell = new Cell() { CellReference = cellReference };
                row.InsertBefore(newCell, refCell);

                worksheet.Save();
                return newCell;
            }
        }

        /// <summary>
        /// Inserts the sheet in worksheet.
        /// </summary>
        /// <param name="workbookPart">The workbook part.</param>
        /// <returns>The worksheet</returns>
        private WorksheetPart InsertSheetInWorksheet(WorkbookPart workbookPart)
        {
            // Add a new worksheet part to the workbook.
            WorksheetPart newWorksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            newWorksheetPart.Worksheet = new Worksheet(new SheetData());
            newWorksheetPart.Worksheet.Save();

            Sheets sheets = workbookPart.Workbook.GetFirstChild<Sheets>();
            string relationshipId = workbookPart.GetIdOfPart(newWorksheetPart);

            // Get a unique ID for the new sheet.
            uint sheetId = 1;
            if (sheets.Elements<Sheet>().Any())
            {
                sheetId = sheets.Elements<Sheet>().Select(s => s.SheetId.Value).Max() + 1;
            }

            string sheetName = "Sheet" + sheetId;

            // Append the new worksheet and associate it with the workbook.
            Sheet sheet = new Sheet() { Id = relationshipId, SheetId = sheetId, Name = sheetName };
            sheets.Append(sheet);
            workbookPart.Workbook.Save();

            return newWorksheetPart;
        }
    }
}