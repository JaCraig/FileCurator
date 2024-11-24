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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace FileCurator.Formats.RSS.Data
{
    /// <summary>
    /// Channel
    /// </summary>
    /// <seealso cref="IChannel"/>
    public class Channel : IChannel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Channel"/> class.
        /// </summary>
        public Channel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="doc">XML representation of the channel</param>
        public Channel(IXPathNavigable doc)
        {
            var Element = doc.CreateNavigator();
            if (!Element.Name.Equals("channel", StringComparison.CurrentCultureIgnoreCase))
                throw new ArgumentException("Element is not a channel");
            var NamespaceManager = new XmlNamespaceManager(Element.NameTable);
            Title = Element.SelectSingleNode("./title", NamespaceManager)?.Value ?? "";
            Link = Element.SelectSingleNode("./link", NamespaceManager)?.Value ?? "";
            Description = Element.SelectSingleNode("./description", NamespaceManager)?.Value ?? "";
            Copyright = Element.SelectSingleNode("./copyright", NamespaceManager)?.Value ?? "";
            Language = Element.SelectSingleNode("./language", NamespaceManager)?.Value ?? "";
            WebMaster = Element.SelectSingleNode("./webmaster", NamespaceManager)?.Value ?? "";
            if (DateTime.TryParse(Element.SelectSingleNode("./pubDate", NamespaceManager)?.Value.Replace("PDT", "-0700"), out var TempDate))
                PubDate = TempDate;
            else
                PubDate = DateTime.Now;
            var Nodes = Element.Select("./category", NamespaceManager);
            foreach (XPathNavigator TempNode in Nodes)
            {
                Categories.Add(Utils.StripIllegalCharacters(TempNode.Value));
            }
            Docs = Element.SelectSingleNode("./docs", NamespaceManager)?.Value ?? "";
            if (int.TryParse(Element.SelectSingleNode("./ttl", NamespaceManager)?.Value, out var TempTTL))
                TTL = TempTTL;
            ImageUrl = Element.SelectSingleNode("./image/url", NamespaceManager)?.Value ?? "";
            Nodes = Element.Select("./item", NamespaceManager);
            foreach (XPathNavigator TempNode in Nodes)
            {
                Items.Add(new FeedItem(TempNode));
            }
        }

        /// <summary>
        /// Gets the categories.
        /// </summary>
        /// <value>The categories.</value>
        public IList<string> Categories { get; } = new List<string>();

        /// <summary>
        /// Gets the cloud.
        /// </summary>
        /// <value>The cloud.</value>
        public string? Cloud { get; set; }

        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content => Items.ToString(x => x.Content, "\n");

        /// <summary>
        /// Gets the copyright.
        /// </summary>
        /// <value>The copyright.</value>
        public string Copyright { get; set; } = "Copyright " + DateTime.Now.ToString("yyyy", CultureInfo.InvariantCulture) + ". All rights reserved.";

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public int Count => Items.Count;

        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>The description.</value>
        public string? Description { get; set; }

        /// <summary>
        /// Gets the docs.
        /// </summary>
        /// <value>The docs.</value>
        public string Docs { get; set; } = "http://blogs.law.harvard.edu/tech/rss";

        /// <summary>
        /// Gets or sets a value indicating whether this <see
        /// cref="T:FileCurator.Formats.Data.Interfaces.IChannel"/> is explicit.
        /// </summary>
        /// <value><c>true</c> if explicit; otherwise, <c>false</c>.</value>
        public bool Explicit { get; set; }

        /// <summary>
        /// Gets or sets the image URL.
        /// </summary>
        /// <value>The image URL.</value>
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        public bool IsReadOnly => Items.IsReadOnly;

        /// <summary>
        /// Gets the items.
        /// </summary>
        /// <value>The items.</value>
        public IList<IFeedItem> Items { get; } = new List<IFeedItem>();

        /// <summary>
        /// Gets or sets the language.
        /// </summary>
        /// <value>The language.</value>
        public string Language { get; set; } = "en-us";

        /// <summary>
        /// Gets or sets the link.
        /// </summary>
        /// <value>The link.</value>
        public string? Link { get; set; }

        /// <summary>
        /// Gets or sets the pub date.
        /// </summary>
        /// <value>The pub date.</value>
        public DateTime PubDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the TTL.
        /// </summary>
        /// <value>The TTL.</value>
        public int TTL { get; set; } = 5;

        /// <summary>
        /// Gets or sets the web master.
        /// </summary>
        /// <value>The web master.</value>
        public string? WebMaster { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IFeedItem"/> at the specified index.
        /// </summary>
        /// <value>The <see cref="IFeedItem"/>.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public IFeedItem this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public void Add(IFeedItem item) => Items.Add(item);

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void Clear() => Items.Clear();

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains
        /// a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(IFeedItem item) => Items.Contains(item);

        /// <summary>
        /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to
        /// an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements
        /// copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see
        /// cref="T:System.Array"/> must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        public void CopyTo(IFeedItem[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IFeedItem> GetEnumerator() => Items.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate
        /// through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Determines the index of a specific item in the <see cref="T:System.Collections.Generic.IList`1"/>.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        /// <returns>The index of <paramref name="item"/> if found in the list; otherwise, -1.</returns>
        public int IndexOf(IFeedItem item) => Items.IndexOf(item);

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the
        /// specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which <paramref name="item"/> should be inserted.
        /// </param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public void Insert(int index, IFeedItem item) => Items.Insert(index, item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also
        /// returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public bool Remove(IFeedItem item) => Items.Remove(item);

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index) => Items.RemoveAt(index);

        /// <summary>
        /// Converts the channel to a string
        /// </summary>
        /// <returns>The channel as a string</returns>
        public override string ToString()
        {
            var ChannelString = new StringBuilder();
            ChannelString.Append("<channel>");
            ChannelString.Append("<title>").Append(Utils.StripIllegalCharacters(Title ?? "")).Append("</title>\r\n");
            ChannelString.Append("<link>").Append(Link).Append("</link>\r\n");
            ChannelString.Append("<atom:link xmlns:atom=\"http://www.w3.org/2005/Atom\" rel=\"self\" href=\"").Append(Link).Append("\" type=\"application/rss+xml\" />");

            ChannelString.Append("<description><![CDATA[").Append(Utils.StripIllegalCharacters(Description ?? "")).Append("]]></description>\r\n");
            ChannelString.Append("<language>").Append(Language).Append("</language>\r\n");
            ChannelString.Append("<copyright>").Append(Utils.StripIllegalCharacters(Copyright)).Append("</copyright>\r\n");
            ChannelString.Append("<webMaster>").Append(Utils.StripIllegalCharacters(WebMaster ?? "")).Append("</webMaster>\r\n");
            ChannelString.Append("<pubDate>").Append(PubDate.ToString("Ddd, dd MMM yyyy HH':'mm':'ss", CultureInfo.InvariantCulture)).Append("</pubDate>\r\n");
            ChannelString.Append("<itunes:explicit>").Append(Explicit ? "yes" : "no").Append("</itunes:explicit>");
            ChannelString.Append("<itunes:subtitle>").Append(Utils.StripIllegalCharacters(Title ?? "")).Append("</itunes:subtitle>");
            ChannelString.Append("<itunes:summary><![CDATA[").Append(Utils.StripIllegalCharacters(Description ?? "")).Append("]]></itunes:summary>");

            foreach (var Category in Categories)
            {
                ChannelString.Append("<category>").Append(Utils.StripIllegalCharacters(Category)).Append("</category>\r\n");
                ChannelString.Append("<itunes:category text=\"").Append(Category).Append("\" />\r\n");
            }
            ChannelString.Append("<docs>").Append(Docs).Append("</docs>\r\n");
            ChannelString.Append("<ttl>").Append(TTL.ToString(CultureInfo.InvariantCulture)).Append("</ttl>\r\n");
            if (!string.IsNullOrEmpty(ImageUrl))
            {
                ChannelString.Append("<image><url>").Append(ImageUrl).Append("</url>\r\n<title>").Append(Utils.StripIllegalCharacters(Title ?? "")).Append("</title>\r\n<link>").Append(Link).Append("</link>\r\n</image>\r\n");
            }
            foreach (FeedItem CurrentItem in Items)
            {
                ChannelString.Append(CurrentItem.ToString());
            }
            ChannelString.Append("</channel>\r\n");
            return ChannelString.ToString();
        }
    }
}