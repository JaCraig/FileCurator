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

using FileCurator.Formats.Data.Interfaces;
using ObjectCartographer;

namespace FileCurator.Formats.Data
{
    /// <summary>
    /// Generic cell data
    /// </summary>
    /// <seealso cref="ICell"/>
    public class GenericCell : ICell
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCell"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public GenericCell(string content)
        {
            Content = content;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content { get; set; }

        /// <summary>
        /// Gets the value as the type specified.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>The content of the cell as the value type specified.</returns>
        public TValue GetValue<TValue>()
        {
            return Content.To<TValue>();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString() => Content;
    }
}