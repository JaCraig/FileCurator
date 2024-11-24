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

using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using System.IO;
using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace FileCurator.Formats.PDF
{
    /// <summary>
    /// PDF Reader
    /// </summary>
    /// <seealso cref="ReaderBaseClass{IGenericFile}"/>
    public class PDFReader : ReaderBaseClass<IGenericFile>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier => new byte[] { 0x25, 0x50, 0x44, 0x46, 0x2D, 0x31, 0x2E };

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IGenericFile Read(Stream stream)
        {
            var Builder = new StringBuilder();
            var Title = "";
            var Meta = "";
            try
            {
                using var Pdf = PdfDocument.Open(stream);
                Title = Pdf.Information.Title;
                Meta = Pdf.Information.Keywords;
                foreach (UglyToad.PdfPig.Content.Page? Page in Pdf.GetPages())
                {
                    _ = Builder.Append(ContentOrderTextExtractor.GetText(Page) + "\n");
                }
            }
            catch
            {
            }
            return new GenericFile(Builder.ToString(), Title, Meta);
        }
    }
}