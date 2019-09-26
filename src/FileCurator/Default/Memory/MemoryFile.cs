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

using FileCurator.BaseClasses;
using FileCurator.HelperMethods;
using FileCurator.Interfaces;
using System;
using System.IO;
using System.Text;

namespace FileCurator.Default.Memory
{
    /// <summary>
    /// Memory file
    /// </summary>
    /// <seealso cref="FileBase{String, MemoryFile}"/>
    public class MemoryFile : FileBase<string, MemoryFile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryFile"/> class.
        /// </summary>
        public MemoryFile()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MemoryFile"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="credentials">The credentials.</param>
        public MemoryFile(string path, Credentials credentials)
            : base(path, credentials)
        {
            created = modified = accessed = DateTime.UtcNow;
            fileData = Array.Empty<byte>();
        }

        /// <summary>
        /// The created
        /// </summary>
        private readonly DateTime created;

        /// <summary>
        /// The accessed
        /// </summary>
        private DateTime accessed;

        /// <summary>
        /// The file data
        /// </summary>
        private byte[] fileData;

        /// <summary>
        /// The modified
        /// </summary>
        private DateTime modified;

        /// <summary>
        /// Last time accessed (UTC time)
        /// </summary>
        public override DateTime Accessed => accessed;

        /// <summary>
        /// Time created (UTC time)
        /// </summary>
        public override DateTime Created => created;

        /// <summary>
        /// Directory the file is within
        /// </summary>
        public override IDirectory Directory => InternalFile == null ? null : new MemoryDirectory((string)InternalFile.Left(InternalFile.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1), Credentials);

        /// <summary>
        /// Does the file exist?
        /// </summary>
        public override bool Exists => InternalFile != null;

        /// <summary>
        /// File extension
        /// </summary>
        public override string Extension => InternalFile.Right(InternalFile.Length - InternalFile.LastIndexOf('.'));

        /// <summary>
        /// Full path
        /// </summary>
        public override string FullName => InternalFile;

        /// <summary>
        /// Size of the file
        /// </summary>
        public override long Length => fileData.LongLength;

        /// <summary>
        /// Time modified (UTC time)
        /// </summary>
        public override DateTime Modified => modified;

        /// <summary>
        /// Name of the file
        /// </summary>
        public override string Name => InternalFile;

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        public override IFile CopyTo(IDirectory directory, bool overwrite)
        {
            if (directory == null || !Exists)
                return this;
            var File = new FileInfo(directory.FullName + "/" + Name.Right(Name.Length - (Name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
            if (!File.Exists || overwrite)
            {
                File.Write(ReadBinary());
                return File;
            }
            return this;
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>Any response for deleting the resource (usually FTP, HTTP, etc)</returns>
        public override string Delete()
        {
            fileData = Array.Empty<byte>();
            modified = DateTime.UtcNow;
            return "";
        }

        /// <summary>
        /// Moves the file to a new directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        /// <returns>The resulting file.</returns>
        public override IFile MoveTo(IDirectory directory)
        {
            if (directory == null || !Exists)
                return this;
            var TempFile = new FileInfo(directory.FullName + "/" + Name.Right(Name.Length - (Name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
            TempFile.Write(ReadBinary());
            Delete();
            return TempFile;
        }

        /// <summary>
        /// Reads the file in as a string
        /// </summary>
        /// <returns>The file contents as a string</returns>
        public override string Read()
        {
            if (fileData == null)
                return "";
            return fileData.ToString(Encoding.UTF8);
        }

        /// <summary>
        /// Reads a file as binary
        /// </summary>
        /// <returns>The file contents as a byte array</returns>
        public override byte[] ReadBinary()
        {
            if (fileData == null)
                return Array.Empty<byte>();
            accessed = DateTime.UtcNow;
            return (byte[])fileData.Clone();
        }

        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="newName">New name for the file</param>
        /// <returns>Renames the file.</returns>
        public override IFile Rename(string newName)
        {
            if (string.IsNullOrEmpty(newName) || !Exists)
                return this;
            modified = DateTime.UtcNow;
            InternalFile = Directory + "/" + newName;
            return this;
        }

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">Mode to open the file as</param>
        /// <param name="encoding">Encoding to use for the content</param>
        /// <returns>The result of the write or original content</returns>
        public override string Write(string content, FileMode mode = FileMode.Create, Encoding encoding = null)
        {
            if (content == null)
                content = "";
            if (encoding == null)
                encoding = Encoding.UTF8;
            return Write(encoding.GetBytes(content), mode).ToString(encoding);
        }

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">Mode to open the file as</param>
        /// <returns>The result of the write or original content</returns>
        public override byte[] Write(byte[] content, FileMode mode = FileMode.Create)
        {
            if (content == null)
                content = Array.Empty<byte>();
            Directory.Create();
            modified = DateTime.UtcNow;
            if (mode == FileMode.Append)
            {
                var Result = new byte[fileData.Length + content.Length];
                Array.Copy(fileData, Result, fileData.Length);
                Array.Copy(content, 0, Result, fileData.Length, content.Length);
                fileData = Result;
            }
            else
            {
                fileData = content;
            }
            return content;
        }
    }
}