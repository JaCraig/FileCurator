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

using BigBook;
using FileCurator.Formats;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using FileCurator.Interfaces;
using System;
using System.IO;
using System.Text;

namespace FileCurator
{
    /// <summary>
    /// File info class
    /// </summary>
    public class FileInfo : IFile
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="credentials">The credentials.</param>
        public FileInfo(string path, Credentials credentials = null)
            : this(Canister.Builder.Bootstrapper.Resolve<FileCurator>().File(path, credentials))
        {
            Credentials = credentials;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="internalFile">Internal file</param>
        public FileInfo(IFile internalFile)
        {
            InternalFile = internalFile;
        }

        /// <summary>
        /// Last time accessed (UTC time)
        /// </summary>
        public DateTime Accessed => InternalFile?.Accessed ?? DateTime.Now;

        /// <summary>
        /// Time created (UTC time)
        /// </summary>
        public DateTime Created => InternalFile?.Created ?? DateTime.Now;

        /// <summary>
        /// Directory the file is within
        /// </summary>
        public IDirectory Directory => InternalFile?.Directory;

        /// <summary>
        /// Does the file exist?
        /// </summary>
        public bool Exists => InternalFile?.Exists ?? false;

        /// <summary>
        /// File extension
        /// </summary>
        public string Extension => InternalFile?.Extension ?? "";

        /// <summary>
        /// Full path
        /// </summary>
        public string FullName => InternalFile?.FullName ?? "";

        /// <summary>
        /// Size of the file
        /// </summary>
        public long Length => InternalFile?.Length ?? 0;

        /// <summary>
        /// Time modified (UTC time)
        /// </summary>
        public DateTime Modified => InternalFile?.Modified ?? DateTime.Now;

        /// <summary>
        /// Name of the file
        /// </summary>
        public string Name => InternalFile?.Name ?? "";

        /// <summary>
        /// Internal directory
        /// </summary>
        protected IFile InternalFile { get; }

        /// <summary>
        /// Gets or sets the credentials.
        /// </summary>
        /// <value>The credentials.</value>
        private Credentials Credentials { get; }

        /// <summary>
        /// Gets or sets the internal manager.
        /// </summary>
        /// <value>The internal manager.</value>
        private Manager FormatManager => Canister.Builder.Bootstrapper.Resolve<Manager>();

        /// <summary>
        /// Reads the file and converts it to a byte array
        /// </summary>
        /// <param name="file">File to read</param>
        /// <returns>The file as a byte array</returns>
        public static implicit operator byte[] (FileInfo file)
        {
            if (file == null)
                return Array.Empty<byte>();
            return file.ReadBinary();
        }

        /// <summary>
        /// Reads the file and converts it to a string
        /// </summary>
        /// <param name="file">File to read</param>
        /// <returns>The file as a string</returns>
        public static implicit operator string(FileInfo file)
        {
            if (file == null)
                return "";
            return file.Read();
        }

        /// <summary>
        /// Determines if two directories are not equal
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>True if they are not equal, false otherwise</returns>
        public static bool operator !=(FileInfo file1, FileInfo file2)
        {
            return !(file1 == file2);
        }

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>The result</returns>
        public static bool operator <(FileInfo file1, FileInfo file2)
        {
            if (file1 == null || file2 == null)
                return false;
            return string.Compare(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase) < 0;
        }

        /// <summary>
        /// Less than or equal
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>The result</returns>
        public static bool operator <=(FileInfo file1, FileInfo file2)
        {
            if (file1 == null || file2 == null)
                return false;
            return string.Compare(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase) <= 0;
        }

        /// <summary>
        /// Determines if two directories are equal
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>True if they are, false otherwise</returns>
        public static bool operator ==(FileInfo file1, FileInfo file2)
        {
            if (file1 is null && file2 is null)
                return true;
            if (file1 is null || file2 is null)
                return false;
            return file1.FullName == file2.FullName;
        }

        /// <summary>
        /// Greater than
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>The result</returns>
        public static bool operator >(FileInfo file1, FileInfo file2)
        {
            if (file1 == null || file2 == null)
                return false;
            return string.Compare(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase) > 0;
        }

        /// <summary>
        /// Greater than or equal
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>The result</returns>
        public static bool operator >=(FileInfo file1, FileInfo file2)
        {
            if (file1 == null || file2 == null)
                return false;
            return string.Compare(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Compares this to another file
        /// </summary>
        /// <param name="other">File to compare to</param>
        /// <returns></returns>
        public int CompareTo(IFile other)
        {
            if (other == null)
                return 1;
            if (InternalFile == null)
                return -1;
            return string.Compare(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares this object to another object
        /// </summary>
        /// <param name="obj">Object to compare it to</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (!(obj is FileInfo Temp))
                return 1;
            return CompareTo(Temp);
        }

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        public IFile CopyTo(IDirectory directory, bool overwrite)
        {
            if (directory == null || !Exists)
                return null;
            return InternalFile.CopyTo(directory, overwrite);
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>Any response for deleting the resource (usually FTP, HTTP, etc)</returns>
        public string Delete()
        {
            if (InternalFile == null)
                return "";
            return InternalFile.Delete();
        }

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            var File = obj as FileInfo;
            return File != null && File == this;
        }

        /// <summary>
        /// Determines if the files are equal
        /// </summary>
        /// <param name="other">Other file</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public bool Equals(IFile other)
        {
            if (other == null)
                return false;
            return other.FullName == FullName;
        }

        /// <summary>
        /// Gets the hash code for the file
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        /// <summary>
        /// Moves the file to a new directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        public IFile MoveTo(IDirectory directory)
        {
            if (InternalFile == null || directory == null)
                return this;
            InternalFile.MoveTo(directory);
            return this;
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <typeparam name="TFile">The type of the file object expected.</typeparam>
        /// <returns>The parsed file</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        public TFile Parse<TFile>()
            where TFile : IGenericFile
        {
            if (!(FormatManager.FindFormat(FullName, Credentials) is IFormat<TFile>Format))
                throw new ArgumentException("Could not find file format that returns the specified object type");
            using (var TempStream = new MemoryStream(ReadBinary()))
            {
                return Format.Read(TempStream);
            }
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns>The parsed file</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        public IGenericFile Parse()
        {
            var Format = FormatManager.FindFormat(FullName, Credentials);
            if (Format == null)
                throw new ArgumentException("Could not find file format that returns the specified object type");
            using (var TempStream = new MemoryStream(ReadBinary()))
            {
                return Format.ReadBase(TempStream);
            }
        }

        /// <summary>
        /// Reads the file in as a string
        /// </summary>
        /// <returns>The file contents as a string</returns>
        public string Read()
        {
            if (InternalFile == null)
                return "";
            return InternalFile.Read();
        }

        /// <summary>
        /// Reads a file as binary
        /// </summary>
        /// <returns>The file contents as a byte array</returns>
        public byte[] ReadBinary()
        {
            if (InternalFile == null)
                return Array.Empty<byte>();
            return InternalFile.ReadBinary();
        }

        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="newName">New name for the file</param>
        public IFile Rename(string newName)
        {
            if (InternalFile == null || string.IsNullOrEmpty(newName))
                return this;
            InternalFile.Rename(newName);
            return this;
        }

        /// <summary>
        /// Returns the name of the file
        /// </summary>
        /// <returns>The name of the file</returns>
        public override string ToString()
        {
            return FullName;
        }

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">Mode to open the file as</param>
        /// <param name="encoding">Encoding to use for the content</param>
        /// <returns>The result of the write or original content</returns>
        public string Write(string content, FileMode mode = FileMode.Create, Encoding encoding = null)
        {
            if (InternalFile == null)
                return content;
            return InternalFile.Write(content, mode, encoding);
        }

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">Mode to open the file as</param>
        /// <returns>The result of the write or original content</returns>
        public byte[] Write(byte[] content, FileMode mode = FileMode.Create)
        {
            if (InternalFile == null)
                return content;
            return InternalFile.Write(content, mode);
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>True if it was written successfully, false otherwise.</returns>
        public bool Write(IGenericFile data, FileMode mode = FileMode.Create)
        {
            var Format = FormatManager.FindFormat(FullName, Credentials);
            using (var TempStream = new MemoryStream())
            {
                bool Success = Format.Write(TempStream, data);
                Write(TempStream.ReadAllBinary(), mode);
                return Success;
            }
        }
    }
}