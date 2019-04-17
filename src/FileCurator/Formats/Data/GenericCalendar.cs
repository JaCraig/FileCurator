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

using FileCurator.Formats.Data.Interfaces;
using System;
using System.Collections.Generic;

namespace FileCurator.Formats.Data
{
    /// <summary>
    /// Generic calendar item
    /// </summary>
    /// <seealso cref="ICalendar"/>
    public class GenericCalendar : Interfaces.ICalendar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericCalendar"/> class.
        /// </summary>
        public GenericCalendar()
        {
            AttendeeList = new List<IMailAddress>();
            Status = "BUSY";
            CurrentTimeZone = TimeZoneInfo.Local;
        }

        /// <summary>
        /// List of attendees
        /// </summary>
        public IList<IMailAddress> AttendeeList { get; }

        /// <summary>
        /// Determines if the calendar item is being canceled
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// Parsed content
        /// </summary>
        /// <value>The content.</value>
        public string Content => Description;

        /// <summary>
        /// The time zone for the calendar event
        /// </summary>
        public TimeZoneInfo CurrentTimeZone { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string Description { get; set; }

        /// <summary>
        /// The end time
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// The location of the event
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Meta data
        /// </summary>
        /// <value>The meta.</value>
        public string Meta { get; } = "";

        /// <summary>
        /// Organizer
        /// </summary>
        public IMailAddress Organizer { get; set; }

        /// <summary>
        /// The start time
        /// </summary>
        public DateTime StartTime { get; set; }

        /// <summary>
        /// Sets the status for the appointment (FREE, BUSY, etc.)
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject { get; set; }

        /// <summary>
        /// Parsed title
        /// </summary>
        /// <value>The title.</value>
        public string Title => Subject;

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString()
        {
            return "Type:Single Meeting\r\n" +
                "Organizer:" + (Organizer == null ? "" : Organizer.Name) + "\r\n" +
                "Start Time:" + StartTime.ToString("dddd, MMMM dd, yyyy") + " " + StartTime.ToString("dddd, MMMM dd, yyyy") + "\r\n" +
                "End Time:" + EndTime.ToString("dddd, MMMM dd, yyyy") + " " + EndTime.ToString("dddd, MMMM dd, yyyy") + "\r\n" +
                "Time Zone:" + TimeZoneInfo.Local.StandardName + "\r\n" +
                "Location: " + Location + "\r\n\r\n" +
                "*~*~*~*~*~*~*~*~*~*\r\n\r\n" +
                Description;
        }
    }
}