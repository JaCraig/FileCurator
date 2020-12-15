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
        public override byte[] HeaderIdentifier { get; } = new byte[] { 0x3C, 0x3F, 0x78, 0x6D, 0x6C, 0x20, 0x76, 0x65, 0x72, 0x73, 0x69, 0x6F, 0x6E, 0x3D, 0x22, 0x31, 0x2E, 0x30, 0x22, 0x20 };

        /// <summary>
        /// Used to determine if a reader can actually read the file
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>True if it can, false otherwise</returns>
        public override bool InternalCanRead(Stream stream)
        {
            try
            {
                var TempFeed = new Feed(stream.ReadAll());
                if (TempFeed.Channels.Count > 0 && TempFeed.Channels[0].Count > 0)
                    return true;
            }
            catch { }
            return false;
        }

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IFeed Read(Stream stream) => new Feed(stream.ReadAll());
    }
}