/*
Copyright 2016 James Craig

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

using FileCurator.Enums;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace FileCurator
{
    /// <summary>
    /// File extensions
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExtensionMethods
    {
        /// <summary>
        /// Gets the internal manager.
        /// </summary>
        /// <value>The internal manager.</value>
        private static Formats.Manager InternalManager => Canister.Builder.Bootstrapper.Resolve<Formats.Manager>();

        /// <summary>
        /// Deletes a list of files
        /// </summary>
        /// <param name="Files">List of files</param>
        public static void Delete(this IEnumerable<IFile> Files)
        {
            if (Files == null)
                return;
            Parallel.ForEach(Files, x => x.Delete());
        }

        /// <summary>
        /// Deletes a list of directories
        /// </summary>
        /// <param name="Directories">Directories to delete</param>
        public static void Delete(this IEnumerable<IDirectory> Directories)
        {
            if (Directories == null)
                return;
            Parallel.ForEach(Directories, x => x.Delete());
        }

        /// <summary>
        /// Reads the specified file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>The file as an TFile object</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        public static IGenericFile Parse(this Stream file, MimeType mimeType)
        {
            var Format = InternalManager.FindFormat(file, mimeType);
            if (Format == null)
                throw new ArgumentException("Could not find file format that returns the specified object type");
            return Format.ReadBase(file);
        }

        /// <summary>
        /// Reads the specified MIME type.
        /// </summary>
        /// <typeparam name="TFile">The type of the file.</typeparam>
        /// <param name="file">The file.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>The file as an TFile object</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        public static TFile Parse<TFile>(this Stream file, MimeType mimeType)
            where TFile : IGenericFile
        {
            if (!(InternalManager.FindFormat(file, mimeType) is IFormat<TFile>Format))
                throw new ArgumentException("Could not find file format that returns the specified object type");
            return Format.Read(file);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="data">The data.</param>
        /// <param name="mimeType">Type of the MIME.</param>
        /// <returns>True if it was written successfully, false otherwise.</returns>
        public static bool Write(this Stream file, IGenericFile data, MimeType mimeType)
        {
            var Format = InternalManager.FindFormat(file, mimeType);
            return Format.Write(file, data);
        }
    }
}