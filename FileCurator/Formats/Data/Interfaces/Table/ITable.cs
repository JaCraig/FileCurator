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

using System.Collections.Generic;

namespace FileCurator.Formats.Data.Interfaces
{
    /// <summary>
    /// Table interface
    /// </summary>
    public interface ITable : IGenericFile
    {
        /// <summary>
        /// Gets the headers.
        /// </summary>
        /// <value>The headers.</value>
        IList<string> Columns { get; }

        /// <summary>
        /// Gets the data rows.
        /// </summary>
        /// <value>The data rows.</value>
        IList<IRow> Rows { get; }

        /// <summary>
        /// Converts this instance into the object array of the type specified.
        /// </summary>
        /// <typeparam name="TObject">The type of the object.</typeparam>
        /// <returns>The resulting array.</returns>
        List<TObject> Convert<TObject>();
    }
}