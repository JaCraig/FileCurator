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

namespace FileCurator.Formats.Data
{
    /// <summary>
    /// Generic row data
    /// </summary>
    /// <seealso cref="IRow"/>
    public class GenericRow : IRow
    {
        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>The cells.</value>
        public IList<ICell> Cells { get; } = new List<ICell>();

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString() => Cells.ToString(x => x.ToString(), " ");
    }
}