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
using FileCurator.Formats.Data.Interfaces;
using System.Collections.Generic;
using System.Text;

namespace FileCurator.Formats.Data.BaseClasses
{
    /// <summary>
    /// Table base class
    /// </summary>
    /// <seealso cref="ITable"/>
    public abstract class TableBaseClass : FileBaseClass<TableBaseClass>, ITable
    {
        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>The headers.</value>
        public IList<string> Columns { get; } = new List<string>();

        /// <summary>
        /// Parsed content
        /// </summary>
        /// <value>The content.</value>
        public override string Content { get => ToString(); set { } }

        /// <summary>
        /// Meta data
        /// </summary>
        /// <value>The meta.</value>
        public override string Meta { get; set; }

        /// <summary>
        /// Gets the data rows.
        /// </summary>
        /// <value>The data rows.</value>
        public IList<IRow> Rows { get; } = new List<IRow>();

        /// <summary>
        /// Parsed title
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get; set; }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            StringBuilder Builder = new StringBuilder();
            Builder.Append(Columns.ToString(x => x, " ") + "\n");
            Builder.Append(Rows.ToString(x => x.ToString(), "\n"));
            return Builder.ToString();
        }
    }
}