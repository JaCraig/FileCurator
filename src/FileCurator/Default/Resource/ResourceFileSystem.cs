﻿/*
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

namespace FileCurator.Default
{
    /// <summary>
    /// Resource file system
    /// </summary>
    public class ResourceFileSystem : FileSystemBase
    {
        /// <summary>
        /// Name of the file system
        /// </summary>
        public override string Name => "Resource File System";

        /// <summary>
        /// Regex string used to determine if the file system can handle the path
        /// </summary>
        protected override string HandleRegexString => @"^resource://";

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="userName">User name to be used to access the directory (optional)</param>
        /// <param name="password">Password to be used to access the directory (optional)</param>
        /// <param name="domain">Domain of the user (optional)</param>
        /// <returns>The directory object</returns>
        public override Interfaces.IDirectory Directory(string path, string userName = "", string password = "", string domain = "")
        {
            return new ResourceDirectory(path, userName, password, domain);
        }

        /// <summary>
        /// Function to override in order to dispose objects
        /// </summary>
        public override void Dispose()
        {
        }

        /// <summary>
        /// Gets the class representation for the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="userName">User name to be used to access the file (optional)</param>
        /// <param name="password">Password to be used to access the file (optional)</param>
        /// <param name="domain">Domain of the user (optional)</param>
        /// <returns>The file object</returns>
        public override Interfaces.IFile File(string path, string userName = "", string password = "", string domain = "")
        {
            return new ResourceFile(path, userName, password, domain);
        }

        /// <summary>
        /// Gets the absolute path of the variable passed in
        /// </summary>
        /// <param name="path">Path to convert to absolute</param>
        /// <returns>The absolute path of the path passed in</returns>
        protected override string AbsolutePath(string path)
        {
            return path;
        }
    }
}