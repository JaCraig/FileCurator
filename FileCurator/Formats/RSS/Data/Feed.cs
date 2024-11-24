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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace FileCurator.Formats.RSS.Data
{
    /// <summary>
    /// Individual RSS Feed
    /// </summary>
    /// <seealso cref="IFeed"/>
    public class Feed : IFeed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Feed"/> class.
        /// </summary>
        public Feed()
        {
            Channels = new List<IChannel>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Feed"/> class.
        /// </summary>
        /// <param name="content">The content.</param>
        public Feed(string content)
            : this()
        {
            LoadFromData(content);
        }

        /// <summary>
        /// Gets the channels.
        /// </summary>
        /// <value>The channels.</value>
        public IList<IChannel> Channels { get; }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>The content.</value>
        public string Content => Channels.ToString(x => x.Content, "\n");

        /// <summary>
        /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public int Count => Channels.Count;

        /// <summary>
        /// Gets a value indicating whether the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
        /// </summary>
        public bool IsReadOnly => Channels.IsReadOnly;

        /// <summary>
        /// Gets or sets the meta.
        /// </summary>
        /// <value>The meta.</value>
        public string Meta => Channels.FirstOrDefault()?.Description ?? string.Empty;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        public string Title => Channels.FirstOrDefault()?.Title ?? string.Empty;

        /// <summary>
        /// Gets or sets the <see cref="IChannel"/> at the specified index.
        /// </summary>
        /// <value>The <see cref="IChannel"/>.</value>
        /// <param name="index">The index.</param>
        /// <returns></returns>
        public IChannel this[int index]
        {
            get => Channels[index];
            set => Channels[index] = value;
        }

        /// <summary>
        /// Adds an item to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        public void Add(IChannel item) => Channels.Add(item);

        /// <summary>
        /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        public void Clear() => Channels.Clear();

        /// <summary>
        /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains
        /// a specific value.
        /// </summary>
        /// <param name="item">The object to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> is found in the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
        /// </returns>
        public bool Contains(IChannel item) => Channels.Contains(item);

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
        public void CopyTo(IChannel[] array, int arrayIndex) => Channels.CopyTo(array, arrayIndex);

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<IChannel> GetEnumerator() => Channels.GetEnumerator();

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
        public int IndexOf(IChannel item) => Channels.IndexOf(item);

        /// <summary>
        /// Inserts an item to the <see cref="T:System.Collections.Generic.IList`1"/> at the
        /// specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which <paramref name="item"/> should be inserted.
        /// </param>
        /// <param name="item">The object to insert into the <see cref="T:System.Collections.Generic.IList`1"/>.</param>
        public void Insert(int index, IChannel item) => Channels.Insert(index, item);

        /// <summary>
        /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </summary>
        /// <param name="item">The object to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
        /// <returns>
        /// true if <paramref name="item"/> was successfully removed from the <see
        /// cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also
        /// returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
        /// </returns>
        public bool Remove(IChannel item) => Channels.Remove(item);

        /// <summary>
        /// Removes the <see cref="T:System.Collections.Generic.IList`1"/> item at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the item to remove.</param>
        public void RemoveAt(int index) => Channels.RemoveAt(index);

        /// <summary>
        /// string representation of the RSS feed.
        /// </summary>
        /// <returns>An rss formatted string</returns>
        public override string ToString()
        {
            return new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<rss xmlns:itunes=\"http://www.itunes.com/dtds/podcast-1.0.dtd\" xmlns:media=\"http://search.yahoo.com/mrss/\" version=\"2.0\">\r\n")
                .Append(Channels.ToString(x => x.ToString(), ""))
                .Append("</rss>")
                .ToString();
        }

        /// <summary>
        /// Loads the specified document.
        /// </summary>
        /// <param name="Document">The document.</param>
        private void Load(IXPathNavigable Document)
        {
            if (Document is null)
                return;
            var Navigator = Document.CreateNavigator();
            var NamespaceManager = new XmlNamespaceManager(Navigator.NameTable);
            var Nodes = Navigator.Select("./channel", NamespaceManager);
            foreach (XPathNavigator Element in Nodes)
            {
                Channels.Add(new Channel(Element));
            }
            if (Channels.Count == 0)
            {
                Nodes = Navigator.Select(".//channel", NamespaceManager);
                foreach (XPathNavigator Element in Nodes)
                {
                    Channels.Add(new Channel(Element));
                }
                if (Channels.Count > 0 && Channels.FirstOrDefault()?.Count == 0)
                {
                    var Items = new List<FeedItem>();
                    Nodes = Navigator.Select(".//item", NamespaceManager);
                    foreach (XPathNavigator Element in Nodes)
                    {
                        Items.Add(new FeedItem(Element));
                    }
                    Channels.FirstOrDefault()?.Add(Items);
                }
            }
        }

        /// <summary>
        /// Loads the object from the data specified
        /// </summary>
        /// <param name="data">Data to load into the object</param>
        private void LoadFromData(string data)
        {
            if (string.IsNullOrEmpty(data))
                return;
            var Document = new XmlDocument();
            Document.LoadXml(data);
            Load(Document.CreateNavigator());
        }
    }
}