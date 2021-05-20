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
using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace FileCurator.Formats.VCard
{
    /// <summary>
    /// Vcard reader
    /// </summary>
    /// <seealso cref="ReaderBaseClass{ICard}"/>
    public class VCardReader : ReaderBaseClass<ICard>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier { get; } = new byte[] { 0x42, 0x45, 0x47, 0x49, 0x4E, 0x3A, 0x56, 0x43, 0x41, 0x52, 0x44 };

        /// <summary>
        /// Gets the entry regex.
        /// </summary>
        /// <value>The entry regex.</value>
        private static Regex EntryRegex { get; } = new Regex("(?<Title>[^:]+):(?<Value>.*)", RegexOptions.Compiled);

        /// <summary>
        /// Gets the type regex.
        /// </summary>
        /// <value>The type regex.</value>
        private static Regex TypeRegex { get; } = new Regex("TYPE=(?<Title>[^:]+)", RegexOptions.Compiled);

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override ICard Read(Stream stream)
        {
            var ReturnValue = new GenericCard();
            string Content = GetData(stream);
            foreach (Match TempMatch in EntryRegex.Matches(Content))
            {
                var Title = TempMatch.Groups["Title"].Value.ToUpperInvariant().Trim();
                var Value = TempMatch.Groups["Value"].Value.Trim();
                if (Title == "N")
                {
                    var Name = Value.Split(';');
                    if (Name.Length > 0)
                    {
                        ReturnValue.LastName = Name[0];
                        if (Name.Length > 1)
                            ReturnValue.FirstName = Name[1];
                        if (Name.Length > 2)
                            ReturnValue.MiddleName = Name[2];
                        if (Name.Length > 3)
                            ReturnValue.Prefix = Name[3];
                        if (Name.Length > 4)
                            ReturnValue.Suffix = Name[4];
                    }
                }
                else if (Title.StartsWith("TEL", StringComparison.Ordinal))
                {
                    var Type = TypeRegex.Match(Title);
                    ReturnValue.DirectDial.Add(new PhoneNumber
                    {
                        Number = Value,
                        Type = Type?.Groups["Title"].Value ?? string.Empty
                    });
                }
                else if (Title.StartsWith("EMAIL", StringComparison.Ordinal))
                {
                    var Type = TypeRegex.Match(Title);
                    ReturnValue.Email.Add(new MailAddress
                    {
                        EmailAddress = Value,
                        Type = Type?.Groups["Title"].Value ?? string.Empty
                    });
                }
                else if (Title.StartsWith("TITLE", StringComparison.Ordinal))
                {
                    ReturnValue.Title = Value;
                }
                else if (Title.StartsWith("ORG", StringComparison.Ordinal))
                {
                    ReturnValue.Organization = Value;
                }
                else if (Title.StartsWith("URL", StringComparison.Ordinal))
                {
                    ReturnValue.Url = Value;
                }
                else if (Title.StartsWith("ADR", StringComparison.Ordinal))
                {
                    var Type = TypeRegex.Match(Title);
                    var Name = Value.Split(';');
                    ReturnValue.Addresses.Add(new Address
                    {
                        Name = Name[1],
                        Type = Type?.Groups["Title"].Value ?? string.Empty,
                        Street = Name[2],
                        City = Name[3],
                        StateOrProvence = Name[4],
                        ZipCode = Name[5],
                        Country = Name[6]
                    });
                }
            }
            return ReturnValue;
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns></returns>
        private static string GetData(Stream stream)
        {
            try
            {
                return stream?.ReadAll().Replace("\r\n ", string.Empty) ?? "";
            }
            catch
            {
                return "";
            }
        }
    }
}