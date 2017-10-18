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
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.RSS.Data;
using System.IO;

namespace FileCurator.Formats.RSS
{
    /// <summary>
    /// RSS Reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{IFeed}"/>
    public class RSSReader : ReaderBaseClass<IFeed>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier => new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x20, 0x65, 0x6E, 0x63, 0x6F, 0x64, 0x69, 0x6E, 0x67, 0x3D, 0x22, 0x75, 0x74, 0x66, 0x2D, 0x38, 0x22, 0x20, 0x3F, 0x3E, 0x3C, 0x72, 0x73, 0x73 };

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IFeed Read(Stream stream)
        {
            return new Feed(stream.ReadAll());
        }
    }
}