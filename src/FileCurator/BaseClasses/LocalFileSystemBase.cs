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

using FileCurator.Default;
using FileCurator.HelperMethods;
using FileCurator.Interfaces;

namespace FileCurator.BaseClasses
{
    /// <summary>
    /// Local file system base class
    /// </summary>
    /// <seealso cref="FileSystemBase"/>
    public abstract class LocalFileSystemBase : FileSystemBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        protected LocalFileSystemBase()
        {
        }

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The directory object</returns>
        public override IDirectory Directory(string path, Credentials? credentials = null)
        {
            return new LocalDirectory(AbsolutePath(path).RemoveIllegalDirectoryNameCharacters());
        }

        /// <summary>
        /// Gets the class representation for the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The file object</returns>
        public override IFile File(string path, Credentials? credentials = null)
        {
            return new LocalFile(AbsolutePath(path));
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        /// <param name="managed">
        /// <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release
        /// only unmanaged resources.
        /// </param>
        protected override void Dispose(bool managed)
        {
        }
    }
}