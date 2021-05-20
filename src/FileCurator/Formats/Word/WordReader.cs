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
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using System.IO;

namespace FileCurator.Formats.Word
{
    /// <summary>
    /// Word reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{IGenericFile}"/>
    public class WordReader : ReaderBaseClass<IGenericFile>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier { get; } = new byte[] { 0x50, 0x4B, 0x03, 0x04 };

        /// <summary>
        /// Used to determine if a reader can actually read the file
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>True if it can, false otherwise</returns>
        public override bool InternalCanRead(Stream stream)
        {
            try
            {
                WordprocessingDocument.Open(stream, false);
            }
            catch { return false; }
            return true;
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IGenericFile Read(Stream stream)
        {
            if (stream is null)
                return new GenericFile("", "", "");
            try
            {
                using var Doc = WordprocessingDocument.Open(stream, false);
                return new GenericFile(Doc.MainDocumentPart.Document.Body.Descendants<Paragraph>().ToString(x => x.InnerText, "\n"),
    Doc.PackageProperties.Title,
    Doc.PackageProperties.Subject);
            }
            catch
            {
                return new GenericFile("", "", "");
            }
        }
    }
}