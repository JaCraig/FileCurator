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
using System.Text.RegularExpressions;

namespace FileCurator.BaseClasses
{
    /// <summary>
    /// File system base class
    /// </summary>
    public abstract class FileSystemBase : IDisposable, IFileSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected FileSystemBase()
            : base()
        {
            HandleRegex = new Regex(HandleRegexString, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Name of the file system
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Regex used to determine if the file system can handle the path
        /// </summary>
        protected Regex HandleRegex { get; private set; }

        /// <summary>
        /// Regex string used to determine if the file system can handle the path
        /// </summary>
        protected abstract string HandleRegexString { get; }

        /// <summary>
        /// Returns true if it can handle the path, false otherwise
        /// </summary>
        /// <param name="path">The path to check against</param>
        /// <returns>True if it can handle the path, false otherwise</returns>
        public bool CanHandle(string path)
        {
            return HandleRegex.IsMatch(path);
        }

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="domain">Domain of the user (optional)</param>
        /// <param name="password">Password to be used to access the directory (optional)</param>
        /// <param name="userName">User name to be used to access the directory (optional)</param>
        /// <returns>The directory object</returns>
        public abstract IDirectory Directory(string path, string userName = "", string password = "", string domain = "");

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Gets the class representation for the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="domain">Domain of the user (optional)</param>
        /// <param name="password">Password to be used to access the file (optional)</param>
        /// <param name="userName">User name to be used to access the file (optional)</param>
        /// <returns>The file object</returns>
        public abstract IFile File(string path, string userName = "", string password = "", string domain = "");

        /// <summary>
        /// Gets the absolute path of the variable passed in
        /// </summary>
        /// <param name="path">Path to convert to absolute</param>
        /// <returns>The absolute path of the path passed in</returns>
        protected abstract string AbsolutePath(string path);
    }
}