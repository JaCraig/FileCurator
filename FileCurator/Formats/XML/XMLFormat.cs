﻿/*
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
using FileCurator.Formats.Data.Interfaces;

namespace FileCurator.Formats.XML
{
    /// <summary>
    /// XML Format
    /// </summary>
    /// <seealso cref="BaseClasses.FormatBaseClass{XMLReader, XMLWriter, IGenericFile}"/>
    public class XMLFormat : FormatBaseClass<XMLReader, XMLWriter, IGenericFile>
    {
        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <value>The content types.</value>
        public override string[] ContentTypes { get; } = new[] { "TEXT/XML", "APPLICATION/XML" };

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public override string DisplayName { get; } = "XML";

        /// <summary>
        /// Gets the file types.
        /// </summary>
        /// <value>The file types.</value>
        public override string[] FileTypes { get; } = new[] { "XML" };
    }
}