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
using System.Collections.Generic;
using System.Text;

namespace FileCurator.Data.FixedLength.BaseClasses
{
    /// <summary>
    /// Record base class
    /// </summary>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <seealso cref="IRecord"/>
    public abstract class RecordBase<TField> : IRecord, IList<IField<TField>>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected RecordBase()
        {
            Fields = new List<IField<TField>>();
        }

        /// <summary>
        /// Number of Fields
        /// </summary>
        public int Count => Fields.Count;

        /// <summary>
        /// Is the file read only
        /// </summary>
        public bool IsReadOnly => Fields.IsReadOnly;

        /// <summary>
        /// Length
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// The list of fields
        /// </summary>
        protected IList<IField<TField>> Fields { get; }

        /// <summary>
        /// Individual records
        /// </summary>
        /// <param name="index">The record that you want to get</param>
        /// <returns>The record requested</returns>
        public IField<TField> this[int index]
        {
            get { return Fields[index]; }
            set { Fields[index] = value; }
        }

        /// <summary>
        /// Adds a Field to the file
        /// </summary>
        /// <param name="item">Field to add</param>
        public void Add(IField<TField> item)
        {
            Fields.Add(item);
        }

        /// <summary>
        /// Clears the file
        /// </summary>
        public void Clear()
        {
            Fields.Clear();
        }

        /// <summary>
        /// Determines if the file contains a Field
        /// </summary>
        /// <param name="item">Field to check for</param>
        /// <returns>True if it does, false otherwise</returns>
        public bool Contains(IField<TField> item)
        {
            return Fields.Contains(item);
        }

        /// <summary>
        /// Copies the delimited file to an array
        /// </summary>
        /// <param name="array">Array to copy to</param>
        /// <param name="arrayIndex">Index to start at</param>
        public void CopyTo(IField<TField>[] array, int arrayIndex)
        {
            Fields.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Gets the enumerator for the file
        /// </summary>
        /// <returns>The enumerator for this file</returns>
        public IEnumerator<IField<TField>> GetEnumerator()
        {
            return Fields.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator for the file
        /// </summary>
        /// <returns>The enumerator for this file</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Fields.GetEnumerator();
        }

        /// <summary>
        /// Index of a specific Field
        /// </summary>
        /// <param name="item">Field to search for</param>
        /// <returns>The index of a specific Field</returns>
        public int IndexOf(IField<TField> item)
        {
            return Fields.IndexOf(item);
        }

        /// <summary>
        /// Inserts a Field at a specific index
        /// </summary>
        /// <param name="index">Index to insert at</param>
        /// <param name="item">Field to insert</param>
        public void Insert(int index, IField<TField> item)
        {
            Fields.Insert(index, item);
        }

        /// <summary>
        /// Parses the record
        /// </summary>
        /// <param name="value">Value</param>
        /// <param name="length">Length of the record</param>
        public abstract void Parse(string value, int length = -1);

        /// <summary>
        /// Removes a Field from the file
        /// </summary>
        /// <param name="item">Field to remove</param>
        /// <returns>True if it is removed, false otherwise</returns>
        public bool Remove(IField<TField> item)
        {
            return Fields.Remove(item);
        }

        /// <summary>
        /// Removes a Field at a specific index
        /// </summary>
        /// <param name="index">Index of the Field to remove</param>
        public void RemoveAt(int index)
        {
            Fields.RemoveAt(index);
        }

        /// <summary>
        /// Converts the record to a string
        /// </summary>
        /// <returns>The record as a string</returns>
        public override string ToString()
        {
            var Builder = new StringBuilder();
            foreach (IField<TField> Field in Fields)
                Builder.Append(Field.ToString());
            return Builder.ToString();
        }
    }
}