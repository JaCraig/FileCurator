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
    /// Address interface
    /// </summary>
    public interface IAddress
    {
        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        /// <value>The city.</value>
        string? City { get; set; }

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        string? Country { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        string? Name { get; set; }

        /// <summary>
        /// Gets or sets the state or provence.
        /// </summary>
        /// <value>The state or provence.</value>
        string? StateOrProvence { get; set; }

        /// <summary>
        /// Gets or sets the street.
        /// </summary>
        /// <value>The street.</value>
        string? Street { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        string? Type { get; set; }

        /// <summary>
        /// Gets or sets the area code.
        /// </summary>
        /// <value>The area code.</value>
        string? ZipCode { get; set; }
    }
}