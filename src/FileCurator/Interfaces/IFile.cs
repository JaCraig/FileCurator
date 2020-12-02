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

using FileCurator.Formats.Data.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileCurator.Interfaces
{
    /// <summary>
    /// Represents an individual file
    /// </summary>
    public interface IFile : IComparable<IFile>, IComparable, IEquatable<IFile>
    {
        /// <summary>
        /// Last time the file was accessed
        /// </summary>
        DateTime Accessed { get; }

        /// <summary>
        /// When the file was created
        /// </summary>
        DateTime Created { get; }

        /// <summary>
        /// Directory the file is in
        /// </summary>
        IDirectory? Directory { get; }

        /// <summary>
        /// Does the file exist currently
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// File extension
        /// </summary>
        string Extension { get; }

        /// <summary>
        /// Full path to the file
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// Size of the file in bytes
        /// </summary>
        long Length { get; }

        /// <summary>
        /// When the file was last modified
        /// </summary>
        DateTime Modified { get; }

        /// <summary>
        /// File name
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        IFile? CopyTo(IDirectory directory, bool overwrite);

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        Task<IFile?> CopyToAsync(IDirectory directory, bool overwrite);

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>Any response for deleting the resource (usually FTP, HTTP, etc)</returns>
        string Delete();

        /// <summary>
        /// Deletes the file
        /// </summary>
        /// <returns>Any response for deleting the resource (usually FTP, HTTP, etc)</returns>
        Task<string> DeleteAsync();

        /// <summary>
        /// Moves the file to another directory
        /// </summary>
        /// <param name="directory">Directory to move the file to</param>
        IFile MoveTo(IDirectory directory);

        /// <summary>
        /// Moves the file to another directory
        /// </summary>
        /// <param name="directory">Directory to move the file to</param>
        Task<IFile> MoveToAsync(IDirectory directory);

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <typeparam name="TFile">The type of the file object expected.</typeparam>
        /// <returns>The parsed file</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        TFile Parse<TFile>() where TFile : IGenericFile;

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns>The parsed file</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        IGenericFile Parse();

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <typeparam name="TFile">The type of the file object expected.</typeparam>
        /// <returns>The parsed file</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        Task<TFile> ParseAsync<TFile>() where TFile : IGenericFile;

        /// <summary>
        /// Parses this instance.
        /// </summary>
        /// <returns>The parsed file</returns>
        /// <exception cref="ArgumentException">
        /// Could not find file format that returns the specified object type
        /// </exception>
        Task<IGenericFile> ParseAsync();

        /// <summary>
        /// Reads the file to the end as a string
        /// </summary>
        /// <returns>A string containing the contents of the file</returns>
        string Read();

        /// <summary>
        /// Reads the file to the end as a string
        /// </summary>
        /// <returns>A string containing the contents of the file</returns>
        Task<string> ReadAsync();

        /// <summary>
        /// Reads the file to the end as a byte array
        /// </summary>
        /// <returns>A byte array containing the contents of the file</returns>
        byte[] ReadBinary();

        /// <summary>
        /// Reads the file to the end as a byte array
        /// </summary>
        /// <returns>A byte array containing the contents of the file</returns>
        Task<byte[]> ReadBinaryAsync();

        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="newName">New file name</param>
        IFile Rename(string newName);

        /// <summary>
        /// Renames the file
        /// </summary>
        /// <param name="newName">New file name</param>
        Task<IFile> RenameAsync(string newName);

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">File mode</param>
        /// <param name="encoding">Encoding that the content should be saved as (default is UTF8)</param>
        /// <returns>The result of the write or original content</returns>
        string Write(string content, FileMode mode = FileMode.Create, Encoding? encoding = null);

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">File mode</param>
        /// <returns>The result of the write or original content</returns>
        byte[] Write(byte[] content, FileMode mode = FileMode.Create);

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>True if it was written successfully, false otherwise.</returns>
        bool Write(IGenericFile data, FileMode mode = FileMode.Create);

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">File mode</param>
        /// <param name="encoding">Encoding that the content should be saved as (default is UTF8)</param>
        /// <returns>The result of the write or original content</returns>
        Task<string> WriteAsync(string content, FileMode mode = FileMode.Create, Encoding? encoding = null);

        /// <summary>
        /// Writes content to the file
        /// </summary>
        /// <param name="content">Content to write</param>
        /// <param name="mode">File mode</param>
        /// <returns>The result of the write or original content</returns>
        Task<byte[]> WriteAsync(byte[] content, FileMode mode = FileMode.Create);

        /// <summary>
        /// Writes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>True if it was written successfully, false otherwise.</returns>
        Task<bool> WriteAsync(IGenericFile data, FileMode mode = FileMode.Create);
    }
}