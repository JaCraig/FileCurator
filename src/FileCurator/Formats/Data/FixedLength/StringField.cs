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

using BigBook;
using FileCurator.Data.FixedLength.BaseClasses;
using System.Text;

namespace FileCurator.Data.FixedLength
{
    /// <summary>
    /// Basic string field
    /// </summary>
    /// <seealso cref="BaseClasses.FieldBaseClass{System.String}"/>
    public class StringField : FieldBaseClass<string>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringField"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        public StringField(string value, int length = -1)
        {
            Parse(value, length);
        }

        /// <summary>
        /// Parses the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length.</param>
        /// <param name="fillerCharacter">The filler character.</param>
        public override void Parse(string value, int length = -1, string fillerCharacter = " ")
        {
            Value = value;
            Length = length >= 0 ? length : Value.Length;
            if (Value.Length > Length)
            {
                Value = Value.Left(Length);
                return;
            }
            if (Value.Length == Length)
                return;
            var Builder = new StringBuilder();
            Builder.Append(Value);
            while (Builder.Length < Length)
                Builder.Append(fillerCharacter);
            Value = Builder.ToString();
        }
    }
}