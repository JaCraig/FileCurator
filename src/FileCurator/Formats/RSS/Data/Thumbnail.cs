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
using System.Globalization;
using System.Xml.XPath;

namespace FileCurator.Formats.RSS.Data
{
    /// <summary>
    /// Thumbnail
    /// </summary>
    /// <seealso cref="IThumbnail"/>
    public class Thumbnail : IThumbnail
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Thumbnail()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc">XML element holding info for the enclosure</param>
        public Thumbnail(IXPathNavigable doc)
        {
            if (doc is null)
                throw new ArgumentNullException(nameof(doc));
            var Element = doc.CreateNavigator();
            Url = Element.GetAttribute("url", "");
            if (!string.IsNullOrEmpty(Element.GetAttribute("width", "")))
            {
                Width = int.Parse(Element.GetAttribute("width", ""), CultureInfo.InvariantCulture);
            }
            if (!string.IsNullOrEmpty(Element.GetAttribute("height", "")))
            {
                Height = int.Parse(Element.GetAttribute("height", ""), CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Image height
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Location of the item
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Image width
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// to string item. Used for outputting the item to RSS.
        /// </summary>
        /// <returns>A string formatted for RSS output</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Url))
            {
                return "<media:thumbnail url=\"" + Url + "\" width=\"" + Width.ToString(CultureInfo.InvariantCulture) + "\" height=\"" + Height + "\" />\r\n";
            }
            return string.Empty;
        }
    }
}