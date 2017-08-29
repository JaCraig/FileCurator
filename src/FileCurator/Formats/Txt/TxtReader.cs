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
using System.IO;

namespace FileCurator.Formats.Txt
{
    /// <summary>
    /// TXT file reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{IGenericFile}"/>
    public class TxtReader : ReaderBaseClass<IGenericFile>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier => new byte[0];

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IGenericFile Read(Stream stream)
        {
            return new GenericFile(stream.ReadAll(), "", "");
        }
    }
}