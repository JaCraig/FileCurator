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
    public class FileCurator : IDisposable
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileSystems">The file systems.</param>
        public FileCurator(IEnumerable<IFileSystem> fileSystems)
        {
            fileSystems = fileSystems ?? new List<IFileSystem>();
            FileSystems = fileSystems.Where(x => !x.GetType().Namespace.StartsWith("FILECURATOR", StringComparison.OrdinalIgnoreCase))
                                          .ToDictionary(x => x.Name);
            foreach (IFileSystem FileSystem in fileSystems.Where(x => x.GetType()
                                                                       .Namespace
                                                                       .StartsWith("FILECURATOR", StringComparison.OrdinalIgnoreCase)))
            {
                if (!FileSystems.ContainsKey(FileSystem.Name))
                    FileSystems.Add(FileSystem.Name, FileSystem);
            }
        }

        /// <summary>
        /// File systems that the library can use
        /// </summary>
        protected IDictionary<string, IFileSystem> FileSystems { get; private set; }

        /// <summary>
        /// Gets the file system by name
        /// </summary>
        /// <param name="name">Name of the file system</param>
        /// <returns>The file system specified</returns>
        public IFileSystem this[string name] { get { return FileSystems[name]; } }

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="domain">Domain of the user (optional)</param>
        /// <param name="password">Password to be used to access the directory (optional)</param>
        /// <param name="userName">User name to be used to access the directory (optional)</param>
        /// <returns>The directory object</returns>
        public IDirectory Directory(string path, string userName = "", string password = "", string domain = "")
        {
            var FileSystem = FindSystem(path);
            return FileSystem == null ? null : FileSystem.Directory(path, userName, password, domain);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (FileSystems != null)
            {
                foreach (IDisposable FileSystem in FileSystems.Values)
                {
                    FileSystem.Dispose();
                }
                FileSystems = null;
            }
        }

        /// <summary>
        /// Gets the class representation for the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="domain">Domain of the user (optional)</param>
        /// <param name="password">Password to be used to access the file (optional)</param>
        /// <param name="userName">User name to be used to access the file (optional)</param>
        /// <returns>The file object</returns>
        public IFile File(string path, string userName = "", string password = "", string domain = "")
        {
            var FileSystem = FindSystem(path);
            return FileSystem == null ? null : FileSystem.File(path, userName, password, domain);
        }

        /// <summary>
        /// Outputs the file system information in string format
        /// </summary>
        /// <returns>The list of file systems that are available</returns>
        public override string ToString()
        {
            var Builder = new StringBuilder();
            Builder.Append("File systems: ");
            string Splitter = "";
            foreach (var FileSystem in FileSystems)
            {
                Builder.AppendFormat("{0}{1}", Splitter, FileSystem.Key);
                Splitter = ",";
            }
            return Builder.ToString();
        }

        /// <summary>
        /// Finds a file system compatible with the path
        /// </summary>
        /// <param name="path">Path to search for</param>
        /// <returns>The file system associated with the path</returns>
        protected IFileSystem FindSystem(string path)
        {
            if (FileSystems == null)
                return null;
            return FileSystems.Values.FirstOrDefault(x => x.CanHandle(path));
        }
    }
}