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
using FileCurator.BaseClasses;
using FileCurator.Enums;
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;

namespace FileCurator.Default
{
    /// <summary>
    /// Directory class
    /// </summary>
    public class WebDirectory : DirectoryBase<Uri, WebDirectory>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WebDirectory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="client">The client.</param>
        /// <param name="credentials">The credentials.</param>
        public WebDirectory(string path, HttpClient? client, Credentials? credentials = null)
            : this(string.IsNullOrEmpty(path) ? null : new Uri(path), client, credentials)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directory">Internal directory</param>
        /// <param name="client">The client.</param>
        /// <param name="credentials">The credentials.</param>
        public WebDirectory(Uri? directory, HttpClient? client, Credentials? credentials = null)
            : base(directory, credentials)
        {
            Client = client;
        }

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Accessed => DateTime.Now;

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Created => DateTime.Now;

        /// <summary>
        /// returns true
        /// </summary>
        public override bool Exists { get; } = true;

        /// <summary>
        /// Full path
        /// </summary>
        public override string FullName => InternalDirectory?.ToString() ?? string.Empty;

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Modified => DateTime.Now;

        /// <summary>
        /// Full path
        /// </summary>
        public override string Name => InternalDirectory?.AbsolutePath ?? string.Empty;

        /// <summary>
        /// Full path
        /// </summary>
        public override IDirectory? Parent => InternalDirectory is null ? null : new WebDirectory(InternalDirectory.AbsolutePath.Left(InternalDirectory.AbsolutePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1), Client, Credentials);

        /// <summary>
        /// Root
        /// </summary>
        public override IDirectory? Root => InternalDirectory is null ? null : new WebDirectory(InternalDirectory.Scheme + "://" + InternalDirectory.Host, Client, Credentials);

        /// <summary>
        /// Size (returns 0)
        /// </summary>
        public override long Size { get; } = 0;

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        private HttpClient? Client { get; }

        /// <summary>
        /// Copies the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to copy to</param>
        /// <param name="options">Options</param>
        /// <returns>Newly created directory</returns>
        public override IDirectory CopyTo(IDirectory directory, CopyOptions options = CopyOptions.CopyAlways)
        {
            if (directory is null || InternalDirectory is null || string.IsNullOrEmpty(directory.FullName))
                return this;
            var TempName = Name;
            if (TempName == "/")
                TempName = "index.html";
            var NewDirectory = new FileInfo(directory.FullName + "\\" + TempName.Right(TempName.Length - (TempName.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
            var OldFile = new FileInfo(InternalDirectory.AbsoluteUri, Credentials);
            NewDirectory.Write(OldFile.Read(), FileMode.Create);
            return directory;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Create()
        {
            if (InternalDirectory is null)
                return this;
            var Request = new HttpRequestMessage(HttpMethod.Post, InternalDirectory);
            SetupData(Request, "");
            SendRequest(Client, Request);
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Delete()
        {
            if (InternalDirectory is null)
                return this;
            var Request = new HttpRequestMessage(HttpMethod.Delete, InternalDirectory);
            SetupData(Request, "");
            SendRequest(Client, Request);
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption options = SearchOption.TopDirectoryOnly) => Array.Empty<WebDirectory>();

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly) => Array.Empty<WebFile>();

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="name"></param>
        public override IDirectory Rename(string name) => this;

        /// <summary>
        /// Sends the request to the URL specified
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The web request object</param>
        /// <returns>The string returned by the service</returns>
        private static string SendRequest(HttpClient? client, HttpRequestMessage request)
        {
            if (request is null || client is null)
                return "";

            var Result = AsyncHelper.RunSync(() => client.SendAsync(request));
            if (Result.StatusCode != HttpStatusCode.OK)
            {
                Result.EnsureSuccessStatusCode();
                return "";
            }
            return AsyncHelper.RunSync(() => Result.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// Sets up any data that needs to be sent
        /// </summary>
        /// <param name="request">The web request object</param>
        /// <param name="data">Data to send with the request</param>
        private static void SetupData(HttpRequestMessage request, string data)
        {
            if (request is null || string.IsNullOrEmpty(data))
                return;
            var ByteData = data.ToByteArray();
            request.Content = new ByteArrayContent(ByteData);
        }
    }
}