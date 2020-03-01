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
using System.Buffers;
using System.IO;

namespace FileCurator.Formats.BaseClasses
{
    /// <summary>
    /// Reader base class
    /// </summary>
    /// <typeparam name="TFile">The type of the file.</typeparam>
    /// <seealso cref="Interfaces.IGenericFileReader{TFile}"/>
    public abstract class ReaderBaseClass<TFile> : IGenericFileReader<TFile>
        where TFile : IGenericFile
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public abstract byte[] HeaderIdentifier { get; }

        /// <summary>
        /// Determines whether this instance can decode the specified file name.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>True if it can, false otherwise</returns>
        public bool CanRead(string fileName)
        {
            using (var File = System.IO.File.OpenRead(fileName))
            {
                return CanRead(File);
            }
        }

        /// <summary>
        /// Determines whether this instance can decode the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>True if it can, false otherwise</returns>
        public bool CanRead(Stream stream)
        {
            if (HeaderIdentifier.Length == 0)
                return false;
            stream.Seek(0, SeekOrigin.Begin);
#if NET462
            var Buffer = new byte[HeaderIdentifier.Length];
#else
            var Buffer = ArrayPool<byte>.Shared.Rent(HeaderIdentifier.Length);
#endif
            stream.Read(Buffer, 0, Buffer.Length);
            stream.Seek(0, SeekOrigin.Begin);
            for (var x = 0; x < HeaderIdentifier.Length; ++x)
            {
                if (Buffer[x] != HeaderIdentifier[x])
                {
#if !NET462
                    ArrayPool<byte>.Shared.Return(Buffer);
#endif
                    return false;
                }
            }
#if !NET462
            ArrayPool<byte>.Shared.Return(Buffer);
#endif
            return InternalCanRead(stream);
        }

        /// <summary>
        /// Used to determine if a reader can actually read the file
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>True if it can, false otherwise</returns>
        public virtual bool InternalCanRead(Stream stream) => true;

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public abstract TFile Read(Stream stream);
    }
}