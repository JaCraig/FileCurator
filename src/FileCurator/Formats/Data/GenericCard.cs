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
using FileCurator.Formats.Data.Enums;
using FileCurator.Formats.Data.Interface;
using FileCurator.Formats.Data.Interfaces;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace FileCurator.Formats.Data
{
    /// <summary>
    /// Generic card
    /// </summary>
    /// <seealso cref="ICard"/>
    public class GenericCard : ICard
    {
        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>The addresses.</value>
        public IList<IAddress> Addresses { get; set; } = new List<IAddress>();

        /// <summary>
        /// Parsed content
        /// </summary>
        /// <value>The content.</value>
        public string Content => FullName;

        /// <summary>
        /// Work phone number of the individual
        /// </summary>
        public IList<IPhoneNumber> DirectDial { get; set; } = new List<IPhoneNumber>();

        /// <summary>
        /// Email of the individual
        /// </summary>
        public IList<IMailAddress> Email { get; set; } = new List<IMailAddress>();

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Full name
        /// </summary>
        public string FullName
        {
            get
            {
                var Builder = new StringBuilder();
                if (!string.IsNullOrEmpty(Prefix))
                {
                    Builder.AppendFormat("{0} ", Prefix);
                }
                Builder.AppendFormat("{0} ", FirstName);
                if (!string.IsNullOrEmpty(MiddleName))
                {
                    Builder.AppendFormat("{0} ", MiddleName);
                }
                Builder.Append(LastName);
                if (!string.IsNullOrEmpty(Suffix))
                {
                    Builder.AppendFormat(" {0}", Suffix);
                }
                return Builder.ToString();
            }
        }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Meta data
        /// </summary>
        /// <value>The meta.</value>
        public string Meta { get; }

        /// <summary>
        /// Middle name
        /// </summary>
        public string MiddleName { get; set; }

        /// <summary>
        /// Organization the person belongs to
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// Prefix
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// Relationship to the person (uses XFN)
        /// </summary>
        public IList<Relationship> Relationships { get; } = new List<Relationship>();

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject { get; set; }

        /// <summary>
        /// Suffix
        /// </summary>
        public string Suffix { get; set; }

        /// <summary>
        /// Parsed title
        /// </summary>
        /// <value>The title.</value>
        public string Title { get; set; }

        /// <summary>
        /// Url to the person's site
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        protected string Name => $"{LastName};{FirstName};{MiddleName};{Prefix};{Suffix}";

        /// <summary>
        /// Gets the VCard as a string
        /// </summary>
        /// <returns>VCard as a string</returns>
        public override string ToString()
        {
            return new StringBuilder().Append("BEGIN:VCARD\r\nVERSION:2.1\r\n")
                .AppendFormat(CultureInfo.CurrentCulture, "FN:{0}\r\n", FullName)
                .AppendFormat(CultureInfo.CurrentCulture, "N:{0}\r\n", Name)
                .AppendLine(DirectDial.ToString(x => $"TEL;TYPE={x.Type}:{x.Number}", "\n"))
                .AppendLine(Email.ToString(x => $"EMAIL;TYPE={x.Type}:{x.EmailAddress}", "\n"))
                .AppendLine(Addresses.ToString(x => $"ADR;TYPE={x.Type}:;{x.Name};{x.Street};{x.City};{x.StateOrProvence};{x.ZipCode};{x.Country}", "\n"))
                .AppendFormat(CultureInfo.CurrentCulture, "TITLE:{0}\r\n", Title)
                .AppendFormat(CultureInfo.CurrentCulture, "ORG:{0}\r\n", Organization)
                .AppendFormat(CultureInfo.CurrentCulture, "END:VCARD\r\n")
                .ToString();
        }
    }
}