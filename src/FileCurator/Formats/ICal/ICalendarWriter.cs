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
using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace FileCurator.Formats.ICal
{
    /// <summary>
    /// ICal writer
    /// </summary>
    /// <seealso cref="IGenericFileWriter"/>
    public class ICalendarWriter : IGenericFileWriter
    {
        /// <summary>
        /// The strip HTML regex
        /// </summary>
        private static readonly Regex STRIP_HTML_REGEX = new Regex("<[^>]*>", RegexOptions.Compiled);

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public bool Write(Stream writer, IGenericFile file)
        {
            if (file is Data.Interfaces.ICalendar CalendarFile)
            {
                WriteCalendar(writer, CalendarFile);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Determines whether the specified input contains HTML.
        /// </summary>
        /// <param name="Input">The input.</param>
        /// <returns><c>true</c> if the specified input contains HTML; otherwise, <c>false</c>.</returns>
        private static bool ContainsHTML(string Input)
        {
            if (string.IsNullOrEmpty(Input))
                return false;

            return STRIP_HTML_REGEX.IsMatch(Input);
        }

        /// <summary>
        /// Strips the HTML.
        /// </summary>
        /// <param name="HTML">The HTML.</param>
        /// <returns></returns>
        private static string StripHTML(string HTML)
        {
            if (string.IsNullOrEmpty(HTML))
                return string.Empty;

            HTML = STRIP_HTML_REGEX.Replace(HTML, string.Empty);
            HTML = HTML.Replace("&nbsp;", " ");
            return HTML.Replace("&#160;", string.Empty);
        }

        /// <summary>
        /// Writes the calendar.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="calendarFile">The calendar file.</param>
        private void WriteCalendar(Stream writer, Data.Interfaces.ICalendar calendarFile)
        {
            var FileOutput = new StringBuilder();
            var StartTime = (calendarFile.StartTime - calendarFile.CurrentTimeZone.BaseUtcOffset);
            var EndTime = (calendarFile.EndTime - calendarFile.CurrentTimeZone.BaseUtcOffset);
            FileOutput.AppendLine("BEGIN:VCALENDAR")
                      .AppendLineFormat("METHOD:{0}", calendarFile.Cancel ? "CANCEL" : "REQUEST")
                      .AppendLine("PRODID:-//Craigs Utility Library//EN")
                      .AppendLine("VERSION:2.0")
                      .AppendLine("BEGIN:VEVENT")
                      .AppendLine("CLASS:PUBLIC")
                      .AppendLineFormat("DTSTAMP:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                      .AppendLineFormat("CREATED:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                      .AppendLine(StripHTML(calendarFile.Description.Replace("<br />", System.Environment.NewLine)))
                      .AppendLineFormat("DTStart:{0}", StartTime.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                      .AppendLineFormat("DTEnd:{0}", EndTime.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                      .AppendLineFormat("LOCATION:{0}", calendarFile.Location)
                      .AppendLineFormat("SUMMARY;LANGUAGE=en-us:{0}", calendarFile.Subject)
                      .AppendLineFormat("UID:{0}{1}{2}", StartTime.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture), EndTime.ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture), calendarFile.Subject);
            if (calendarFile.AttendeeList.Count > 0)
                FileOutput.AppendLineFormat("ATTENDEE;ROLE=REQ-PARTICIPANT;PARTSTAT=NEEDS-ACTION;RSVP=TRUE;CN=\"{0}\":MAILTO:{0}", calendarFile.AttendeeList.ToString(x => x.EmailAddress, ";"));
            if (calendarFile.Organizer != null)
                FileOutput.AppendLineFormat("ACTION;RSVP=TRUE;CN=\"{0}\":MAILTO:{0}\r\nORGANIZER;CN=\"{1}\":mailto:{0}", calendarFile.Organizer.EmailAddress, calendarFile.Organizer.Name);
            if (ContainsHTML(calendarFile.Description))
            {
                FileOutput.AppendLineFormat("X-ALT-DESC;FMTTYPE=text/html:{0}", calendarFile.Description.Replace("\n", ""));
            }
            else
            {
                FileOutput.AppendLineFormat("DESCRIPTION:{0}", calendarFile.Description);
            }
            FileOutput.AppendLine("SEQUENCE:1")
                             .AppendLine("PRIORITY:5")
                             .AppendLine("CLASS:")
                             .AppendLineFormat("LAST-MODIFIED:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                             .AppendLine("STATUS:CONFIRMED")
                             .AppendLine("TRANSP:OPAQUE")
                             .AppendLineFormat("X-MICROSOFT-CDO-BUSYSTATUS:{0}", calendarFile.Status)
                             .AppendLine("X-MICROSOFT-CDO-INSTTYPE:0")
                             .AppendLine("X-MICROSOFT-CDO-INTENDEDSTATUS:BUSY")
                             .AppendLine("X-MICROSOFT-CDO-ALLDAYEVENT:FALSE")
                             .AppendLine("X-MICROSOFT-CDO-IMPORTANCE:1")
                             .AppendLine("X-MICROSOFT-CDO-OWNERAPPTID:-1")
                             .AppendLineFormat("X-MICROSOFT-CDO-ATTENDEE-CRITICAL-CHANGE:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                             .AppendLineFormat("X-MICROSOFT-CDO-OWNER-CRITICAL-CHANGE:{0}", DateTime.Now.ToUniversalTime().ToString("yyyyMMddTHHmmssZ", CultureInfo.InvariantCulture))
                             .AppendLine("BEGIN:VALARM")
                             .AppendLine("TRIGGER;RELATED=START:-PT00H15M00S")
                             .AppendLine("ACTION:DISPLAY")
                             .AppendLine("DESCRIPTION:Reminder")
                             .AppendLine("END:VALARM")
                             .AppendLine("END:VEVENT")
                             .AppendLine("END:VCALENDAR");
            var ByteData = Encoding.UTF8.GetBytes(FileOutput.ToString());
            writer.Write(ByteData, 0, ByteData.Length);
        }
    }
}