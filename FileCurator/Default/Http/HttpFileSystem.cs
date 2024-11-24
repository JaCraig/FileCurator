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
using FileCurator.HelperMethods;
using FileCurator.Interfaces;

namespace FileCurator.Default
{
    /// <summary>
    /// HTTP file system
    /// </summary>
    public class HttpFileSystem : FileSystemBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpFileSystem"/> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public HttpFileSystem(InternalHttpClientFactory httpClientFactory)
        {
            HttpClientFactory = httpClientFactory;
        }

        /// <summary>
        /// Name of the file system
        /// </summary>
        public override string Name { get; } = "HTTP";

        /// <summary>
        /// Gets the order (lower numbers occur first).
        /// </summary>
        /// <value>The order.</value>
        public override int Order { get; } = int.MaxValue;

        /// <summary>
        /// Relative starter
        /// </summary>
        protected override string HandleRegexString { get; } = @"^https?://|^www\.";

        /// <summary>
        /// Gets the HTTP client factory.
        /// </summary>
        /// <value>The HTTP client factory.</value>
        private InternalHttpClientFactory HttpClientFactory { get; }

        /// <summary>
        /// Gets the directory representation for the directory
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The directory object</returns>
        public override IDirectory Directory(string path, Credentials? credentials = null)
        {
            return new WebDirectory(AbsolutePath(path), HttpClientFactory.GetClient(credentials), credentials);
        }

        /// <summary>
        /// Gets the class representation for the file
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="credentials">The credentials.</param>
        /// <returns>The file object</returns>
        public override IFile File(string path, Credentials? credentials = null)
        {
            return new WebFile(AbsolutePath(path), HttpClientFactory.GetClient(credentials), credentials);
        }

        /// <summary>
        /// Gets the absolute path of the variable passed in
        /// </summary>
        /// <param name="path">Path to convert to absolute</param>
        /// <returns>The absolute path of the path passed in</returns>
        protected override string AbsolutePath(string path) => path;

        /// <summary>
        /// Used to dispose of any resources
        /// </summary>
        protected override void Dispose(bool managed)
        {
        }
    }
}