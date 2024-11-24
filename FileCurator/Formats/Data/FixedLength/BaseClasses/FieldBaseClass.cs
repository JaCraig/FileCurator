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

namespace FileCurator.Data.FixedLength.BaseClasses
{
    /// <summary>
    /// Field base class
    /// </summary>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <seealso cref="Interfaces.IField{TField}"/>
    public abstract class FieldBaseClass<TField> : IField<TField>
    {
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        public int Length { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        public TField Value { get; set; }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="fillerCharacter">The filler character.</param>
        public abstract void Parse(string value, int length = -1, string fillerCharacter = " ");

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString() => Value.ToString();
    }
}