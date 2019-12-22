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

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileCurator.Formats.Excel
{
    /// <summary>
    /// Excel reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{ITable}"/>
    public class ExcelReader : ReaderBaseClass<ITable>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier { get; } = new byte[] { 0x50, 0x4B, 0x03, 0x04 };

        /// <summary>
        /// Used to determine if a reader can actually read the file
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>True if it can, false otherwise</returns>
        public override bool InternalCanRead(Stream stream)
        {
            try
            {
                SpreadsheetDocument.Open(stream, false);
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Reads the excel.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The excel data</returns>
        public override ITable Read(Stream stream)
        {
            var data = new GenericTable();

            // Open the excel document
            WorkbookPart workbookPart; List<Row> rows;
            try
            {
                var document = SpreadsheetDocument.Open(stream, false);
                workbookPart = document.WorkbookPart;

                var sheets = workbookPart.Workbook.Descendants<Sheet>();
                var sheet = sheets.First();
                data.Title = sheet.Name;

                var workSheet = ((WorksheetPart)workbookPart
                    .GetPartById(sheet.Id)).Worksheet;
                var columns = workSheet.Descendants<Columns>().FirstOrDefault();

                var sheetData = workSheet.Elements<SheetData>().First();
                rows = sheetData.Elements<Row>().ToList();
            }
            catch (Exception)
            {
                return data;
            }

            // Read the header
            if (rows.Count > 0)
            {
                var row = rows[0];
                var cellEnumerator = GetExcelCellEnumerator(row);
                while (cellEnumerator.MoveNext())
                {
                    var cell = cellEnumerator.Current;
                    var text = ReadExcelCell(cell, workbookPart).Trim();
                    data.Columns.Add(text);
                }
            }

            // Read the sheet data
            if (rows.Count > 1)
            {
                for (var i = 1; i < rows.Count; i++)
                {
                    var dataRow = new GenericRow();
                    data.Rows.Add(dataRow);
                    var row = rows[i];
                    var cellEnumerator = GetExcelCellEnumerator(row);
                    while (cellEnumerator.MoveNext())
                    {
                        var cell = cellEnumerator.Current;
                        var text = ReadExcelCell(cell, workbookPart).Trim();
                        dataRow.Cells.Add(new GenericCell(text));
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Converts the column name to number.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The column name to number</returns>
        /// <exception cref="ArgumentException">columnName</exception>
        private int ConvertColumnNameToNumber(string columnName)
        {
            var Alpha = new Regex("^[A-Z]+$");
            if (!Alpha.IsMatch(columnName)) throw new ArgumentException(nameof(columnName));

            var ColLetters = columnName.ToCharArray();
            Array.Reverse(ColLetters);

            var ConvertedValue = 0;
            for (var i = 0; i < ColLetters.Length; i++)
            {
                var Letter = ColLetters[i];
                // ASCII 'A' = 65
                var Current = i == 0 ? Letter - 65 : Letter - 64;
                ConvertedValue += Current * (int)Math.Pow(26, i);
            }

            return ConvertedValue;
        }

        /// <summary>
        /// Gets the name of the column.
        /// </summary>
        /// <param name="cellReference">The cell reference.</param>
        /// <returns>The column name.</returns>
        private string GetColumnName(string cellReference)
        {
            if (string.IsNullOrEmpty(cellReference))
                return "";
            return new Regex("[A-Za-z]+").Match(cellReference)
                                         .Value;
        }

        /// <summary>
        /// Gets the excel cell enumerator.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns>The individual cell.</returns>
        private IEnumerator<Cell> GetExcelCellEnumerator(Row row)
        {
            var CurrentCount = 0;
            foreach (var CurrentCell in row.Descendants<Cell>())
            {
                var ColumnName = GetColumnName(CurrentCell.CellReference);

                var CurrentColumnIndex = ConvertColumnNameToNumber(ColumnName);

                for (; CurrentCount < CurrentColumnIndex; ++CurrentCount)
                {
                    var EmptyCell = new Cell
                    {
                        DataType = null,
                        CellValue = new CellValue(string.Empty)
                    };
                    yield return EmptyCell;
                }
                yield return CurrentCell;
                ++CurrentCount;
            }
        }

        /// <summary>
        /// Reads the excel cell.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="workbookPart">The workbook part.</param>
        /// <returns>The excel cell</returns>
        private string ReadExcelCell(Cell cell, WorkbookPart workbookPart)
        {
            var CellValue = cell.CellValue;
            var Text = (CellValue == null) ? cell.InnerText : CellValue.Text;
            if ((cell.DataType != null) && (cell.DataType == CellValues.SharedString))
            {
                Text = workbookPart.SharedStringTablePart
                                   .SharedStringTable
                                   .Elements<SharedStringItem>()
                                   .ElementAt(Convert.ToInt32(cell.CellValue.Text))
                                   .InnerText;
            }

            return (Text ?? string.Empty).Trim();
        }
    }
}