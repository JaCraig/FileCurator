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
using System.IO;
using System.Threading.Tasks;

namespace FileCurator.Formats.Interfaces
{
    /// <summary>
    /// Format interface
    /// </summary>
    public interface IFormat
    {
        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <value>The content types.</value>
        string[] ContentTypes { get; }

        /// <summary>
        /// Gets the display name.
        /// </summary>
        /// <value>The display name.</value>
        string DisplayName { get; }

        /// <summary>
        /// Gets the file types.
        /// </summary>
        /// <value>The file types.</value>
        string[] FileTypes { get; }

        /// <summary>
        /// Gets the header information.
        /// </summary>
        /// <value>The header information.</value>
        byte[] HeaderInfo { get; }

        /// <summary>
        /// Determines whether this instance can decode the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True if it can, false otherwise</returns>
        bool CanRead(string fileName);

        /// <summary>
        /// Determines whether this instance can decode the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>True if it can, false otherwise</returns>
        bool CanRead(Stream stream);

        /// <summary>
        /// Determines whether this instance can encode the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True if it can, false otherwise</returns>
        bool CanWrite(string fileName);

        /// <summary>
        /// Reads the base.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns an IGenericFile version of the doc.</returns>
        IGenericFile ReadBase(Stream stream);

        /// <summary>
        /// Reads the base.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>Returns an IGenericFile version of the doc.</returns>
        Task<IGenericFile> ReadBaseAsync(Stream stream);

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        bool Write(Stream writer, IGenericFile file);

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        Task<bool> WriteAsync(Stream writer, IGenericFile file);
    }

    /// <summary>
    /// Format interface
    /// </summary>
    /// <typeparam name="TFile">The type of the file.</typeparam>
    public interface IFormat<TFile> : IFormat
        where TFile : IGenericFile
    {
        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The resulting file content.</returns>
        TFile Read(Stream stream);

        /// <summary>
        /// Parses the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The resulting file content.</returns>
        Task<TFile> ReadAsync(Stream stream);
    }
}