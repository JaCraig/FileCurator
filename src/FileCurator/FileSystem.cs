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

using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileCurator
{
    /// <summary>
    /// Main class for FileCurator
    /// </summary>
    public class FileSystem : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileSystems">The file systems.</param>
        public FileSystem(IEnumerable<IFileSystem> fileSystems)
        {
            fileSystems ??= Array.Empty<IFileSystem>();
            FileSystems = fileSystems.Where(x => x.GetType().Assembly != typeof(FileSystem).Assembly)
                                          .ToDictionary(x => x.Name);
            foreach (var FileSystem in fileSystems.Where(x => x.GetType().Assembly == typeof(FileSystem).Assembly))
            {
                if (!FileSystems.ContainsKey(FileSystem.Name))
                    FileSystems.Add(FileSystem.Name, FileSystem);
            }
        }

        /// <summary>
        /// File systems that the library can use
        /// </summary>
        protected IDictionary<string, IFileSystem> FileSystems { get; }

        /// <summary>
        /// Gets the file system by name
        /// </summary>
        /// <param name="name">Name of the file system</param>
        /// <returns>The file system specified</returns>
        public IFileSystem this[string name] => FileSystems[name];

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The directory object</returns>
        public IDirectory? Directory(string path, Credentials? credentials = null)
        {
            return FindSystem(path)?.Directory(path, credentials);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the class representation for the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The file object</returns>
        public IFile? File(string path, Credentials? credentials = null)
        {
            var FileSystem = FindSystem(path);
            return FileSystem?.File(path, credentials);
        }

        /// <summary>
        /// Outputs the file system information in string format
        /// </summary>
        /// <returns>The list of file systems that are available</returns>
        public override string ToString()
        {
            var Builder = new StringBuilder();
            Builder.Append("File systems: ");
            var Splitter = "";
            foreach (var FileSystem in FileSystems)
            {
                Builder.AppendFormat("{0}{1}", Splitter, FileSystem.Key);
                Splitter = ",";
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool managed)
        {
            if (FileSystems is null)
                return;
            foreach (IDisposable FileSystem in FileSystems.Values)
            {
                FileSystem.Dispose();
            }
            FileSystems.Clear();
        }

        /// <summary>
        /// Finds a file system compatible with the path
        /// </summary>
        /// <param name="path">Path to search for</param>
        /// <returns>The file system associated with the path</returns>
        protected IFileSystem? FindSystem(string path)
        {
            return FileSystems?.Values.OrderBy(x => x.Order).FirstOrDefault(x => x.CanHandle(path));
        }
    }
}