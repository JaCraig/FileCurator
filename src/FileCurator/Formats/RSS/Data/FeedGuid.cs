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

*/using FileCurator.Formats.Data.Interfaces;
using System;
using System.Xml.XPath;

namespace FileCurator.Formats.RSS.Data
{
    /// <summary>
    /// Feed GUID
    /// </summary>
    /// <seealso cref="IFeedGuid"/>
    public class FeedGuid : IFeedGuid
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FeedGuid()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">XML element holding info for the enclosure</param>
        public FeedGuid(IXPathNavigable element)
        {
            if (element is null)
                throw new ArgumentNullException(nameof(element));
            var Navigator = element.CreateNavigator();
            if (!string.IsNullOrEmpty(Navigator.GetAttribute("isPermaLink", "")))
            {
                IsPermaLink = bool.Parse(Navigator.GetAttribute("isPermaLink", ""));
            }
            GuidText = Navigator.Value;
        }

        /// <summary>
        /// GUID Text
        /// </summary>
        public string? GuidText { get; set; }

        /// <summary>
        /// Is this a perma link?
        /// </summary>
        public bool IsPermaLink { get; set; }

        /// <summary>
        /// to string item. Used for outputting the item to RSS.
        /// </summary>
        /// <returns>A string formatted for RSS output</returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(GuidText))
                return string.Empty;
            return "<guid" + (IsPermaLink ? " IsPermaLink='True'" : " IsPermaLink='False'") + ">" + GuidText + "</guid>\r\n";
        }
    }
}