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
using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileCurator.Formats.HTML
{
    /// <summary>
    /// HTML reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{IGenericFile}"/>
    public class HTMLReader : ReaderBaseClass<IGenericFile>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier { get; } = Array.Empty<byte>();

        /// <summary>
        /// The strip HTML regex
        /// </summary>
        private static readonly Regex STRIP_HTML_REGEX = new Regex("<[^>]*>", RegexOptions.Compiled);

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IGenericFile Read(Stream stream)
        {
            var Content = stream.ReadAll();
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(Content);
            var Title = doc.DocumentNode.SelectSingleNode("//head//title")?.InnerText ?? "";
            var Meta = new Regex(@"<meta\s*name=[""']description[""']\s*content=[""'](?<Content>[^""']*)[""']\s*/>", RegexOptions.IgnoreCase).Match(Content).Groups["Content"].Value;
            Meta += new Regex(@"<meta\s*name=[""']keywords[""']\s*content=[""'](?<Content>[^""']*)[""']\s*/>", RegexOptions.IgnoreCase).Match(Content).Groups["Content"].Value;
            doc.DocumentNode.SelectSingleNode("//body")
                            .Descendants("style")
                            .ToList()
                            .ForEach(x => x.Remove());

            doc.DocumentNode.SelectSingleNode("//body")
                                .Descendants("script")
                                .ToList()
                                .ForEach(x => x.Remove());
            Content = doc.DocumentNode.SelectSingleNode("//body").InnerHtml;
            Content = StripHTML(Content);
            Content = Content.Replace("&amp;", "&").Replace("&rsquo;", "'");
            var RemoveSpaces = new Regex(@"\s+", RegexOptions.None);
            return new GenericFile(RemoveSpaces.Replace(Content, " ").Trim(), Title, Meta);
        }

        /// <summary>
        /// Strips the HTML.
        /// </summary>
        /// <param name="html">The HTML.</param>
        /// <returns></returns>
        private static string StripHTML(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;
            return STRIP_HTML_REGEX.Replace(html, " ")
                                   .Replace("&nbsp;", " ")
                                   .Replace("&#160;", string.Empty);
        }
    }
}