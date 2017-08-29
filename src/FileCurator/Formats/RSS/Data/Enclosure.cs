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
using System.Xml.XPath;

namespace FileCurator.Formats.RSS.Data
{
    /// <summary>
    /// Enclosure
    /// </summary>
    /// <seealso cref="IEnclosure"/>
    public class Enclosure : IEnclosure
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public Enclosure()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc">XML element holding info for the enclosure</param>
        public Enclosure(IXPathNavigable doc)
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));
            var Element = doc.CreateNavigator();
            if (!string.IsNullOrEmpty(Element.GetAttribute("url", "")))
            {
                Url = Element.GetAttribute("url", "");
            }
            if (!string.IsNullOrEmpty(Element.GetAttribute("length", "")))
            {
                Length = int.Parse(Element.GetAttribute("length", ""));
            }
            if (!string.IsNullOrEmpty(Element.GetAttribute("type", "")))
            {
                Type = Element.GetAttribute("type", "");
            }
        }

        /// <summary>
        /// Size in bytes
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// File type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Location of the item
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// to string item. Used for outputting the item to RSS.
        /// </summary>
        /// <returns>A string formatted for RSS output</returns>
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(Url) && !string.IsNullOrEmpty(Type))
            {
                return "<enclosure url=\"" + Url + "\" length=\"" + Length + "\" type=\"" + Type + "\" />\r\n"
                    + "<media:content url=\"" + Url + "\" fileSize=\"" + Length + "\" type=\"" + Type + "\" />";
            }
            return string.Empty;
        }
    }
}