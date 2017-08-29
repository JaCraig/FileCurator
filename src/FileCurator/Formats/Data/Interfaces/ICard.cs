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

using FileCurator.Formats.Data.Enums;
using FileCurator.Formats.Data.Interface;
using System.Collections.Generic;

namespace FileCurator.Formats.Data.Interfaces
{
    /// <summary>
    /// Card file type
    /// </summary>
    /// <seealso cref="IGenericFile"/>
    public interface ICard : IGenericFile
    {
        /// <summary>
        /// Work phone number of the individual
        /// </summary>
        IList<IPhoneNumber> DirectDial { get; set; }

        /// <summary>
        /// Email of the individual
        /// </summary>
        IList<IMailAddress> Email { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the full name.
        /// </summary>
        /// <value>The full name.</value>
        string FullName { get; }

        /// <summary>
        /// Last name
        /// </summary>
        string LastName { get; set; }

        /// <summary>
        /// Middle name
        /// </summary>
        string MiddleName { get; set; }

        /// <summary>
        /// Organization the person belongs to
        /// </summary>
        string Organization { get; set; }

        /// <summary>
        /// Prefix
        /// </summary>
        string Prefix { get; set; }

        /// <summary>
        /// Relationship to the person (uses XFN)
        /// </summary>
        IList<Relationship> Relationships { get; }

        /// <summary>
        /// Suffix
        /// </summary>
        string Suffix { get; set; }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        new string Title { get; set; }

        /// <summary>
        /// Url to the person's site
        /// </summary>
        string Url { get; set; }
    }
}