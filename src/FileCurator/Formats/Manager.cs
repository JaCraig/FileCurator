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
using FileCurator.Formats.Interfaces;
using FileCurator.Formats.Txt;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FileCurator.Formats
{
    /// <summary>
    /// Format manager
    /// </summary>
    public class Manager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Manager"/> class.
        /// </summary>
        /// <param name="formats">The formats.</param>
        public Manager(IEnumerable<IFormat> formats)
        {
            FormatsByFileType = new ConcurrentDictionary<string, IFormat>();
            FormatsByMimeType = new ConcurrentDictionary<string, IFormat>();
            var LocalFormats = formats.Where(x => x.GetType().GetTypeInfo().Assembly.FullName.ToUpperInvariant().Contains("FILECURATOR"));
            var OtherFormats = formats.Where(x => !x.GetType().GetTypeInfo().Assembly.FullName.ToUpperInvariant().Contains("FILECURATOR"));
            AddFormats(OtherFormats);
            AddFormats(LocalFormats);
            DefaultFormat = new TxtFormat();
            Formats = FormatsByFileType.Values.Distinct();
        }

        /// <summary>
        /// Gets or sets the default format.
        /// </summary>
        /// <value>The default format.</value>
        public IFormat DefaultFormat { get; set; }

        /// <summary>
        /// Gets or sets the formats.
        /// </summary>
        /// <value>The formats.</value>
        public IEnumerable<IFormat> Formats { get; set; }

        /// <summary>
        /// Gets or sets the formats.
        /// </summary>
        /// <value>The formats.</value>
        public IDictionary<string, IFormat> FormatsByFileType { get; set; }

        /// <summary>
        /// Gets or sets the formats.
        /// </summary>
        /// <value>The formats.</value>
        public IDictionary<string, IFormat> FormatsByMimeType { get; set; }

        /// <summary>
        /// Finds the format.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>The format associated with the file.</returns>
        public IFormat FindFormat(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return DefaultFormat;
            fileName = fileName.ToUpperInvariant();
            var Extension = new FileInfo(fileName).Extension.Replace(".", "");
            if (FormatsByFileType.ContainsKey(Extension))
                return FormatsByFileType[Extension];
            using (var TempStream = File.OpenRead(fileName))
            {
                return FindFormat(TempStream, "");
            }
        }

        /// <summary>
        /// Finds the format.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>The format associated with the stream.</returns>
        public IFormat FindFormat(Stream stream, string mimeType)
        {
            mimeType = mimeType.ToUpperInvariant();
            if (!string.IsNullOrEmpty(mimeType))
            {
                var Key = FormatsByMimeType.Keys.FirstOrDefault(mimeType.Contains) ?? mimeType;
                if (FormatsByMimeType.ContainsKey(Key))
                    return FormatsByMimeType[Key];
            }
            return Formats.FirstOrDefault(x => x.CanRead(stream)) ?? DefaultFormat;
        }

        /// <summary>
        /// Adds the formats.
        /// </summary>
        /// <param name="formats">The formats.</param>
        private void AddFormats(IEnumerable<IFormat> formats)
        {
            foreach (var Format in formats)
            {
                foreach (var FileType in Format.FileTypes)
                {
                    if (!FormatsByFileType.ContainsKey(FileType))
                        FormatsByFileType.Add(FileType, Format);
                }
                foreach (var ContentType in Format.ContentTypes)
                {
                    if (!FormatsByMimeType.ContainsKey(ContentType))
                        FormatsByMimeType.Add(ContentType, Format);
                }
            }
        }
    }
}