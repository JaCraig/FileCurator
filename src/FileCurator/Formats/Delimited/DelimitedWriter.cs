﻿/*
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

using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileCurator.Formats.Delimited
{
    /// <summary>
    /// Delimited file writer
    /// </summary>
    /// <seealso cref="IGenericFileWriter"/>
    public class DelimitedWriter : IGenericFileWriter
    {
        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public bool Write(Stream writer, IGenericFile file)
        {
            if (writer is null || file is null)
                return false;
            var Builder = new StringBuilder();
            if (file is ITable FileTable)
            {
                Builder.Append(CreateFromTable(FileTable));
            }
            else
            {
                Builder.Append(CreateFromFile(file));
            }
            var ByteData = Encoding.UTF8.GetBytes(Builder.ToString());
            try
            {
                writer.Write(ByteData, 0, ByteData.Length);
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public async Task<bool> WriteAsync(Stream writer, IGenericFile file)
        {
            if (writer is null || file is null)
                return false;
            var Builder = new StringBuilder();
            if (file is ITable FileTable)
            {
                Builder.Append(CreateFromTable(FileTable));
            }
            else
            {
                Builder.Append(CreateFromFile(file));
            }
            var ByteData = Encoding.UTF8.GetBytes(Builder.ToString());
            try
            {
                await writer.WriteAsync(ByteData, 0, ByteData.Length).ConfigureAwait(false);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private string CreateFromFile(IGenericFile file) => "\"" + file?.ToString().Replace("\"", "") + "\"";

        /// <summary>
        /// Creates from table.
        /// </summary>
        /// <param name="fileTable">The file table.</param>
        /// <returns>The file data from a table object</returns>
        private string CreateFromTable(ITable fileTable)
        {
            var Builder = new StringBuilder();
            var Seperator = "";
            if (fileTable.Columns.Count > 0)
            {
                foreach (var HeaderColumn in fileTable.Columns)
                {
                    Builder.Append(Seperator).Append("\"").Append(HeaderColumn?.Replace("\"", "") ?? "").Append("\"");
                    Seperator = ",";
                }
                Builder.AppendLine();
            }
            foreach (var Row in fileTable.Rows)
            {
                Seperator = "";
                foreach (var CurrentCell in Row.Cells)
                {
                    Builder.Append(Seperator).Append("\"").Append(CurrentCell.Content?.Replace("\"", "") ?? "").Append("\"");
                    Seperator = ",";
                }
                Builder.AppendLine();
            }
            return Builder.ToString();
        }
    }
}