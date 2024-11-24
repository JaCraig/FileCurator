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

using System;
using System.Collections.Generic;

namespace FileCurator.Formats.Data.Interfaces
{
    /// <summary>
    /// Calendar item
    /// </summary>
    /// <seealso cref="IGenericFile"/>
    public interface ICalendar : IGenericFile
    {
        /// <summary>
        /// List of attendees
        /// </summary>
        IList<IMailAddress> AttendeeList { get; }

        /// <summary>
        /// Determines if the calendar item is being canceled
        /// </summary>
        bool Cancel { get; set; }

        /// <summary>
        /// The time zone for the calendar event
        /// </summary>
        TimeZoneInfo CurrentTimeZone { get; set; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        string Description { get; set; }

        /// <summary>
        /// The end time
        /// </summary>
        DateTime EndTime { get; set; }

        /// <summary>
        /// The location of the event
        /// </summary>
        string Location { get; set; }

        /// <summary>
        /// Organizer
        /// </summary>
        IMailAddress Organizer { get; set; }

        /// <summary>
        /// The start time
        /// </summary>
        DateTime StartTime { get; set; }

        /// <summary>
        /// Sets the status for the appointment (FREE, BUSY, etc.)
        /// </summary>
        string Status { get; set; }

        /// <summary>
        /// Gets the subject.
        /// </summary>
        /// <value>The subject.</value>
        string Subject { get; set; }
    }
}