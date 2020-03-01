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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace FileCurator.Formats.VCalendar
{
    /// <summary>
    /// VCal reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{ICalendar}"/>
    public class VCalendarReader : ReaderBaseClass<ICalendar>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier { get; } = Array.Empty<byte>();

        /// <summary>
        /// Gets the entry regex.
        /// </summary>
        /// <value>The entry regex.</value>
        private static Regex EntryRegex { get; } = new Regex("(?<Title>[^\r\n:]+):(?<Value>[^\r\n]*)", RegexOptions.Compiled);

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override ICalendar Read(Stream stream)
        {
            var StringData = stream.ReadAll();
            var ReturnValue = new GenericCalendar();
            foreach (Match TempMatch in EntryRegex.Matches(StringData))
            {
                var Title = TempMatch.Groups["Title"].Value.ToUpperInvariant().Trim();
                var Value = TempMatch.Groups["Value"].Value.Trim();
                if (Title.StartsWith("DTSTART", StringComparison.Ordinal))
                {
                    ReturnValue.StartTime = DateTime.Parse(Value.ToString("####/##/## ##:##"), CultureInfo.CurrentCulture) + ReturnValue.CurrentTimeZone.BaseUtcOffset;
                }
                else if (Title.StartsWith("DTEND", StringComparison.Ordinal))
                {
                    ReturnValue.EndTime = DateTime.Parse(Value.ToString("####/##/## ##:##"), CultureInfo.CurrentCulture) + ReturnValue.CurrentTimeZone.BaseUtcOffset;
                }
                else if (Title.StartsWith("LOCATION", StringComparison.Ordinal))
                {
                    ReturnValue.Location = Value;
                }
                else if (Title.StartsWith("SUMMARY", StringComparison.Ordinal))
                {
                    ReturnValue.Subject = Value;
                }
                else if (Title.StartsWith("LOCATION", StringComparison.Ordinal))
                {
                    ReturnValue.Location = Value;
                }
                else if (Title.StartsWith("DESCRIPTION", StringComparison.Ordinal) && string.IsNullOrEmpty(ReturnValue.Description))
                {
                    ReturnValue.Description = Value.Replace("\\n", "\n").Replace("\\,", ",").Replace("\\r", "").Replace("\\t", "\t");
                }
            }
            return ReturnValue;
        }
    }
}