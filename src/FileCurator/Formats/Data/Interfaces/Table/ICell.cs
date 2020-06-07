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

namespace FileCurator.Formats.Data.Interfaces
{
    /// <summary>
    /// Cell interface
    /// </summary>
    public interface ICell
    {
        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        string Content { get; set; }

        /// <summary>
        /// Gets the value as the type specified.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <returns>The content of the cell as the value type specified.</returns>
        TValue GetValue<TValue>();
    }
}