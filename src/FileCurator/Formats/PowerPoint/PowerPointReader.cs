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
using DocumentFormat.OpenXml.Presentation;
using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FileCurator.Formats.PowerPoint
{
    /// <summary>
    /// Powerpoint file reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{IGenericFile}"/>
    public class PowerPointReader : ReaderBaseClass<IGenericFile>
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
                PresentationDocument.Open(stream, false);
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
            var PowerPointDoc = PresentationDocument.Open(stream, false);
            return new GenericFile(PowerPointDoc.PresentationPart
                                .SlideParts
                                .ToString(x => x.Slide
                                                .CommonSlideData
                                                .ShapeTree
                                                .ChildElements
                                                .ToString(y => y.InnerText, " "),
                                          "\n"),
                                    GetTitle(PowerPointDoc),
                                    GetMetaData(PowerPointDoc));
        }

        /// <summary>
        /// Gets the slide title.
        /// </summary>
        /// <param name="slidePart">The slide part.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">presentationDocument</exception>
        private static string GetSlideTitle(SlidePart slidePart)
        {
            if (slidePart == null)
            {
                throw new ArgumentNullException(nameof(slidePart));
            }

            // Declare a paragraph separator.
            string paragraphSeparator = null;

            if (slidePart.Slide != null)
            {
                // Find all the title shapes.
                var shapes = slidePart.Slide.Descendants<Shape>().Where(IsTitleShape);

                StringBuilder paragraphText = new StringBuilder();

                foreach (var shape in shapes)
                {
                    // Get the text in each paragraph in this shape.
                    foreach (var paragraph in shape.TextBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                    {
                        // Add a line break.
                        paragraphText.Append(paragraphSeparator);

                        foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                        {
                            paragraphText.Append(text.Text);
                        }

                        paragraphSeparator = "\n";
                    }
                }

                return paragraphText.ToString();
            }

            return string.Empty;
        }

        /// <summary>
        /// Determines whether [is title shape] [the specified shape].
        /// </summary>
        /// <param name="shape">The shape.</param>
        /// <returns><c>true</c> if [is title shape] [the specified shape]; otherwise, <c>false</c>.</returns>
        private static bool IsTitleShape(Shape shape)
        {
            var placeholderShape = shape.NonVisualShapeProperties.ApplicationNonVisualDrawingProperties.GetFirstChild<PlaceholderShape>();
            if (placeholderShape?.Type?.HasValue == true)
            {
                switch ((PlaceholderValues)placeholderShape.Type)
                {
                    // Any title shape.
                    case PlaceholderValues.Title:

                    // A centered title.
                    case PlaceholderValues.CenteredTitle:
                        return true;

                    default:
                        return false;
                }
            }
            return false;
        }

        /// <summary>
        /// Gets the meta data.
        /// </summary>
        /// <param name="powerPointDoc">The power point document.</param>
        /// <returns></returns>
        private string GetMetaData(PresentationDocument powerPointDoc)
        {
            return "";
        }

        /// <summary>
        /// Gets the title.
        /// </summary>
        /// <param name="powerPointDoc">The power point document.</param>
        /// <returns>The title</returns>
        private string GetTitle(PresentationDocument powerPointDoc)
        {
            var presentationPart = powerPointDoc.PresentationPart;
            if (presentationPart?.Presentation != null)
            {
                // Get a Presentation object from the PresentationPart object.
                Presentation presentation = presentationPart.Presentation;

                if (presentation.SlideIdList != null)
                {
                    List<string> titlesList = new List<string>();

                    // Get the title of each slide in the slide order.
                    foreach (var slideId in presentation.SlideIdList.Elements<SlideId>())
                    {
                        SlidePart slidePart = presentationPart.GetPartById(slideId.RelationshipId) as SlidePart;

                        // Get the slide title.
                        string title = GetSlideTitle(slidePart);
                        if (!string.IsNullOrEmpty(title))
                            return title;

                        // An empty title can also be added.
                        titlesList.Add(title);
                    }
                }
            }
            return "";
        }
    }
}