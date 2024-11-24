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
    /// <seealso cref="IDisposable"/>
    /// <seealso cref="IFileSystem"/>
    public abstract class FileSystemBase : IDisposable, IFileSystem
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected FileSystemBase()
        {
            HandleRegex = new Regex(HandleRegexString, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// Name of the file system
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the order (lower numbers occur first).
        /// </summary>
        /// <value>The order.</value>
        public abstract int Order { get; }

        /// <summary>
        /// Regex used to determine if the file system can handle the path
        /// </summary>
        protected Regex HandleRegex { get; }

        /// <summary>
        /// Regex string used to determine if the file system can handle the path
        /// </summary>
        protected abstract string HandleRegexString { get; }

        /// <summary>
        /// Returns true if it can handle the path, false otherwise
        /// </summary>
        /// <param name="path">The path to check against</param>
        /// <returns>True if it can handle the path, false otherwise</returns>
        public bool CanHandle(string path) => !string.IsNullOrEmpty(path) && HandleRegex.IsMatch(path);

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The directory object</returns>
        public abstract IDirectory Directory(string path, Credentials? credentials = null);

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
        public abstract IFile File(string path, Credentials? credentials = null);

        /// <summary>
        /// Gets the absolute path of the variable passed in
        /// </summary>
        /// <param name="path">Path to convert to absolute</param>
        /// <returns>The absolute path of the path passed in</returns>
        protected abstract string AbsolutePath(string path);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="managed">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        protected abstract void Dispose(bool managed);
    }
}