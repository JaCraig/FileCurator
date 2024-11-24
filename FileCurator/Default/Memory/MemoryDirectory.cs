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
using BigBook.ExtensionMethods;
using FileCurator.BaseClasses;
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace FileCurator.Default.Memory
{
    /// <summary>
    /// Memory directory
    /// </summary>
    /// <seealso cref="DirectoryBase{String, MemoryDirectory}"/>
    public class MemoryDirectory : DirectoryBase<string, MemoryDirectory>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryDirectory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="credentials">The credentials.</param>
        public MemoryDirectory(string path, Credentials? credentials = null)
            : base(path?.Replace(Path.DirectorySeparatorChar, '/') ?? "", credentials)
        {
            Created = modified = accessed = DateTime.UtcNow;
        }

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Accessed => accessed;

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Created { get; }

        /// <summary>
        /// returns true
        /// </summary>
        public override bool Exists => exists;

        /// <summary>
        /// Full path
        /// </summary>
        public override string FullName => InternalDirectory ?? "";

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Modified => modified;

        /// <summary>
        /// Full path
        /// </summary>
        public override string Name => InternalDirectory ?? "";

        /// <summary>
        /// Full path
        /// </summary>
        public override IDirectory? Parent
        {
            get
            {
                if (string.IsNullOrEmpty(InternalDirectory))
                    return null;
                var TempValue = InternalDirectory.Replace("mem://", "").TrimEnd('/');
                return new MemoryDirectory("mem://" + TempValue.Left(TempValue.LastIndexOf('/')), Credentials);
            }
        }

        /// <summary>
        /// Root
        /// </summary>
        public override IDirectory Root => new MemoryDirectory("mem://", Credentials);

        /// <summary>
        /// Size (returns 0)
        /// </summary>
        public override long Size => Files.Sum(x => x.Length) + Directories.Sum(x => x.Size);

        /// <summary>
        /// The directories
        /// </summary>
        private readonly List<IDirectory> Directories = new List<IDirectory>();

        /// <summary>
        /// The file data
        /// </summary>
        private readonly List<IFile> Files = new List<IFile>();

        /// <summary>
        /// The accessed
        /// </summary>
        private DateTime accessed;

        /// <summary>
        /// The exists
        /// </summary>
        private bool exists;

        /// <summary>
        /// The modified
        /// </summary>
        private DateTime modified;

        /// <summary>
        /// Not used
        /// </summary>
        /// <returns>This</returns>
        public override IDirectory Create()
        {
            exists = true;
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Delete()
        {
            modified = DateTime.UtcNow;
            exists = false;
            for (int i = 0, FilesCount = Files.Count; i < FilesCount; i++)
            {
                var File = Files[i];
                File.Delete();
            }

            for (int i = 0, DirectoriesCount = Directories.Count; i < DirectoriesCount; i++)
            {
                var Directory = Directories[i];
                Directory.Delete();
            }

            Files.Clear();
            Directories.Clear();
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern">Search pattern</param>
        /// <param name="options">Search options</param>
        /// <returns>List of directories under this directory</returns>
        public override IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            accessed = DateTime.UtcNow;
            searchPattern = searchPattern.Replace("*", ".*");
            var SearchRegex = new Regex(searchPattern);
            for (int i = 0, DirectoriesCount = Directories.Count; i < DirectoriesCount; i++)
            {
                var Directory = Directories[i];
                if (SearchRegex.IsMatch(Directory.FullName))
                    yield return Directory;
                if (options == SearchOption.AllDirectories)
                {
                    foreach (var SubDirectory in Directory.EnumerateDirectories(searchPattern, options))
                    {
                        yield return SubDirectory;
                    }
                }
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            accessed = DateTime.UtcNow;
            searchPattern = searchPattern.Replace("*", ".*");
            var SearchRegex = new Regex(searchPattern);
            for (int i = 0, FilesCount = Files.Count; i < FilesCount; i++)
            {
                var File = Files[i];
                if (SearchRegex.IsMatch(File.FullName))
                    yield return File;
                if (options == SearchOption.AllDirectories)
                {
                    foreach (var Directory in Directories)
                    {
                        foreach (var SubFile in Directory.EnumerateFiles(searchPattern, options))
                        {
                            yield return SubFile;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Moves this directory under another directory
        /// </summary>
        /// <param name="directory">Directory to move to</param>
        /// <returns></returns>
        public override IDirectory MoveTo(IDirectory directory)
        {
            if (directory is null)
                return this;
            var ReturnValue = CopyTo(new DirectoryInfo(directory.FullName + Path.DirectorySeparatorChar + Name.Replace("mem://", "").Replace("/", "").Replace(new string(new char[] { Path.DirectorySeparatorChar }), ""), Credentials));
            Delete();
            return ReturnValue;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override IDirectory Rename(string name)
        {
            if (string.IsNullOrEmpty(name) || !Exists)
                return this;
            modified = DateTime.UtcNow;
            InternalDirectory = Parent + "/" + name;
            return this;
        }
    }
}