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
using System;
using System.Collections.Generic;
using System.IO;

namespace FileCurator.Interfaces
{
    /// <summary>
    /// Represents a directory
    /// </summary>
    /// <seealso cref="System.IComparable{IDirectory}"/>
    /// <seealso cref="System.Collections.Generic.IEnumerable{IFile}"/>
    /// <seealso cref="IComparable"/>
    /// <seealso cref="System.IEquatable{IDirectory}"/>
    public interface IDirectory : IComparable<IDirectory>, IEnumerable<IFile>, IComparable, IEquatable<IDirectory>
    {
        /// <summary>
        /// Last time it was accessed
        /// </summary>
        DateTime Accessed { get; }

        /// <summary>
        /// When it was created
        /// </summary>
        DateTime Created { get; }

        /// <summary>
        /// Does the directory exist
        /// </summary>
        bool Exists { get; }

        /// <summary>
        /// Full path to the directory
        /// </summary>
        string FullName { get; }

        /// <summary>
        /// When it was last modified
        /// </summary>
        DateTime Modified { get; }

        /// <summary>
        /// Name of the directory
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Parent directory
        /// </summary>
        IDirectory Parent { get; }

        /// <summary>
        /// Root directory
        /// </summary>
        IDirectory Root { get; }

        /// <summary>
        /// Size of the contents of the directory in bytes
        /// </summary>
        long Size { get; }

        /// <summary>
        /// Copies the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to copy to</param>
        /// <param name="options">Copy options</param>
        IDirectory CopyTo(IDirectory directory, CopyOptions options = CopyOptions.CopyAlways);

        /// <summary>
        /// Creates the directory if it does not currently exist
        /// </summary>
        void Create();

        /// <summary>
        /// Deletes the directory
        /// </summary>
        void Delete();

        /// <summary>
        /// Enumerates sub directories (defaults to top level sub directories)
        /// </summary>
        /// <param name="searchPattern">Search pattern to use</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of directories</returns>
        IEnumerable<IDirectory> EnumerateDirectories(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Enumerates sub directories (defaults to top level sub directories)
        /// </summary>
        /// <param name="predicate">Predicate used to filter directories</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of directories</returns>
        IEnumerable<IDirectory> EnumerateDirectories(Predicate<IDirectory> predicate, SearchOption options = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Enumerates files within the directory (defaults to top level directory and not the sub directories)
        /// </summary>
        /// <param name="searchPattern">Search pattern to use</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of files</returns>
        IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Enumerates files within the directory (defaults to top level directory and not the sub directories)
        /// </summary>
        /// <param name="predicate">Predicate used to filter files</param>
        /// <param name="options">Search options to use</param>
        /// <returns>The list of files</returns>
        IEnumerable<IFile> EnumerateFiles(Predicate<IFile> predicate, SearchOption options = SearchOption.TopDirectoryOnly);

        /// <summary>
        /// Moves the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        IDirectory MoveTo(IDirectory directory);

        /// <summary>
        /// Renames the directory
        /// </summary>
        /// <param name="name">The new name of the directory</param>
        void Rename(string name);
    }
}