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
            StringBuilder Builder = new StringBuilder();
            if (file is ITable FileTable)
            {
                Builder.Append(CreateFromTable(FileTable));
            }
            else
            {
                Builder.Append(CreateFromFile(file));
            }
            var ByteData = Encoding.UTF8.GetBytes(Builder.ToString());
            writer.Write(ByteData, 0, ByteData.Length);
            return true;
        }

        private string CreateFromFile(IGenericFile file)
        {
            return "\"" + file.ToString().Replace("\"", "") + "\"";
        }

        /// <summary>
        /// Creates from table.
        /// </summary>
        /// <param name="fileTable">The file table.</param>
        /// <returns>The file data from a table object</returns>
        private string CreateFromTable(ITable fileTable)
        {
            var Builder = new StringBuilder();
            string Seperator = "";
            if (fileTable.Columns.Count > 0)
            {
                foreach (var HeaderColumn in fileTable.Columns)
                {
                    Builder.Append(Seperator).Append(HeaderColumn);
                    Seperator = ",";
                }
                Builder.AppendLine();
            }
            foreach (var Row in fileTable.Rows)
            {
                Seperator = "";
                foreach (var CurrentCell in Row.Cells)
                {
                    Builder.Append(Seperator).Append("\"" + CurrentCell.Content.Replace("\"", "") + "\"");
                    Seperator = ",";
                }
                Builder.AppendLine();
            }
            return Builder.ToString();
        }
    }
}