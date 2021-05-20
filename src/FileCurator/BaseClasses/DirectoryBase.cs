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
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileCurator.BaseClasses
{
    /// <summary>
    /// Directory base class
    /// </summary>
    /// <typeparam name="TInternalDirectoryType">
    /// Data type internally to hold true directory info
    /// </typeparam>
    /// <typeparam name="TDirectoryType">Directory type</typeparam>
    public abstract class DirectoryBase<TInternalDirectoryType, TDirectoryType> : IDirectory
        where TDirectoryType : DirectoryBase<TInternalDirectoryType, TDirectoryType>, new()
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected DirectoryBase()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="internalDirectory">Internal directory object</param>
        /// <param name="credentials">The credentials.</param>
        protected DirectoryBase(TInternalDirectoryType internalDirectory, Credentials? credentials = null)
        {
            Credentials = credentials;
            InternalDirectory = internalDirectory;
        }

        /// <summary>
        /// Last time accessed (UTC time)
        /// </summary>
        public abstract DateTime Accessed { get; }

        /// <summary>
        /// Date created (UTC time)
        /// </summary>
        public abstract DateTime Created { get; }

        /// <summary>
        /// Gets the credentials.
        /// </summary>
        /// <value>The credentials.</value>
        public Credentials? Credentials { get; }

        /// <summary>
        /// Does it exist?
        /// </summary>
        public abstract bool Exists { get; }

        /// <summary>
        /// Full path
        /// </summary>
        public abstract string FullName { get; }

        /// <summary>
        /// Date modified (UTC time)
        /// </summary>
        public abstract DateTime Modified { get; }

        /// <summary>
        /// Name
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Parent directory
        /// </summary>
        public abstract IDirectory? Parent { get; }

        /// <summary>
        /// Root directory
        /// </summary>
        public abstract IDirectory? Root { get; }

        /// <summary>
        /// Size of the directory
        /// </summary>
        public abstract long Size { get; }

        /// <summary>
        /// Internal directory
        /// </summary>
        protected TInternalDirectoryType InternalDirectory { get; set; }

        /// <summary>
        /// Determines if two directories are not equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>True if they are not equal, false otherwise</returns>
        public static bool operator !=(DirectoryBase<TInternalDirectoryType, TDirectoryType> directory1, IDirectory directory2)
        {
            return !(directory1 == directory2);
        }

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator <(DirectoryBase<TInternalDirectoryType, TDirectoryType> directory1, IDirectory directory2)
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
        public static bool operator <=(DirectoryBase<TInternalDirectoryType, TDirectoryType> directory1, IDirectory directory2)
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
        public static bool operator ==(DirectoryBase<TInternalDirectoryType, TDirectoryType> directory1, IDirectory directory2)
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
        public static bool operator >(DirectoryBase<TInternalDirectoryType, TDirectoryType> directory1, IDirectory directory2)
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
        public static bool operator >=(DirectoryBase<TInternalDirectoryType, TDirectoryType> directory1, IDirectory directory2)
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
        public int CompareTo(IDirectory other)
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
            if (!(obj is IDirectory Temp))
                return 1;
            return CompareTo(Temp);
        }

        /// <summary>
        /// Copies the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to copy to</param>
        /// <param name="options">Copy options</param>
        /// <returns>Returns the new directory</returns>
        public virtual IDirectory CopyTo(IDirectory directory, CopyOptions options = CopyOptions.CopyAlways)
        {
            if (InternalDirectory is null || directory is null || string.IsNullOrEmpty(directory.FullName))
                return this;
            directory.Create();
            foreach (var TempFile in EnumerateFiles())
            {
                switch (options)
                {
                    case CopyOptions.CopyAlways:
                        TempFile.CopyTo(directory, true);
                        break;

                    case CopyOptions.CopyIfNewer:
                        if (new FileInfo(directory.FullName + "\\" + TempFile.Name.Replace("/", "").Replace("\\", ""), Credentials).Exists)
                        {
                            var FileInfo = new FileInfo(directory.FullName + "\\" + TempFile.Name.Replace("/", "").Replace("\\", ""), Credentials);
                            if (FileInfo.Modified.CompareTo(TempFile.Modified) < 0)
                                TempFile.CopyTo(directory, true);
                        }
                        else
                        {
                            TempFile.CopyTo(directory, true);
                        }

                        break;

                    case CopyOptions.DoNotOverwrite:
                        TempFile.CopyTo(directory, false);
                        break;
                }
            }
            foreach (var SubDirectory in EnumerateDirectories())
                SubDirectory.CopyTo(new DirectoryInfo(directory.FullName + "\\" + SubDirectory.Name.Replace("/", "").Replace("\\", ""), Credentials), options);
            return directory;
        }

        /// <summary>
        /// Copies the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to copy to</param>
        /// <param name="options">Copy options</param>
        /// <returns></returns>
        public async Task<IDirectory> CopyToAsync(IDirectory directory, CopyOptions options = CopyOptions.CopyAlways)
        {
            if (InternalDirectory is null || directory is null || string.IsNullOrEmpty(directory.FullName))
                return this;
            directory.Create();
            List<Task> Tasks = new List<Task>();
            foreach (var TempFile in EnumerateFiles())
            {
                switch (options)
                {
                    case CopyOptions.CopyAlways:
                        Tasks.Add(TempFile.CopyToAsync(directory, true));
                        break;

                    case CopyOptions.CopyIfNewer:
                        if (new FileInfo(directory.FullName + "\\" + TempFile.Name.Replace("/", "").Replace("\\", ""), Credentials).Exists)
                        {
                            var FileInfo = new FileInfo(directory.FullName + "\\" + TempFile.Name.Replace("/", "").Replace("\\", ""), Credentials);
                            if (FileInfo.Modified.CompareTo(TempFile.Modified) < 0)
                                Tasks.Add(TempFile.CopyToAsync(directory, true));
                        }
                        else
                        {
                            Tasks.Add(TempFile.CopyToAsync(directory, true));
                        }

                        break;

                    case CopyOptions.DoNotOverwrite:
                        Tasks.Add(TempFile.CopyToAsync(directory, true));
                        break;
                }
            }
            await Task.WhenAll(Tasks).ConfigureAwait(false);
            Tasks.Clear();
            foreach (var SubDirectory in EnumerateDirectories())
            {
                Tasks.Add(SubDirectory.CopyToAsync(new DirectoryInfo(directory.FullName + "\\" + SubDirectory.Name.Replace("/", "").Replace("\\", ""), Credentials), options));
            }
            await Task.WhenAll(Tasks).ConfigureAwait(false);
            return directory;
        }

        /// <summary>
        /// Creates the directory
        /// </summary>
        public abstract IDirectory Create();

        /// <summary>
        /// Creates the directory if it does not currently exist
        /// </summary>
        /// <returns></returns>
        public virtual Task<IDirectory> CreateAsync()
        {
            return Task.FromResult(Create());
        }

        /// <summary>
        /// Deletes the directory
        /// </summary>
        public abstract IDirectory Delete();

        /// <summary>
        /// Deletes the directory
        /// </summary>
        /// <returns></returns>
        public virtual Task<IDirectory> DeleteAsync()
        {
            return Task.FromResult(Delete());
        }

        /// <summary>
        /// Enumerates directories under this directory
        /// </summary>
        /// <param name="searchPattern">Search pattern</param>
        /// <param name="options">Search options</param>
        /// <returns>List of directories under this directory</returns>
        public abstract IEnumerable<IDirectory> EnumerateDirectories(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Enumerates sub directories (defaults to top level sub directories)
        /// </summary>
        /// <param name="predicate">Predicate used to filter directories</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of directories</returns>
        public IEnumerable<IDirectory> EnumerateDirectories(Predicate<IDirectory> predicate, SearchOption options = SearchOption.TopDirectoryOnly) => EnumerateDirectories("*", options).Where(x => predicate(x));

        /// <summary>
        /// Enumerates files under this directory
        /// </summary>
        /// <param name="searchPattern">Search pattern</param>
        /// <param name="options">Search options</param>
        /// <returns>List of files under this directory</returns>
        public abstract IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Enumerates files within the directory (defaults to top level directory and not the sub directories)
        /// </summary>
        /// <param name="predicate">Predicate used to filter files</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of files</returns>
        public IEnumerable<IFile> EnumerateFiles(Predicate<IFile> predicate, SearchOption options = SearchOption.TopDirectoryOnly) => EnumerateFiles("*", options).Where(x => predicate(x));

        /// <summary>
        /// Determines if the two directories are the same
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if they're the same, false otherwise</returns>
        public override bool Equals(object obj)
        {
            return obj is DirectoryBase<TInternalDirectoryType, TDirectoryType> Other && Other == this;
        }

        /// <summary>
        /// Determines if the directories are equal
        /// </summary>
        /// <param name="other">Other directory</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public bool Equals(IDirectory other)
        {
            return FullName == other?.FullName;
        }

        /// <summary>
        /// Enumerates the files in the directory
        /// </summary>
        /// <returns>The files in the directory</returns>
        public IEnumerator<IFile> GetEnumerator()
        {
            foreach (var TempFile in EnumerateFiles())
                yield return TempFile;
        }

        /// <summary>
        /// Enumerates the files and directories in the directory
        /// </summary>
        /// <returns>The files and directories</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (var TempFile in EnumerateFiles())
                yield return TempFile;
        }

        /// <summary>
        /// Returns the hash code for the directory
        /// </summary>
        /// <returns>The hash code for the directory</returns>
        public override int GetHashCode() => FullName.GetHashCode();

        /// <summary>
        /// Moves this directory under another directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        public virtual IDirectory MoveTo(IDirectory directory)
        {
            if (directory is null || string.IsNullOrEmpty(directory.FullName))
                return this;
            var ReturnValue = CopyTo(new DirectoryInfo(directory.FullName + "\\" + Name.Replace("/", "").Replace("\\", ""), Credentials));
            Delete();
            return ReturnValue;
        }

        /// <summary>
        /// Moves the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        /// <returns></returns>
        public async Task<IDirectory> MoveToAsync(IDirectory directory)
        {
            if (directory is null || string.IsNullOrEmpty(directory.FullName))
                return this;
            var ReturnValue = await CopyToAsync(new DirectoryInfo(directory.FullName + "\\" + Name.Replace("/", "").Replace("\\", ""), Credentials)).ConfigureAwait(false);
            await DeleteAsync().ConfigureAwait(false);
            return ReturnValue;
        }

        /// <summary>
        /// Renames the directory
        /// </summary>
        /// <param name="name">Name of the new directory</param>
        public abstract IDirectory Rename(string name);

        /// <summary>
        /// Renames the directory
        /// </summary>
        /// <param name="name">The new name of the directory</param>
        /// <returns></returns>
        public virtual Task<IDirectory> RenameAsync(string name)
        {
            return Task.FromResult(Rename(name));
        }

        /// <summary>
        /// Gets info for the directory
        /// </summary>
        /// <returns>The full path to the directory</returns>
        public override string ToString() => FullName;
    }
}