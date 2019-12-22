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

namespace FileCurator.Enums
{
    /// <summary>
    /// Mime type enum like static class
    /// </summary>
    public class MimeType
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MimeType"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        protected MimeType(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the CSV.
        /// </summary>
        /// <value>The CSV.</value>
        public static MimeType CSV { get; } = new MimeType("TEXT/CSV");

        /// <summary>
        /// Gets the excel.
        /// </summary>
        /// <value>The excel.</value>
        public static MimeType Excel { get; } = new MimeType("APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.SPREADSHEETML.SHEET");

        /// <summary>
        /// Gets the HTML.
        /// </summary>
        /// <value>The HTML.</value>
        public static MimeType Html { get; } = new MimeType("TEXT/HTML");

        /// <summary>
        /// Gets the i cal.
        /// </summary>
        /// <value>The i cal.</value>
        public static MimeType ICal { get; } = new MimeType("TEXT/CALENDAR");

        /// <summary>
        /// Gets the power point.
        /// </summary>
        /// <value>The power point.</value>
        public static MimeType PowerPoint { get; } = new MimeType("APPLICATION/VND.MS-POWERPOINT");

        /// <summary>
        /// Gets the RSS.
        /// </summary>
        /// <value>The RSS.</value>
        public static MimeType RSS { get; } = new MimeType("APPLICATION/RSS+XML");

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <value>The text.</value>
        public static MimeType Text { get; } = new MimeType("TEXT/PLAIN");

        /// <summary>
        /// Gets the unknown.
        /// </summary>
        /// <value>The unknown.</value>
        public static MimeType Unknown { get; } = new MimeType("");

        /// <summary>
        /// Gets the v cal.
        /// </summary>
        /// <value>The v cal.</value>
        public static MimeType VCal { get; } = new MimeType("APPLICATION/HBS-VCS");

        /// <summary>
        /// Gets the v card.
        /// </summary>
        /// <value>The v card.</value>
        public static MimeType VCard { get; } = new MimeType("TEXT/VCARD");

        /// <summary>
        /// Gets the word.
        /// </summary>
        /// <value>The word.</value>
        public static MimeType Word { get; } = new MimeType("APPLICATION/VND.OPENXMLFORMATS-OFFICEDOCUMENT.WORDPROCESSINGML.DOCUMENT");

        /// <summary>
        /// Gets the XML.
        /// </summary>
        /// <value>The XML.</value>
        public static MimeType XML { get; } = new MimeType("TEXT/XML");

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>The name.</value>
        private string Name { get; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="System.String"/> to <see cref="MimeType"/>.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator MimeType(string name)
        {
            return new MimeType(name);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="MimeType"/> to <see cref="System.String"/>.
        /// </summary>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator string(MimeType mimeType)
        {
            return mimeType.ToString();
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents this instance.</returns>
        public override string ToString() => Name;
    }
}