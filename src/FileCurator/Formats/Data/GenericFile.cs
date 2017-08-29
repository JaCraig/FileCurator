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
using System;

namespace FileCurator.Formats.Data
{
    /// <summary>
    /// Generic file
    /// </summary>
    /// <seealso cref="BaseClasses.FileBaseClass{GenericFile}"/>
    public class GenericFile : FileBaseClass<GenericFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFile"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="title">The title.</param>
        /// <param name="meta">The meta.</param>
        public GenericFile(string content, string title, string meta)
        {
            Content = content ?? "";
            Title = title ?? " ";
            Meta = meta ?? "";
        }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public override string Content { get; set; }

        /// <summary>
        /// Meta data
        /// </summary>
        /// <value>The meta.</value>
        public override string Meta { get; set; }

        /// <summary>
        /// Parsed title
        /// </summary>
        /// <value>The title.</value>
        public override string Title { get; set; }

        /// <summary>
        /// Compares the object to another object
        /// </summary>
        /// <param name="other">Object to compare to</param>
        /// <returns>0 if they are equal, -1 if this is smaller, 1 if it is larger</returns>
        public override int CompareTo(GenericFile other)
        {
            return string.Compare(other.ToString(), ToString(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="other">Other object to compare to</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public override bool Equals(GenericFile other)
        {
            return ToString().Equals(other.ToString(), StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return Content;
        }
    }
}