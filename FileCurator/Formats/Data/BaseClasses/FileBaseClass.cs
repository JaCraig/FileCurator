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

namespace FileCurator.Formats.Data.BaseClasses
{
    /// <summary>
    /// File base class
    /// </summary>
    /// <typeparam name="FormatFileType">The type of the format file type.</typeparam>
    /// <seealso cref="IGenericFile"/>
    public abstract class FileBaseClass<FormatFileType> : IGenericFile
        where FormatFileType : FileBaseClass<FormatFileType>
    {
        /// <summary>
        /// Parsed content
        /// </summary>
        /// <value>The content.</value>
        public abstract string Content { get; set; }

        /// <summary>
        /// Meta data
        /// </summary>
        /// <value>The meta.</value>
        public abstract string Meta { get; set; }

        /// <summary>
        /// Parsed title
        /// </summary>
        /// <value>The title.</value>
        public abstract string Title { get; set; }

        /// <summary>
        /// Determines if the two are not equal
        /// </summary>
        /// <param name="Value1">Value 1</param>
        /// <param name="Value2">Value 2</param>
        /// <returns>True if they are not equal, false otherwise</returns>
        public static bool operator !=(FileBaseClass<FormatFileType> Value1, FileBaseClass<FormatFileType> Value2)
        {
            return !(Value1 == Value2);
        }

        /// <summary>
        /// Determines if it is less than
        /// </summary>
        /// <param name="Value1">Value 1</param>
        /// <param name="Value2">Value 2</param>
        /// <returns>True if it is less than, false otherwise</returns>
        public static bool operator <(FileBaseClass<FormatFileType> Value1, FileBaseClass<FormatFileType> Value2)
        {
            return Value1.CompareTo(Value2) < 0;
        }

        /// <summary>
        /// Determines if it is less than or equal
        /// </summary>
        /// <param name="Value1">Value 1</param>
        /// <param name="Value2">Value 2</param>
        /// <returns>True if it is less than or equal, false otherwise</returns>
        public static bool operator <=(FileBaseClass<FormatFileType> Value1, FileBaseClass<FormatFileType> Value2)
        {
            return Value1.CompareTo(Value2) <= 0;
        }

        /// <summary>
        /// Determines if the two are equal
        /// </summary>
        /// <param name="Value1">Value 1</param>
        /// <param name="Value2">Value 2</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public static bool operator ==(FileBaseClass<FormatFileType> Value1, FileBaseClass<FormatFileType> Value2)
        {
            return Value1.Equals(Value2);
        }

        /// <summary>
        /// Determines if it is greater than
        /// </summary>
        /// <param name="Value1">Value 1</param>
        /// <param name="Value2">Value 2</param>
        /// <returns>True if it is greater than, false otherwise</returns>
        public static bool operator >(FileBaseClass<FormatFileType> Value1, FileBaseClass<FormatFileType> Value2)
        {
            return Value1.CompareTo(Value2) > 0;
        }

        /// <summary>
        /// Determines if it is greater than or equal
        /// </summary>
        /// <param name="Value1">Value 1</param>
        /// <param name="Value2">Value 2</param>
        /// <returns>True if it is greater than or equal, false otherwise</returns>
        public static bool operator >=(FileBaseClass<FormatFileType> Value1, FileBaseClass<FormatFileType> Value2)
        {
            return Value1.CompareTo(Value2) >= 0;
        }

        /// <summary>
        /// Compares the object to another object
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>0 if they are equal, -1 if this is smaller, 1 if it is larger</returns>
        public int CompareTo(object obj) => obj is FormatFileType formatFileType ? CompareTo(formatFileType) : -1;

        /// <summary>
        /// Compares the object to another object
        /// </summary>
        /// <param name="other">Object to compare to</param>
        /// <returns>0 if they are equal, -1 if this is smaller, 1 if it is larger</returns>
        public abstract int CompareTo(FormatFileType other);

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="other">Other object to compare to</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public abstract bool Equals(FormatFileType other);

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="obj">Other object to compare to</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            return (obj is FormatFileType TempItem) && Equals(TempItem);
        }

        /// <summary>
        /// Gets the hash code for the object
        /// </summary>
        /// <returns>The hash code for the object</returns>
        public override int GetHashCode() => ToString().GetHashCode();
    }
}