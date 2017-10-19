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
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace FileCurator.Formats.RSS.Data
{
    /// <summary>
    /// Feed item
    /// </summary>
    /// <seealso cref="IFeedItem"/>
    public class FeedItem : IFeedItem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FeedItem()
        {
            Categories = new List<string>();
            PubDate = DateTime.Now;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc">XML element containing the item content</param>
        public FeedItem(IXPathNavigable doc)
            : this()
        {
            if (doc == null)
                throw new ArgumentNullException(nameof(doc));
            var Element = doc.CreateNavigator();
            var NamespaceManager = new XmlNamespaceManager(Element.NameTable);
            NamespaceManager.AddNamespace("media", "http://search.yahoo.com/mrss/");
            var Node = Element.SelectSingleNode("./title", NamespaceManager);
            if (Node != null)
            {
                Title = Node.Value;
            }
            Node = Element.SelectSingleNode("./link", NamespaceManager);
            if (Node != null)
            {
                Link = Node.Value;
            }
            Node = Element.SelectSingleNode("./description", NamespaceManager);
            if (Node != null)
            {
                Description = Node.Value;
            }
            Node = Element.SelectSingleNode("./author", NamespaceManager);
            if (Node != null)
            {
                Author = Node.Value;
            }
            var Nodes = Element.Select("./category", NamespaceManager);
            foreach (XPathNavigator TempNode in Nodes)
            {
                Categories.Add(Utils.StripIllegalCharacters(TempNode.Value));
            }
            Node = Element.SelectSingleNode("./enclosure", NamespaceManager);
            if (Node != null)
            {
                Enclosure = new Enclosure(Node);
            }
            Node = Element.SelectSingleNode("./pubDate", NamespaceManager);
            if (Node != null)
            {
                if (DateTime.TryParse(Node.Value.Replace("PDT", "-0700"), out DateTime TempDate))
                {
                    PubDate = TempDate;
                }
                PubDate = DateTime.Now;
            }
            Node = Element.SelectSingleNode("./media:thumbnail", NamespaceManager);
            if (Node != null)
            {
                Thumbnail = new Thumbnail(Node);
            }
            Node = Element.SelectSingleNode("./guid", NamespaceManager);
            if (Node != null)
            {
                GUID = new FeedGuid(Node);
            }
        }

        /// <summary>
        /// Author
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Categories
        /// </summary>
        public IList<string> Categories { get; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content => Description;

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Enclosure
        /// </summary>
        public IEnclosure Enclosure { get; set; }

        /// <summary>
        /// GUID for the item
        /// </summary>
        public virtual IFeedGuid GUID { get; set; }

        /// <summary>
        /// Link
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Publication date
        /// </summary>
        public DateTime PubDate { get; set; }

        /// <summary>
        /// Thumbnail
        /// </summary>
        public IThumbnail Thumbnail { get; set; }

        /// <summary>
        /// Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Outputs a string ready for RSS
        /// </summary>
        /// <returns>A string formatted for RSS</returns>
        public override string ToString()
        {
            var ItemString = new StringBuilder();
            ItemString.Append("<item><title>").Append(Title).Append("</title>\r\n<link>")
                .Append(Link).Append("</link>\r\n<author>").Append(Author)
                .Append("</author>\r\n");
            foreach (string Category in Categories)
            {
                ItemString.Append("<category>").Append(Category).Append("</category>\r\n");
            }
            ItemString.Append("<pubDate>").Append(PubDate.ToString("r", CultureInfo.InvariantCulture)).Append("</pubDate>\r\n");
            if (Enclosure != null)
                ItemString.Append(Enclosure.ToString());
            if (Thumbnail != null)
                ItemString.Append(Thumbnail.ToString());
            ItemString.Append("<description><![CDATA[").Append(Description).Append("]]></description>\r\n");
            if (GUID != null)
                ItemString.Append(GUID.ToString());
            ItemString.Append("<itunes:subtitle>").Append(Title).Append("</itunes:subtitle>");
            ItemString.Append("<itunes:summary><![CDATA[").Append(Description).Append("]]></itunes:summary>");
            ItemString.Append("</item>\r\n");
            return ItemString.ToString();
        }
    }
}