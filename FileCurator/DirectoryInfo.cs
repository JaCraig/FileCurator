﻿/*
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
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileCurator
{
    /// <summary>
    /// Directory info class
    /// </summary>
    public class DirectoryInfo : IDirectory
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="credentials">The credentials.</param>
        public DirectoryInfo(string path, Credentials? credentials = null)
            : this(FileSystem.Instance?.Directory(path, credentials))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directory">Directory object</param>
        public DirectoryInfo(IDirectory? directory)
        {
            InternalDirectory = directory;
        }

        /// <summary>
        /// Last time it was accessed
        /// </summary>
        public DateTime Accessed => InternalDirectory?.Accessed ?? DateTime.Now;

        /// <summary>
        /// When it was created
        /// </summary>
        public DateTime Created => InternalDirectory?.Created ?? DateTime.Now;

        /// <summary>
        /// Does the directory exist
        /// </summary>
        public bool Exists => InternalDirectory?.Exists == true;

        /// <summary>
        /// Full path to the directory
        /// </summary>
        public string FullName => InternalDirectory?.FullName ?? string.Empty;

        /// <summary>
        /// When it was last modified
        /// </summary>
        public DateTime Modified => InternalDirectory?.Modified ?? DateTime.Now;

        /// <summary>
        /// Name of the directory
        /// </summary>
        public string Name => InternalDirectory?.Name ?? string.Empty;

        /// <summary>
        /// Parent directory
        /// </summary>
        public IDirectory? Parent => InternalDirectory is null ? null : new DirectoryInfo(InternalDirectory.Parent);

        /// <summary>
        /// Root directory
        /// </summary>
        public IDirectory? Root => InternalDirectory is null ? null : new DirectoryInfo(InternalDirectory.Root);

        /// <summary>
        /// Size of the contents of the directory in bytes
        /// </summary>
        public long Size => InternalDirectory?.Size ?? 0;

        /// <summary>
        /// Internal directory object
        /// </summary>
        protected IDirectory? InternalDirectory { get; }

        /// <summary>
        /// Determines if two directories are not equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>True if they are not equal, false otherwise</returns>
        public static bool operator !=(DirectoryInfo directory1, DirectoryInfo directory2)
        {
            return !(directory1 == directory2);
        }

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator <(DirectoryInfo directory1, DirectoryInfo directory2)
        {
            return !(directory1 is null)
                && !(directory2 is null)
                && string.Compare(directory1.FullName, directory2.FullName, StringComparison.OrdinalIgnoreCase) < 0;
        }

        /// <summary>
        /// Less than or equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator <=(DirectoryInfo directory1, DirectoryInfo directory2)
        {
            return !(directory1 is null)
                && !(directory2 is null)
                && string.Compare(directory1.FullName, directory2.FullName, StringComparison.OrdinalIgnoreCase) <= 0;
        }

        /// <summary>
        /// Determines if two directories are equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>True if they are, false otherwise</returns>
        public static bool operator ==(DirectoryInfo directory1, DirectoryInfo directory2)
        {
            return (directory1 is null && directory2 is null)
                || (!(directory1 is null) && !(directory2 is null) && directory1.FullName == directory2.FullName);
        }

        /// <summary>
        /// Greater than
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator >(DirectoryInfo directory1, DirectoryInfo directory2)
        {
            return !(directory1 is null)
                && !(directory2 is null)
                && string.Compare(directory1.FullName, directory2.FullName, StringComparison.OrdinalIgnoreCase) > 0;
        }

        /// <summary>
        /// Greater than or equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator >=(DirectoryInfo directory1, DirectoryInfo directory2)
        {
            return !(directory1 is null)
                && !(directory2 is null)
                && string.Compare(directory1.FullName, directory2.FullName, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Compares this to another directory
        /// </summary>
        /// <param name="other">Directory to compare to</param>
        /// <returns></returns>
        public int CompareTo(IDirectory? other)
        {
            if (other is null)
                return 1;
            if (InternalDirectory is null)
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
            if (!(obj is DirectoryInfo Temp))
                return 1;
            return CompareTo(Temp);
        }

        /// <summary>
        /// Copies the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to copy to</param>
        /// <param name="options">Copy options</param>
        /// <returns>Returns the new directory</returns>
        public IDirectory CopyTo(IDirectory? directory, CopyOptions options = CopyOptions.CopyAlways)
        {
            if (InternalDirectory is null || directory is null)
                return this;
            return InternalDirectory.CopyTo(directory, options);
        }

        /// <summary>
        /// Copies the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to copy to</param>
        /// <param name="options">Copy options</param>
        /// <returns></returns>
        public Task<IDirectory> CopyToAsync(IDirectory directory, CopyOptions options = CopyOptions.CopyAlways)
        {
            if (InternalDirectory is null || directory is null)
                return Task.FromResult<IDirectory>(this);
            return InternalDirectory.CopyToAsync(directory, options);
        }

        /// <summary>
        /// Creates the directory if it does not currently exist
        /// </summary>
        public IDirectory Create()
        {
            InternalDirectory?.Create();
            return this;
        }

        /// <summary>
        /// Creates the directory if it does not currently exist
        /// </summary>
        /// <returns></returns>
        public async Task<IDirectory> CreateAsync()
        {
            if (InternalDirectory is null)
                return this;
            await InternalDirectory.CreateAsync().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Deletes the directory
        /// </summary>
        public IDirectory Delete()
        {
            InternalDirectory?.Delete();
            return this;
        }

        /// <summary>
        /// Deletes the directory
        /// </summary>
        /// <returns></returns>
        public async Task<IDirectory> DeleteAsync()
        {
            if (InternalDirectory is null)
                return this;
            await InternalDirectory.DeleteAsync().ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Enumerates sub directories (defaults to top level sub directories)
        /// </summary>
        /// <param name="searchPattern">Search pattern to use</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of directories</returns>
        public IEnumerable<IDirectory> EnumerateDirectories(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (InternalDirectory is null)
                yield break;
            foreach (var TempDirectory in InternalDirectory.EnumerateDirectories(searchPattern, options))
            {
                yield return new DirectoryInfo(TempDirectory);
            }
        }

        /// <summary>
        /// Enumerates sub directories (defaults to top level sub directories)
        /// </summary>
        /// <param name="predicate">Predicate used to filter directories</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of directories</returns>
        public IEnumerable<IDirectory> EnumerateDirectories(Predicate<IDirectory> predicate, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (InternalDirectory is null)
                return Array.Empty<IDirectory>();
            return InternalDirectory.EnumerateDirectories("*", options).Where(x => predicate(x)).Select(x => new DirectoryInfo(x));
        }

        /// <summary>
        /// Enumerates files within the directory (defaults to top level directory and not the sub directories)
        /// </summary>
        /// <param name="searchPattern">Search pattern to use</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of files</returns>
        public IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (InternalDirectory is null)
                yield break;
            foreach (var TempFile in InternalDirectory.EnumerateFiles(searchPattern, options))
            {
                yield return new FileInfo(TempFile);
            }
        }

        /// <summary>
        /// Enumerates files within the directory (defaults to top level directory and not the sub directories)
        /// </summary>
        /// <param name="predicate">Predicate used to filter files</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of files</returns>
        public IEnumerable<IFile> EnumerateFiles(Predicate<IFile> predicate, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (InternalDirectory is null)
                return Array.Empty<IFile>();
            return InternalDirectory.EnumerateFiles("*", options).Where(x => predicate(x)).Select(x => new FileInfo(x));
        }

        /// <summary>
        /// Determines if the two directories are the same
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if they're the same, false otherwise</returns>
        public override bool Equals(object obj)
        {
            return obj is DirectoryInfo Other && Other == this;
        }

        /// <summary>
        /// Determines if the directories are equal
        /// </summary>
        /// <param name="other">Other directory</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public bool Equals(IDirectory? other)
        {
            return FullName == other?.FullName;
        }

        /// <summary>
        /// Enumerates the files in the directory
        /// </summary>
        /// <returns>The files in the directory</returns>
        public IEnumerator<IFile> GetEnumerator()
        {
            foreach (FileInfo TempFile in EnumerateFiles())
                yield return TempFile;
        }

        /// <summary>
        /// Enumerates the files and directories in the directory
        /// </summary>
        /// <returns>The files and directories</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (FileInfo TempFile in EnumerateFiles())
                yield return TempFile;
        }

        /// <summary>
        /// Returns the hash code for the directory
        /// </summary>
        /// <returns>The hash code for the directory</returns>
        public override int GetHashCode() => FullName.GetHashCode();

        /// <summary>
        /// Moves the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        public IDirectory MoveTo(IDirectory? directory)
        {
            if (InternalDirectory is null || directory is null)
                return this;
            return InternalDirectory.MoveTo(directory);
        }

        /// <summary>
        /// Moves the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        /// <returns></returns>
        public Task<IDirectory> MoveToAsync(IDirectory directory)
        {
            if (InternalDirectory is null || directory is null)
                return Task.FromResult<IDirectory>(this);
            return InternalDirectory.MoveToAsync(directory);
        }

        /// <summary>
        /// Renames the directory
        /// </summary>
        /// <param name="name">The new name of the directory</param>
        public IDirectory Rename(string name)
        {
            if (InternalDirectory is null || string.IsNullOrEmpty(name))
                return this;
            InternalDirectory.Rename(name);
            return this;
        }

        /// <summary>
        /// Renames the directory
        /// </summary>
        /// <param name="name">The new name of the directory</param>
        /// <returns></returns>
        public async Task<IDirectory> RenameAsync(string name)
        {
            if (InternalDirectory is null || string.IsNullOrEmpty(name))
                return this;
            await InternalDirectory.RenameAsync(name).ConfigureAwait(false);
            return this;
        }

        /// <summary>
        /// Gets info for the directory
        /// </summary>
        /// <returns>The full path to the directory</returns>
        public override string ToString() => FullName;
    }
}