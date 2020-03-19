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
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCurator.Windows.Formats.PDF
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
        /// The number of characters to keep, when extracting text.
        /// </summary>
        private const int NumberOfCharsToKeep = 15;

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IGenericFile Read(Stream stream)
        {
            var Builder = new StringBuilder();
            string Title = "";
            string Meta = "";
            try
            {
                using (PdfDocument inputDocument = PdfReader.Open(stream, PdfDocumentOpenMode.ReadOnly))
                {
                    Title = inputDocument.Info.Title;
                    Meta = inputDocument.Info.Keywords;
                    foreach (PdfPage page in inputDocument.Pages)
                    {
                        for (int index = 0; index < page.Contents.Elements.Count; index++)
                        {
                            PdfDictionary.PdfStream tempStream = page.Contents.Elements.GetDictionary(index).Stream;
                            Builder.Append(ExtractTextFromPDFBytes(tempStream.Value));
                        }
                    }
                }
            }
            catch
            {
            }
            return new GenericFile(Builder.ToString(), Title, Meta);
        }

        /// <summary>
        /// Check if a certain 2 character token just came along (e.g. BT)
        /// </summary>
        /// <param name="tokens">the searched token</param>
        /// <param name="recent">the recent character array</param>
        /// <returns>True if it has, false otherwise</returns>
        private static bool CheckToken(string[] tokens, char[] recent)
        {
            return tokens.Any(token => recent[NumberOfCharsToKeep - 3] == token[0]
                                    && recent[NumberOfCharsToKeep - 2] == token[1]
                                    && (recent[NumberOfCharsToKeep - 1] == ' '
                                        || recent[NumberOfCharsToKeep - 1] == 0x0d
                                        || recent[NumberOfCharsToKeep - 1] == 0x0a)
                                    && (recent[NumberOfCharsToKeep - 4] == ' '
                                        || recent[NumberOfCharsToKeep - 4] == 0x0d
                                        || recent[NumberOfCharsToKeep - 4] == 0x0a)
                             );
        }

        /// <summary>
        /// This method processes an uncompressed Adobe (text) object and extracts text.
        /// </summary>
        /// <param name="input">uncompressed</param>
        /// <returns></returns>
        private static string ExtractTextFromPDFBytes(byte[] input)
        {
            if (input is null || input.Length == 0) return "";
            try
            {
                var resultString = new StringBuilder();
                var inTextObject = false;
                var nextLiteral = false;
                var bracketDepth = 0;
                var previousCharacters = new char[NumberOfCharsToKeep];
                for (var x = 0; x < NumberOfCharsToKeep; ++x)
                    previousCharacters[x] = ' ';

                for (var x = 0; x < input.Length; x++)
                {
                    var c = (char)input[x];

                    if (inTextObject)
                    {
                        // Position the text
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(new string[] { "TD", "Td" }, previousCharacters))
                            {
                                resultString.Append("\n\r");
                            }
                            else
                            {
                                if (CheckToken(new string[] { "'", "T*", "\"" }, previousCharacters))
                                {
                                    resultString.Append("\n");
                                }
                                else
                                {
                                    if (CheckToken(new string[] { "Tj" }, previousCharacters))
                                    {
                                        resultString.Append(" ");
                                    }
                                }
                            }
                        }

                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0
                            && CheckToken(new string[] { "ET" }, previousCharacters))
                        {
                            inTextObject = false;
                            resultString.Append(" ");
                        }
                        else
                        {
                            // Start outputting text
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                            }
                            else
                            {
                                // Stop outputting text
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    // Just a normal text character:
                                    if (bracketDepth == 1)
                                    {
                                        // Only print out next character no matter what. Do not interpret.
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                        }
                                        else
                                        {
                                            if (((c >= ' ') && (c <= '~'))
                                                || ((c >= 128) && (c < 255)))
                                            {
                                                resultString.Append(c);
                                            }

                                            nextLiteral = false;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Store the recent characters for when we have to go back for a checking
                    for (int j = 0; j < NumberOfCharsToKeep - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[NumberOfCharsToKeep - 1] = c;

                    // Start of a text object
                    if (!inTextObject && CheckToken(new string[] { "BT" }, previousCharacters))
                    {
                        inTextObject = true;
                    }
                }
                return resultString.ToString();
            }
            catch
            {
                return "";
            }
        }
    }
}