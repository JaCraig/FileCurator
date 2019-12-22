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
    /// Generic table object
    /// </summary>
    /// <seealso cref="TableBaseClass"/>
    public class GenericTable : TableBaseClass
    {
        /// <summary>
        /// Compares the object to another object
        /// </summary>
        /// <param name="other">Object to compare to</param>
        /// <returns>0 if they are equal, -1 if this is smaller, 1 if it is larger</returns>
        public override int CompareTo(TableBaseClass other) => string.Compare(other.ToString(), ToString(), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="other">Other object to compare to</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public override bool Equals(TableBaseClass other) => ToString().Equals(other.ToString(), StringComparison.OrdinalIgnoreCase);
    }
}