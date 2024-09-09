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
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileCurator.Formats.VCalendar
{
    /// <summary>
    /// VCal writer
    /// </summary>
    /// <seealso cref="IGenericFileWriter"/>
    public class VCalendarWriter : IGenericFileWriter
    {
        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public bool Write(Stream writer, IGenericFile file)
        {
            if (file is ICalendar CalendarFile)
            {
                WriteCalendar(writer, CalendarFile);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public async Task<bool> WriteAsync(Stream writer, IGenericFile file)
        {
            if (file is ICalendar CalendarFile)
            {
                await WriteCalendarAsync(writer, CalendarFile).ConfigureAwait(false);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Generates the file.
        /// </summary>
        /// <param name="calendarFile">The calendar file.</param>
        /// <returns></returns>
        private static byte[] GenerateFile(ICalendar calendarFile)
        {
            return Encoding.UTF8.GetBytes(new StringBuilder().AppendLine("BEGIN:VCALENDAR")
                      .AppendLine("VERSION:1.0")
                      .AppendLine("BEGIN:VEVENT")
                      .AppendLineFormat("DTSTART:{0}Z", (calendarFile.StartTime - calendarFile.CurrentTimeZone.BaseUtcOffset).ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture))
                      .AppendLineFormat("DTEND:{0}Z", (calendarFile.EndTime - calendarFile.CurrentTimeZone.BaseUtcOffset).ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture))
                      .AppendLineFormat("LOCATION;ENCODING=QUOTED-PRINTABLE:{0}", calendarFile.Location)
                      .AppendLineFormat("SUMMARY;ENCODING=QUOTED-PRINTABLE:{0}", calendarFile.Subject)
                      .AppendLineFormat("DESCRIPTION;ENCODING=QUOTED-PRINTABLE:{0}", calendarFile.Description.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "\\t").Replace(",", "\\,"))
                      .AppendLineFormat("UID:{0}{1}{2}", (calendarFile.StartTime - calendarFile.CurrentTimeZone.BaseUtcOffset).ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture), (calendarFile.EndTime - calendarFile.CurrentTimeZone.BaseUtcOffset).ToString("yyyyMMddTHHmmss", CultureInfo.InvariantCulture), calendarFile.Subject)
                      .AppendLine("PRIORITY:3")
                      .AppendLine("End:VEVENT")
                      .AppendLine("End:VCALENDAR")
                      .ToString());
        }

        /// <summary>
        /// Writes the calendar.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="calendarFile">The calendar file.</param>
        private void WriteCalendar(Stream writer, ICalendar calendarFile)
        {
            byte[] ByteData = GenerateFile(calendarFile);
            writer.Write(ByteData, 0, ByteData.Length);
        }

        /// <summary>
        /// Writes the calendar asynchronous.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="calendarFile">The calendar file.</param>
        /// <returns></returns>
        private Task WriteCalendarAsync(Stream writer, ICalendar calendarFile)
        {
            byte[] ByteData = GenerateFile(calendarFile);
            return writer.WriteAsync(ByteData, 0, ByteData.Length);
        }
    }
}