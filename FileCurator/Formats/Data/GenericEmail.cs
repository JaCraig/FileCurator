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

using FileCurator.Formats.Data.BaseClasses;
using FileCurator.Formats.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace FileCurator.Formats.Data
{
    /// <summary>
    /// Generic email message
    /// </summary>
    /// <seealso cref="IMessage"/>
    public class GenericEmail : FileBaseClass<GenericEmail>, IMessage
    {
        /// <summary>
        /// Gets the BCC.
        /// </summary>
        /// <value>The BCC.</value>
        public IList<string> BCC { get; } = new List<string>();

        /// <summary>
        /// Gets the cc.
        /// </summary>
        /// <value>The cc.</value>
        public IList<string> CC { get; } = new List<string>();

        /// <summary>
        /// Parsed content
        /// </summary>
        /// <value>The content.</value>
        public override string Content { get; set; }

        /// <summary>
        /// Gets or sets from.
        /// </summary>
        /// <value>From.</value>
        public string From { get; set; }

        /// <summary>
        /// Meta data
        /// </summary>
        /// <value>The meta.</value>
        public override string Meta { get; set; }

        /// <summary>
        /// Gets or sets the sent.
        /// </summary>
        /// <value>The sent.</value>
        public DateTime Sent { get; set; }

        /// <summary>
        /// Parsed title
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get; set; }

        /// <summary>
        /// Gets to.
        /// </summary>
        /// <value>To.</value>
        public IList<string> To { get; } = new List<string>();

        /// <summary>
        /// Compares the object to another object
        /// </summary>
        /// <param name="other">Object to compare to</param>
        /// <returns>0 if they are equal, -1 if this is smaller, 1 if it is larger</returns>
        public override int CompareTo(GenericEmail other) => string.Compare(other.ToString(), ToString(), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="other">Other object to compare to</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public override bool Equals(GenericEmail other) => ToString().Equals(other.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}