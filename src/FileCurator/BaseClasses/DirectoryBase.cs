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

namespace FileCurator.BaseClasses
{
    /// <summary>
    /// Directory base class
    /// </summary>
    /// <typeparam name="InternalDirectoryType">
    /// Data type internally to hold true directory info
    /// </typeparam>
    /// <typeparam name="DirectoryType">Directory type</typeparam>
    public abstract class DirectoryBase<InternalDirectoryType, DirectoryType> : IDirectory
        where DirectoryType : DirectoryBase<InternalDirectoryType, DirectoryType>, new()
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
        /// <param name="domain">Domain of the user (optional)</param>
        /// <param name="password">Password to be used to access the file (optional)</param>
        /// <param name="userName">User name to be used to access the file (optional)</param>
        protected DirectoryBase(InternalDirectoryType internalDirectory, string userName = "", string password = "", string domain = "")
        {
            InternalDirectory = internalDirectory;
            UserName = userName;
            Password = password;
            Domain = domain;
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
        public abstract IDirectory Parent { get; }

        /// <summary>
        /// Root directory
        /// </summary>
        public abstract IDirectory Root { get; }

        /// <summary>
        /// Size of the directory
        /// </summary>
        public abstract long Size { get; }

        /// <summary>
        /// Domain
        /// </summary>
        protected string Domain { get; set; }

        /// <summary>
        /// Internal directory
        /// </summary>
        protected InternalDirectoryType InternalDirectory { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        protected string Password { get; set; }

        /// <summary>
        /// User name
        /// </summary>
        protected string UserName { get; set; }

        /// <summary>
        /// Determines if two directories are not equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>True if they are not equal, false otherwise</returns>
        public static bool operator !=(DirectoryBase<InternalDirectoryType, DirectoryType> directory1, IDirectory directory2)
        {
            return !(directory1 == directory2);
        }

        /// <summary>
        /// Less than
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator <(DirectoryBase<InternalDirectoryType, DirectoryType> directory1, IDirectory directory2)
        {
            if (directory1 == null || directory2 == null)
                return false;
            return string.Compare(directory1.FullName, directory2.FullName, StringComparison.OrdinalIgnoreCase) < 0;
        }

        /// <summary>
        /// Less than or equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator <=(DirectoryBase<InternalDirectoryType, DirectoryType> directory1, IDirectory directory2)
        {
            if (directory1 == null || directory2 == null)
                return false;
            return string.Compare(directory1.FullName, directory2.FullName, StringComparison.OrdinalIgnoreCase) <= 0;
        }

        /// <summary>
        /// Determines if two directories are equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>True if they are, false otherwise</returns>
        public static bool operator ==(DirectoryBase<InternalDirectoryType, DirectoryType> directory1, IDirectory directory2)
        {
            if ((object)directory1 == null && (object)directory2 == null)
                return true;
            if ((object)directory1 == null || (object)directory2 == null)
                return false;
            return directory1.FullName == directory2.FullName;
        }

        /// <summary>
        /// Greater than
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator >(DirectoryBase<InternalDirectoryType, DirectoryType> directory1, IDirectory directory2)
        {
            if (directory1 == null || directory2 == null)
                return false;
            return string.Compare(directory1.FullName, directory2.FullName, StringComparison.OrdinalIgnoreCase) > 0;
        }

        /// <summary>
        /// Greater than or equal
        /// </summary>
        /// <param name="directory1">Directory 1</param>
        /// <param name="directory2">Directory 2</param>
        /// <returns>The result</returns>
        public static bool operator >=(DirectoryBase<InternalDirectoryType, DirectoryType> directory1, IDirectory directory2)
        {
            if (directory1 == null || directory2 == null)
                return false;
            return string.Compare(directory1.FullName, directory2.FullName, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        /// <summary>
        /// Compares this to another directory
        /// </summary>
        /// <param name="other">Directory to compare to</param>
        /// <returns></returns>
        public int CompareTo(IDirectory other)
        {
            if (other == null)
                return 1;
            if (InternalDirectory == null)
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
            var Temp = obj as IDirectory;
            if (Temp == null)
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
            if (InternalDirectory == null || directory == null)
                return this;
            directory.Create();
            foreach (IFile TempFile in EnumerateFiles())
            {
                switch (options)
                {
                    case CopyOptions.CopyAlways:
                        TempFile.CopyTo(directory, true);
                        break;

                    case CopyOptions.CopyIfNewer:
                        if (new FileInfo(directory.FullName + "\\" + TempFile.Name.Replace("/", "").Replace("\\", ""), UserName, Password, Domain).Exists)
                        {
                            var FileInfo = new FileInfo(directory.FullName + "\\" + TempFile.Name.Replace("/", "").Replace("\\", ""), UserName, Password, Domain);
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
            foreach (IDirectory SubDirectory in EnumerateDirectories())
                SubDirectory.CopyTo(new DirectoryInfo(directory.FullName + "\\" + SubDirectory.Name.Replace("/", "").Replace("\\", ""), UserName, Password, Domain), options);
            return directory;
        }

        /// <summary>
        /// Creates the directory
        /// </summary>
        public abstract void Create();

        /// <summary>
        /// Deletes the directory
        /// </summary>
        public abstract void Delete();

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
        public IEnumerable<IDirectory> EnumerateDirectories(Predicate<IDirectory> predicate, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            return EnumerateDirectories("*", options).Where(x => predicate(x));
        }

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
        public IEnumerable<IFile> EnumerateFiles(Predicate<IFile> predicate, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            return EnumerateFiles("*", options).Where(x => predicate(x));
        }

        /// <summary>
        /// Determines if the two directories are the same
        /// </summary>
        /// <param name="obj">Object to compare to</param>
        /// <returns>True if they're the same, false otherwise</returns>
        public override bool Equals(object obj)
        {
            var Other = obj as DirectoryBase<InternalDirectoryType, DirectoryType>;
            return Other != null && Other == this;
        }

        /// <summary>
        /// Determines if the directories are equal
        /// </summary>
        /// <param name="other">Other directory</param>
        /// <returns>True if they are equal, false otherwise</returns>
        public bool Equals(IDirectory other)
        {
            if (other == null)
                return false;
            return FullName == other.FullName;
        }

        /// <summary>
        /// Enumerates the files in the directory
        /// </summary>
        /// <returns>The files in the directory</returns>
        public IEnumerator<IFile> GetEnumerator()
        {
            foreach (IFile File in EnumerateFiles())
                yield return File;
        }

        /// <summary>
        /// Returns the hash code for the directory
        /// </summary>
        /// <returns>The hash code for the directory</returns>
        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        /// <summary>
        /// Moves this directory under another directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        public virtual IDirectory MoveTo(IDirectory directory)
        {
            var ReturnValue = CopyTo(new DirectoryInfo(directory.FullName + "\\" + Name.Replace("/", "").Replace("\\", ""), UserName, Password, Domain));
            Delete();
            return ReturnValue;
        }

        /// <summary>
        /// Renames the directory
        /// </summary>
        /// <param name="name">Name of the new directory</param>
        public abstract void Rename(string name);

        /// <summary>
        /// Enumerates the files and directories in the directory
        /// </summary>
        /// <returns>The files and directories</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            foreach (IFile File in EnumerateFiles())
                yield return File;
        }

        /// <summary>
        /// Gets info for the directory
        /// </summary>
        /// <returns>The full path to the directory</returns>
        public override string ToString()
        {
            return FullName;
        }
    }
}