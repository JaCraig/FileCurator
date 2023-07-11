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
using System;
using System.IO;

namespace FileCurator.Default
{
    /// <summary>
    /// Relative local file system
    /// </summary>
    public class RelativeLocalFileSystem : LocalFileSystemBase
    {
        /// <summary>
        /// Name of the file system
        /// </summary>
        public override string Name { get; } = "Relative Local";

        /// <summary>
        /// Gets the order (lower numbers occur first).
        /// </summary>
        /// <value>The order.</value>
        public override int Order { get; } = int.MaxValue;

        /// <summary>
        /// Relative starter
        /// </summary>
        protected override string HandleRegexString { get; } = @"^[~|\.]";

        /// <summary>
        /// Gets the absolute path of the variable passed in
        /// </summary>
        /// <param name="path">Path to convert to absolute</param>
        /// <returns>The absolute path of the path passed in</returns>
        protected override string AbsolutePath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return path;
            path = path.Replace('/', Path.DirectorySeparatorChar);
            var BaseDirectory = new System.IO.DirectoryInfo(".").FullName;
            var ParentDirectory = new LocalDirectory(BaseDirectory).Parent.FullName;
            if (path.StartsWith("..\\", StringComparison.Ordinal))
            {
                return ParentDirectory + path.Remove(0, 2);
            }
            else if (path.StartsWith(".\\", StringComparison.Ordinal))
            {
                return BaseDirectory + path.Remove(0, 1);
            }
            else if (path.StartsWith("~\\", StringComparison.Ordinal))
            {
                return BaseDirectory + path.Remove(0, 1);
            }
            return path;
        }
    }
}