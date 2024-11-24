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
using System.Threading.Tasks;

namespace FileCurator.BaseClasses
{
    /// <summary>
    /// Directory base class
    /// </summary>
    /// <typeparam name="InternalFileType">Internal file type</typeparam>
    /// <typeparam name="FileType">File type</typeparam>
    public abstract class FileBase<InternalFileType, FileType> : IFile
        where FileType : FileBase<InternalFileType, FileType>, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected FileBase()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="internalFile">Internal file</param>
        protected FileBase(InternalFileType internalFile)
            : this()
        {
            InternalFile = internalFile;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="internalFile">Internal file</param>
        /// <param name="credentials">The credentials.</param>
        protected FileBase(InternalFileType internalFile, Credentials? credentials = null)
            : this(internalFile)
        {
            Credentials = credentials;
        }

        /// <summary>
        /// Last time accessed (UTC time)
        /// </summary>
        public abstract DateTime Accessed { get; }

        /// <summary>
        /// Time created (UTC time)
        /// </summary>
        public abstract DateTime Created { get; }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <value>The credentials.</value>
        public Credentials? Credentials { get; }

        /// <summary>
        /// Directory the file is within
        /// </summary>
        public abstract IDirectory? Directory { get; }

        /// <summary>
        /// Does the file exist?
        /// </summary>
        public abstract bool Exists { get; }

        /// <summary>
        /// File extension
        /// </summary>
        public abstract string Extension { get; }

        /// <summary>
        /// Full path
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Size of the file
        /// </summary>
        public abstract long Length { get; }

        /// <summary>
        /// Time modified (UTC time)
        /// </summary>
        public abstract DateTime Modified { get; }

        /// <summary>
        /// Name of the file
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets or sets the internal manager.
        /// </summary>
        /// <value>The internal manager.</value>
        protected Manager? FormatManager => Manager.Instance;

        /// <summary>
        /// Internal directory
        /// </summary>
        protected InternalFileType InternalFile { get; set; }

        /// <summary>
        /// Reads the file and converts it to a byte array
        /// </summary>
        /// <param name="file">File to read</param>
        /// <returns>The file as a byte array</returns>
        public static implicit operator byte[](FileBase<InternalFileType, FileType> file)
        {
            return file?.ReadBinary() ?? Array.Empty<byte>();
        }

        /// <summary>
        /// Reads the file and converts it to a string
        /// </summary>
        /// <param name="file">File to read</param>
        /// <returns>The file as a string</returns>
        public static implicit operator string(FileBase<InternalFileType, FileType> file)
        {
            return file?.Read() ?? string.Empty;
        }

        /// <summary>
        /// Determines if two directories are not equal
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>True if they are not equal, false otherwise</returns>
        public static bool operator !=(FileBase<InternalFileType, FileType> file1, IFile file2)
        {
            return !(file1 == file2);
        }

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>The result</returns>
        public static bool operator <(FileBase<InternalFileType, FileType> file1, IFile file2)
        {
            return !(file1 is null)
                && !(file2 is null)
                && string.Compare(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase) < 0;
        }

        /// <summary>
        /// Less than or equal
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>The result</returns>
        public static bool operator <=(FileBase<InternalFileType, FileType> file1, IFile file2)
        {
            return !(file1 is null)
                && !(file2 is null)
                && string.Compare(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase) <= 0;
        }

        /// <summary>
        /// Determines if two directories are equal
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>True if they are, false otherwise</returns>
        public static bool operator ==(FileBase<InternalFileType, FileType> file1, IFile file2)
        {
            return (file1 is null && file2 is null)
                || (!(file1 is null) && !(file2 is null) && file1.FullName == file2.FullName);
        }

        /// <summary>
        /// Greater than
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>The result</returns>
        public static bool operator >(FileBase<InternalFileType, FileType> file1, IFile file2)
        {
            return !(file1 is null)
                && !(file2 is null)
                && string.Compare(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase) > 0;
        }

        /// <summary>
        /// Greater than or equal
        /// </summary>
        /// <param name="file1">File 1</param>
        /// <param name="file2">File 2</param>
        /// <returns>The result</returns>
        public static bool operator >=(FileBase<InternalFileType, FileType> file1, IFile file2)
        {
            return !(file1 is null)
                && !(file2 is null)
                && string.Compare(file1.FullName, file2.FullName, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Compares this to another file
        /// </summary>
        /// <param name="other">File to compare to</param>
        /// <returns></returns>
        public int CompareTo(IFile other)
        {
            if (other is null)
                return 1;
            return string.Compare(FullName, other.FullName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Compares this object to another object
        /// </summary>
        /// <param name="obj">Object to compare it to</param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (!(obj is FileBase<InternalFileType, FileType> Temp))
                return 1;
            return CompareTo(Temp);
        }

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        public abstract IFile CopyTo(IDirectory directory, bool overwrite);

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        public virtual Task<IFile?> CopyToAsync(IDirectory directory, bool overwrite)
        {
            return Task.FromResult<IFile?>(CopyTo(directory, overwrite));
        }

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>Any response for deleting the resource (usually FTP, HTTP, etc)</returns>
        public abstract string Delete();

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>Any response for deleting the resource (usually FTP, HTTP, etc)</returns>
        public virtual Task<string> DeleteAsync()
        {
            return Task.FromResult(Delete());
        }

        /// <summary>
        /// Determines if the objects are equal
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            return obj is FileBase<InternalFileType, FileType> File && File == this;
        }

        /// <summary>
        /// Determines if the files are equal
        /// </summary>
        /// <param name="other">Other file</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public bool Equals(IFile other)
        {
            return other?.FullName == FullName;
        }

        /// <summary>
        /// Gets the hash code for the file
        /// </summary>
        /// <returns>The hash code</returns>
        public override int GetHashCode() => FullName.GetHashCode();

        /// <summary>
        /// Moves the file to a new directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        public abstract IFile MoveTo(IDirectory directory);

        /// <summary>
        /// Moves the file to another directory
        /// </summary>
        /// <param name="directory">Directory to move the file to</param>
        /// <returns></returns>
        public virtual Task<IFile> MoveToAsync(IDirectory directory)
        {
            return Task.FromResult(MoveTo(directory));
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
            if (!(FormatManager?.FindFormat(FullName, Credentials) is IFormat<TFile> Format))
                throw new ArgumentException("Could not find file format that returns the specified object type");
            using var TempStream = new MemoryStream(ReadBinary());
            return Format.Read(TempStream);
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
            var Format = FormatManager?.FindFormat(FullName, Credentials);
            if (Format is null)
                throw new ArgumentException("Could not find file format that returns the specified object type");
            using var TempStream = new MemoryStream(ReadBinary());
            return Format.ReadBase(TempStream);
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <typeparam name="TFile">The type of the file object expected.</typeparam>
        /// <returns>The parsed file</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        public async Task<TFile> ParseAsync<TFile>() where TFile : IGenericFile
        {
            if (!(FormatManager?.FindFormat(FullName, Credentials) is IFormat<TFile> Format))
                throw new ArgumentException("Could not find file format that returns the specified object type");
            using var TempStream = new MemoryStream(await ReadBinaryAsync().ConfigureAwait(false));
            return await Format.ReadAsync(TempStream).ConfigureAwait(false);
        }

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns>The parsed file</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        public async Task<IGenericFile> ParseAsync()
        {
            var Format = FormatManager?.FindFormat(FullName, Credentials);
            if (Format is null)
                throw new ArgumentException("Could not find file format that returns the specified object type");
            using var TempStream = new MemoryStream(await ReadBinaryAsync().ConfigureAwait(false));
            return await Format.ReadBaseAsync(TempStream).ConfigureAwait(false);
        }

        /// <summary>
        /// Reads the file in as a string
        /// </summary>
        /// <returns>The file contents as a string</returns>
        public abstract string Read();

        /// <summary>
        /// Reads the file to the end as a string
        /// </summary>
        /// <returns>A string containing the contents of the file</returns>
        public virtual Task<string> ReadAsync()
        {
            return Task.FromResult(Read());
        }

        /// <summary>
        /// Reads a file as binary
        /// </summary>
        /// <returns>The file contents as a byte array</returns>
        public abstract byte[] ReadBinary();

        /// <summary>
        /// Reads the file to the end as a byte array
        /// </summary>
        /// <returns>A byte array containing the contents of the file</returns>
        public virtual Task<byte[]> ReadBinaryAsync()
        {
            return Task.FromResult(ReadBinary());
        }

        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="newName">New name for the file</param>
        public abstract IFile Rename(string newName);

        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="newName">New file name</param>
        /// <returns></returns>
        public virtual Task<IFile> RenameAsync(string newName)
        {
            return Task.FromResult(Rename(newName));
        }

        /// <summary>
        /// Returns the name of the file
        /// </summary>
        /// <returns>The name of the file</returns>
        public override string ToString() => FullName;

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">Mode to open the file as</param>
        /// <param name="encoding">Encoding to use for the content</param>
        /// <returns>The result of the write or original content</returns>
        public abstract string Write(string content, FileMode mode = FileMode.Create, Encoding? encoding = null);

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">Mode to open the file as</param>
        /// <returns>The result of the write or original content</returns>
        public abstract byte[] Write(byte[] content, FileMode mode = FileMode.Create);

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>True if it was written successfully, false otherwise.</returns>
        public bool Write(IGenericFile data, FileMode mode = FileMode.Create)
        {
            var Format = FormatManager?.FindFormat(FullName, Credentials);
            if (Format is null)
                return false;
            using var TempStream = new MemoryStream();
            var Success = Format.Write(TempStream, data);
            Write(TempStream.ReadAllBinary(), mode);
            return Success;
        }

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">File mode</param>
        /// <param name="encoding">Encoding that the content should be saved as (default is UTF8)</param>
        /// <returns>The result of the write or original content</returns>
        public virtual Task<string> WriteAsync(string content, FileMode mode = FileMode.Create, Encoding? encoding = null)
        {
            return Task.FromResult(Write(content, mode, encoding));
        }

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">File mode</param>
        /// <returns>The result of the write or original content</returns>
        public virtual Task<byte[]> WriteAsync(byte[] content, FileMode mode = FileMode.Create)
        {
            return Task.FromResult(Write(content, mode));
        }

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>True if it was written successfully, false otherwise.</returns>
        public async Task<bool> WriteAsync(IGenericFile data, FileMode mode = FileMode.Create)
        {
            var Format = FormatManager?.FindFormat(FullName, Credentials);
            if (Format is null)
                return false;
            using var TempStream = new MemoryStream();
            var Success = await Format.WriteAsync(TempStream, data).ConfigureAwait(false);
            await WriteAsync(await TempStream.ReadAllBinaryAsync().ConfigureAwait(false), mode).ConfigureAwait(false);
            return Success;
        }
    }
}