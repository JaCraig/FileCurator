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
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileCurator.Default
{
    /// <summary>
    /// Local directory class
    /// </summary>
    public class LocalDirectory : DirectoryBase<System.IO.DirectoryInfo, LocalDirectory>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LocalDirectory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the directory</param>
        public LocalDirectory(string path)
            : base(string.IsNullOrEmpty(path) ? null : new System.IO.DirectoryInfo(path))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directory">Internal directory</param>
        public LocalDirectory(System.IO.DirectoryInfo directory)
            : base(directory)
        {
        }

        /// <summary>
        /// Time accessed (UTC time)
        /// </summary>
        public override DateTime Accessed => InternalDirectory?.LastAccessTimeUtc ?? DateTime.Now;

        /// <summary>
        /// Time created (UTC time)
        /// </summary>
        public override DateTime Created => InternalDirectory?.CreationTimeUtc ?? DateTime.Now;

        /// <summary>
        /// Does the directory exist?
        /// </summary>
        public override bool Exists => InternalDirectory?.Exists ?? false;

        /// <summary>
        /// Full path of the directory
        /// </summary>
        public override string FullName => InternalDirectory?.FullName ?? "";

        /// <summary>
        /// Time modified (UTC time)
        /// </summary>
        public override DateTime Modified => InternalDirectory?.LastWriteTimeUtc ?? DateTime.Now;

        /// <summary>
        /// Name of the directory
        /// </summary>
        public override string Name => InternalDirectory?.Name ?? "";

        /// <summary>
        /// Parent directory
        /// </summary>
        public override IDirectory Parent => InternalDirectory == null ? null : new LocalDirectory(InternalDirectory.Parent);

        /// <summary>
        /// Root directory
        /// </summary>
        public override IDirectory Root => InternalDirectory == null ? null : new LocalDirectory(InternalDirectory.Root);

        /// <summary>
        /// Size of the directory
        /// </summary>
        public override long Size => Exists ? InternalDirectory.EnumerateFiles("*", SearchOption.AllDirectories).Sum(x => x.Length) : 0;

        /// <summary>
        /// Creates the directory
        /// </summary>
        public override IDirectory Create()
        {
            if (InternalDirectory == null)
                return this;
            InternalDirectory.Create();
            InternalDirectory.Refresh();
            return this;
        }

        /// <summary>
        /// Deletes the directory
        /// </summary>
        public override IDirectory Delete()
        {
            if (!Exists)
                return this;
            foreach (var TempFile in EnumerateFiles())
            {
                TempFile.Delete();
            }
            foreach (var TempDirectory in EnumerateDirectories())
            {
                TempDirectory.Delete();
            }
            InternalDirectory.Delete(true);
            InternalDirectory.Refresh();
            return this;
        }

        /// <summary>
        /// Enumerates directories under this directory
        /// </summary>
        /// <param name="searchPattern">Search pattern</param>
        /// <param name="options">Search options</param>
        /// <returns>List of directories under this directory</returns>
        public override IEnumerable<IDirectory> EnumerateDirectories(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!Exists)
                yield break;

            foreach (var SubDirectory in InternalDirectory.EnumerateDirectories(searchPattern, options))
            {
                yield return new LocalDirectory(SubDirectory);
            }
        }

        /// <summary>
        /// Enumerates files under this directory
        /// </summary>
        /// <param name="searchPattern">Search pattern</param>
        /// <param name="options">Search options</param>
        /// <returns>List of files under this directory</returns>
        public override IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!Exists)
                yield break;
            foreach (var TempFile in InternalDirectory.EnumerateFiles(searchPattern, options))
            {
                yield return new LocalFile(TempFile);
            }
        }

        /// <summary>
        /// Renames the directory
        /// </summary>
        /// <param name="name">Name of the new directory</param>
        public override IDirectory Rename(string name)
        {
            if (InternalDirectory == null || string.IsNullOrEmpty(name))
                return this;
            InternalDirectory.MoveTo(Parent.FullName + "\\" + name);
            InternalDirectory = new System.IO.DirectoryInfo(Parent.FullName + "\\" + name);
            return this;
        }
    }
}