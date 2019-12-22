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

using BigBook;
using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileCurator.Formats.Delimited
{
    /// <summary>
    /// Delimited file reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{ITable}"/>
    public class DelimitedReader : ReaderBaseClass<ITable>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier { get; } = Array.Empty<byte>();

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override ITable Read(Stream stream)
        {
            var FileContent = stream.ReadAll();
            var ReturnValue = new GenericTable();
            var Delimiter = "";
            if (string.IsNullOrEmpty(FileContent))
                return ReturnValue;
            var TempSplitter = new Regex("[^\"\r\n]*(\r\n|\n|$)|(([^\"\r\n]*)(\"[^\"]*\")([^\"\r\n]*))*(\r\n|\n|$)");
            var Matches = TempSplitter.Matches(FileContent);
            if (string.IsNullOrEmpty(Delimiter) && Matches != null)
                Delimiter = CheckDelimiters((Matches.Where(x => !string.IsNullOrEmpty(x.Value)).FirstOrDefault()?.Value) ?? ",");
            foreach (var TempRowData in Matches.Where(x => !string.IsNullOrEmpty(x.Value)))
            {
                ReturnValue.Rows.Add(ReadRow(TempRowData.Value, Delimiter));
            }
            if (ReturnValue.Rows.Count > 0)
            {
                SetupColumnHeaders(ReturnValue);
            }
            return ReturnValue;
        }

        /// <summary>
        /// Checks the delimiters.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>The delimiter in the file</returns>
        private static string CheckDelimiters(string content)
        {
            if (string.IsNullOrEmpty(content))
                return ",";
            string[] Delimiters = { ",", "|", "\t", "$", ";", ":" };
            var Count = new int[6];
            var MaxIndex = 0;
            for (var x = 0; x < Delimiters.Length; ++x)
            {
                var TempDelimiter = Delimiters[x];
                var TempSplitter = new Regex(string.Format(CultureInfo.InvariantCulture, "(?<Value>\"(?:[^\"]|\"\")*\"|[^{0}\r\n]*?)(?<Delimiter>{0}|\r\n|\n|$)", Regex.Escape(TempDelimiter)));
                Count[x] = TempSplitter.Matches(content).Count;
                if (Count[MaxIndex] < Count[x])
                    MaxIndex = x;
            }
            return Count[MaxIndex] > 1 ? Delimiters[MaxIndex] : ",";
        }

        /// <summary>
        /// Reads the row.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">value or delimiter</exception>
        private IRow ReadRow(string value, string delimiter)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(nameof(value));
            if (string.IsNullOrEmpty(delimiter))
                throw new ArgumentNullException(nameof(delimiter));
            var ReturnValue = new GenericRow();
            var TempSplitter = new Regex(string.Format(CultureInfo.InvariantCulture, "(?<Value>\"(?:[^\"]|\"\")*\"|[^{0}\r\n]*?)(?<Delimiter>{0}|\r\n|\n|$)", Regex.Escape(delimiter)));
            var Matches = TempSplitter.Matches(value);
            var Finished = false;
            foreach (Match TempMatch in Matches)
            {
                if (!Finished)
                {
                    ReturnValue.Cells.Add(new GenericCell(TempMatch.Groups["Value"].Value.Replace("\"", "")));
                }
                Finished = string.IsNullOrEmpty(TempMatch.Groups["Delimiter"].Value) || TempMatch.Groups["Delimiter"].Value == "\r\n" || TempMatch.Groups["Delimiter"].Value == "\n";
            }
            return ReturnValue;
        }

        /// <summary>
        /// Setups the column headers.
        /// </summary>
        /// <param name="returnValue">The return value.</param>
        private void SetupColumnHeaders(GenericTable returnValue)
        {
            var FirstRow = returnValue.Rows[0];
            returnValue.Rows.Remove(FirstRow);
            foreach (var Cell in FirstRow.Cells)
            {
                returnValue.Columns.Add(Cell.Content);
            }
        }
    }
}