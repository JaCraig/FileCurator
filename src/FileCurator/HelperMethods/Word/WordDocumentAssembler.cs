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
using BigBook.Patterns.BaseClasses;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileCurator.HelperMethods.Word
{
    /// <summary>
    /// Document assembly helper class
    /// </summary>
    /// <seealso cref="SafeDisposableBaseClass"/>
    public class WordDocumentAssembler : SafeDisposableBaseClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WordDocumentAssembler"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <exception cref="System.ArgumentNullException">document</exception>
        private WordDocumentAssembler(WordprocessingDocument document)
        {
            InternalWordDoc = document ?? throw new ArgumentNullException(nameof(document));
        }

        /// <summary>
        /// Gets or sets the internal word document.
        /// </summary>
        /// <value>The internal word document.</value>
        private WordprocessingDocument InternalWordDoc { get; set; }

        /// <summary>
        /// Creates the document at the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The new document assembler object</returns>
        public static WordDocumentAssembler Create(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return new WordDocumentAssembler(WordprocessingDocument.Create(path, WordprocessingDocumentType.Document));
        }

        /// <summary>
        /// Opens the document at the specified path specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>The new document assembler object</returns>
        public static WordDocumentAssembler Open(string path)
        {
            if (string.IsNullOrEmpty(path)) return null;
            return new WordDocumentAssembler(WordprocessingDocument.Open(path, true));
        }

        /// <summary>
        /// Appends a page break to the end of the document.
        /// </summary>
        /// <returns>This</returns>
        public WordDocumentAssembler AppendPageBreak()
        {
            InternalWordDoc.MainDocumentPart
                        .Document
                        .Body
                        .Append(new Paragraph(
                                    new Run(
                                    new Break { Type = BreakValues.Page })));
            return this;
        }

        /// <summary>
        /// Appends a table to the document
        /// </summary>
        /// <param name="Table">The table.</param>
        /// <returns>This</returns>
        public WordDocumentAssembler AppendTable(List<List<string>> Table)
        {
            var TempTable = new DocumentFormat.OpenXml.Wordprocessing.Table();
            foreach (var Row in Table)
            {
                var TempRow = new TableRow();
                var Width = 100f / Row.Count;
                foreach (var Cell in Row)
                {
                    var TempCell = new TableCell
                    {
                        TableCellProperties = new TableCellProperties
                        {
                            TableCellWidth = new TableCellWidth
                            {
                                Width = Width + "%",
                                Type = TableWidthUnitValues.Pct
                            },
                            TableCellBorders = new TableCellBorders
                            {
                                TopBorder = new TopBorder
                                {
                                    Val = new EnumValue<BorderValues>(BorderValues.Thick),
                                    Color = "000000"
                                }
                            }
                        }
                    };
                    TempCell.Append(new Paragraph(new Run(new Text(Cell))));
                    TempRow.Append(TempCell);
                }
                TempTable.Append(TempRow);
            }
            InternalWordDoc.MainDocumentPart.Document.Body.Append(TempTable);
            return this;
        }

        /// <summary>
        /// Combines the documents specified and combines them in this document with a page break
        /// between each one.
        /// </summary>
        /// <param name="docLocations">The document locations.</param>
        /// <returns>This</returns>
        public WordDocumentAssembler CombineDocuments(List<string> docLocations)
        {
            if (docLocations == null || docLocations.Count == 0)
                return this;
            MainDocumentPart mainPart = InternalWordDoc.MainDocumentPart;
            for (int x = 0; x < docLocations.Count; ++x)
            {
                var chunk = mainPart.AddAlternativeFormatImportPart(AlternativeFormatImportPartType.WordprocessingML);
                var altChunkId = mainPart.GetIdOfPart(chunk);
                using (FileStream fileStream = File.Open(docLocations[x], FileMode.Open))
                {
                    chunk.FeedData(fileStream);
                    var altChunk = new AltChunk
                    {
                        Id = altChunkId
                    };
                    AppendPageBreak();
                    mainPart.Document
                        .Body
                        .Append(altChunk);
                }
            }
            return this;
        }

        /// <summary>
        /// Replaces the content/template fields with the content in the replacements dictionary.
        /// </summary>
        /// <typeparam name="T">Data type for the Object Args</typeparam>
        /// <param name="objectArgs">The object arguments</param>
        /// <param name="replacements">The replacements.</param>
        public WordDocumentAssembler ReplaceContent<T>(T objectArgs, Dictionary<string, Func<T, string>> replacements)
        {
            if (replacements == null || replacements.Count == 0)
                return this;
            string docText = null;
            using (StreamReader reader = new StreamReader(InternalWordDoc.MainDocumentPart.GetStream()))
            {
                docText = reader.ReadToEnd();
            }
            foreach (var Key in replacements.Keys)
            {
                docText = docText.Replace(Key, replacements[Key](objectArgs).StripIllegalXML());
            }
            using (StreamWriter sw = new StreamWriter(InternalWordDoc.MainDocumentPart.GetStream(FileMode.Create)))
            {
                sw.Write(docText);
            }

            foreach (var WordHeaderPart in InternalWordDoc.MainDocumentPart.HeaderParts)
            {
                using (StreamReader reader = new StreamReader(WordHeaderPart.GetStream()))
                {
                    docText = reader.ReadToEnd();
                }
                foreach (var Key in replacements.Keys)
                {
                    docText = docText.Replace(Key, replacements[Key](objectArgs));
                }
                using (StreamWriter sw = new StreamWriter(WordHeaderPart.GetStream(FileMode.Create)))
                {
                    sw.Write(docText);
                }
            }
            return this;
        }

        /// <summary>
        /// Function to override in order to dispose objects
        /// </summary>
        /// <param name="Managed">
        /// If true, managed and unmanaged objects should be disposed. Otherwise unmanaged objects only.
        /// </param>
        protected override void Dispose(bool Managed)
        {
            if (InternalWordDoc != null)
            {
                InternalWordDoc.Dispose();
                InternalWordDoc = null;
            }
        }
    }
}