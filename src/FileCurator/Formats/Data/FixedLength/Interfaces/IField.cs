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

namespace FileCurator.Data.FixedLength.Interfaces
{
    /// <summary>
    /// Field interface
    /// </summary>
    /// <typeparam name="TField">The type of the field.</typeparam>
    public interface IField<TField>
    {
        /// <summary>
        /// Gets or sets the length.
        /// </summary>
        /// <value>The length.</value>
        int Length { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        TField Value { get; set; }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="fillerCharacter">The filler character.</param>
        void Parse(string value, int length = -1, string fillerCharacter = " ");
    }
}