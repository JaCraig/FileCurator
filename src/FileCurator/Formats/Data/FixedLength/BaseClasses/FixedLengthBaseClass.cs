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

using FileCurator.Data.FixedLength.Interfaces;
using FileCurator.Formats.Data.BaseClasses;
using System.Collections.Generic;
using System.Text;

namespace FileCurator.Data.FixedLength.BaseClasses
{
    /// <summary>
    /// Fixed length base class
    /// </summary>
    /// <seealso cref="Formats.Data.BaseClasses.FileBaseClass{FixedLengthBaseClass}"/>
    public abstract class FixedLengthBaseClass : FileBaseClass<FixedLengthBaseClass>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FixedLengthBaseClass"/> class.
        /// </summary>
        protected FixedLengthBaseClass()
        {
            Records = new List<IRecord>();
        }

        /// <summary>
        /// Gets or sets the records.
        /// </summary>
        /// <value>The records.</value>
        protected IList<IRecord> Records { get; set; }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <param name="Length">The length.</param>
        public abstract void Parse(string Value, int Length = -1);

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            var Builder = new StringBuilder();
            foreach (var Record in Records)
                Builder.Append(Record.ToString());
            return Builder.ToString();
        }
    }
}