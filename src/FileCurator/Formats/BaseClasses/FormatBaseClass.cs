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

using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileCurator.Formats.BaseClasses
{
    /// <summary>
    /// Format base class
    /// </summary>
    /// <typeparam name="TFileReader">The type of the file reader.</typeparam>
    /// <typeparam name="TFileWriter">The type of the file writer.</typeparam>
    /// <typeparam name="TFile">The type of the file.</typeparam>
    /// <seealso cref="IFormat{TFile}"/>
    /// <seealso cref="IFormat"/>
    public abstract class FormatBaseClass<TFileReader, TFileWriter, TFile> : IFormat<TFile>
        where TFileReader : IGenericFileReader<TFile>, new()
        where TFileWriter : IGenericFileWriter, new()
        where TFile : IGenericFile
    {
        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <value>The content types.</value>
        public abstract string[] ContentTypes { get; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public abstract string DisplayName { get; }

        /// <summary>
        /// Gets the file types.
        /// </summary>
        /// <value>The file types.</value>
        public abstract string[] FileTypes { get; }

        /// <summary>
        /// Gets the header information.
        /// </summary>
        /// <value>The header information.</value>
        public byte[] HeaderInfo => Reader.HeaderIdentifier;

        /// <summary>
        /// Gets the reader.
        /// </summary>
        /// <value>The reader.</value>
        protected TFileReader Reader { get; } = new TFileReader();

        /// <summary>
        /// Gets the writer.
        /// </summary>
        /// <value>The writer.</value>
        protected TFileWriter Writer { get; } = new TFileWriter();

        /// <summary>
        /// Determines whether this instance can decode the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True if it can, false otherwise</returns>
        public bool CanRead(string fileName) => !string.IsNullOrEmpty(fileName) && FileTypes.Any(x => fileName.EndsWith(x, System.StringComparison.Ordinal));

        /// <summary>
        /// Determines whether this instance can decode the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>True if it can, false otherwise</returns>
        public bool CanRead(Stream stream) => Reader.CanRead(stream);

        /// <summary>
        /// Determines whether this instance can encode the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True if it can, false otherwise</returns>
        public bool CanWrite(string fileName) => !string.IsNullOrEmpty(fileName) && FileTypes.Any(x => fileName.EndsWith(x, System.StringComparison.Ordinal));

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The resulting file content.</returns>
        public TFile Read(Stream stream) => Reader.Read(stream);

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The resulting file content.</returns>
        public Task<TFile> ReadAsync(Stream stream) => Reader.ReadAsync(stream);

        /// <summary>
        /// Reads the base.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns an IGenericFile version of the doc.</returns>
        public IGenericFile ReadBase(Stream stream) => Read(stream);

        /// <summary>
        /// Reads the base.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns an IGenericFile version of the doc.</returns>
        public async Task<IGenericFile> ReadBaseAsync(Stream stream) => await ReadAsync(stream).ConfigureAwait(false);

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public bool Write(Stream writer, IGenericFile file) => Writer.Write(writer, file);

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public Task<bool> WriteAsync(Stream writer, IGenericFile file) => Writer.WriteAsync(writer, file);
    }
}