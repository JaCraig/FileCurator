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

namespace FileCurator.Default
{
    /// <summary>
    /// Basic local file class
    /// </summary>
    public class LocalFile : FileBase<System.IO.FileInfo, LocalFile>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LocalFile()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the file</param>
        public LocalFile(string path)
            : base(string.IsNullOrEmpty(path) ? null : new System.IO.FileInfo(path))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File to use</param>
        public LocalFile(System.IO.FileInfo file)
            : base(file)
        {
        }

        /// <summary>
        /// Last time accessed (UTC time)
        /// </summary>
        public override DateTime Accessed => InternalFile?.LastAccessTimeUtc ?? DateTime.Now;

        /// <summary>
        /// Time created (UTC time)
        /// </summary>
        public override DateTime Created => InternalFile?.CreationTimeUtc ?? DateTime.Now;

        /// <summary>
        /// Directory the file is within
        /// </summary>
        public override IDirectory Directory => InternalFile is null ? null : new LocalDirectory(InternalFile.Directory);

        /// <summary>
        /// Does the file exist?
        /// </summary>
        public override bool Exists => InternalFile?.Exists ?? false;

        /// <summary>
        /// File extension
        /// </summary>
        public override string Extension => InternalFile?.Extension ?? string.Empty;

        /// <summary>
        /// Full path
        /// </summary>
        public override string FullName => InternalFile?.FullName ?? string.Empty;

        /// <summary>
        /// Size of the file
        /// </summary>
        public override long Length => Exists ? InternalFile.Length : 0;

        /// <summary>
        /// Time modified (UTC time)
        /// </summary>
        public override DateTime Modified => InternalFile?.LastWriteTimeUtc ?? DateTime.Now;

        /// <summary>
        /// Name of the file
        /// </summary>
        public override string Name => InternalFile?.Name ?? string.Empty;

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        public override IFile CopyTo(IDirectory directory, bool overwrite)
        {
            if (directory is null || !Exists)
                return null;
            directory.Create();
            var File = new FileInfo(directory.FullName + "\\" + Name.Right(Name.Length - (Name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
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
            if (!Exists)
                return string.Empty;
            InternalFile.Delete();
            InternalFile.Refresh();
            return string.Empty;
        }

        /// <summary>
        /// Moves the file to a new directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        public override IFile MoveTo(IDirectory directory)
        {
            if (directory is null || !Exists)
                return this;
            directory.Create();
            InternalFile.MoveTo(directory.FullName + "\\" + Name);
            InternalFile = new System.IO.FileInfo(directory.FullName + "\\" + Name);
            return this;
        }

        /// <summary>
        /// Reads the file in as a string
        /// </summary>
        /// <returns>The file contents as a string</returns>
        public override string Read()
        {
            if (!Exists)
                return string.Empty;
            using var Reader = InternalFile.OpenText();
            return Reader.ReadToEnd();
        }

        /// <summary>
        /// Reads a file as binary
        /// </summary>
        /// <returns>The file contents as a byte array</returns>
        public override byte[] ReadBinary()
        {
            if (!Exists)
                return Array.Empty<byte>();
            using var Reader = InternalFile.OpenRead();
            var Buffer = new byte[Reader.Length];
            Reader.Read(Buffer, 0, Buffer.Length);
            return Buffer;
        }

        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="newName">New name for the file</param>
        public override IFile Rename(string newName)
        {
            if (string.IsNullOrEmpty(newName) || !Exists)
                return this;
            InternalFile.MoveTo(InternalFile.DirectoryName + "\\" + newName);
            InternalFile = new System.IO.FileInfo(InternalFile.DirectoryName + "\\" + newName);
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
            if (content is null)
                content = "";
            if (encoding is null)
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
            if (content is null)
                content = Array.Empty<byte>();
            Directory.Create();
            using (var Writer = InternalFile.Open(mode, FileAccess.Write))
            {
                Writer.Write(content, 0, content.Length);
            }
            InternalFile.Refresh();
            return content;
        }
    }
}