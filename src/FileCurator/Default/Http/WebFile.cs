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
using FileCurator.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace FileCurator.Default
{
    /// <summary>
    /// Web file
    /// </summary>
    /// <seealso cref="FileBase{Uri, WebFile}"/>
    public class WebFile : FileBase<Uri, WebFile>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WebFile()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="client">The client.</param>
        /// <param name="credentials">The credentials.</param>
        public WebFile(string path, HttpClient? client, Credentials? credentials = null)
            : this(string.IsNullOrEmpty(path) ? null : new Uri(path), client, credentials)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directory">Internal directory</param>
        /// <param name="client">The client.</param>
        /// <param name="credentials">The credentials.</param>
        public WebFile(Uri? directory, HttpClient? client, Credentials? credentials = null)
            : base(directory, credentials)
        {
            Client = client;
        }

        /// <summary>
        /// Time accessed (Just returns now)
        /// </summary>
        public override DateTime Accessed => DateTime.Now;

        /// <summary>
        /// Time created (Just returns now)
        /// </summary>
        public override DateTime Created => DateTime.Now;

        /// <summary>
        /// Directory base path
        /// </summary>
        public override IDirectory? Directory => InternalFile is null ? null : new WebDirectory(InternalFile.AbsolutePath.Left(InternalFile.AbsolutePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1), Client, Credentials);

        /// <summary>
        /// Does it exist? Always true.
        /// </summary>
        public override bool Exists { get; } = true;

        /// <summary>
        /// Extension (always empty)
        /// </summary>
        public override string Extension { get; } = string.Empty;

        /// <summary>
        /// Full path
        /// </summary>
        public override string FullName => InternalFile?.ToString() ?? string.Empty;

        /// <summary>
        /// Size of the file (always 0)
        /// </summary>
        public override long Length { get; } = 0;

        /// <summary>
        /// Time modified (just returns now)
        /// </summary>
        public override DateTime Modified => DateTime.Now;

        /// <summary>
        /// Absolute path of the file (same as FullName)
        /// </summary>
        public override string Name => InternalFile?.AbsolutePath ?? string.Empty;

        /// <summary>
        /// Gets the client.
        /// </summary>
        /// <value>The client.</value>
        private HttpClient? Client { get; }

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        public override IFile CopyTo(IDirectory directory, bool overwrite)
        {
            if (directory is null || string.IsNullOrEmpty(directory.FullName))
                return this;
            var File = new FileInfo(directory.FullName + Path.DirectorySeparatorChar + Name.Right(Name.Length - (Name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
            if (!File.Exists || overwrite)
            {
                File.Write(ReadBinary());
                return File;
            }
            return this;
        }

        /// <summary>
        /// Delete (does nothing)
        /// </summary>
        /// <returns>Any response for deleting the resource (usually FTP, HTTP, etc)</returns>
        public override string Delete()
        {
            if (InternalFile is null)
                return string.Empty;
            var Request = new HttpRequestMessage(HttpMethod.Delete, InternalFile);
            SetupData(Request, "");
            return SendRequest(Client, Request);
        }

        /// <summary>
        /// Moves the file (not used)
        /// </summary>
        /// <param name="directory">Not used</param>
        public override IFile MoveTo(IDirectory directory)
        {
            if (directory is null || !Exists || string.IsNullOrEmpty(directory.FullName))
                return this;
            var TempFile = new FileInfo(directory.FullName + Path.DirectorySeparatorChar + Name.Right(Name.Length - (Name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
            TempFile.Write(ReadBinary());
            Delete();
            return TempFile;
        }

        /// <summary>
        /// Reads the web page
        /// </summary>
        /// <returns>The content as a string</returns>
        public override string Read()
        {
            if (InternalFile is null)
                return string.Empty;
            var Request = new HttpRequestMessage(HttpMethod.Get, InternalFile);
            SetupData(Request, "");
            return SendRequest(Client, Request);
        }

        /// <summary>
        /// Reads the web page
        /// </summary>
        /// <returns>The content as a byte array</returns>
        public override byte[] ReadBinary()
        {
            return InternalFile is null ? Array.Empty<byte>() : Read().ToByteArray();
        }

        /// <summary>
        /// Renames the file (not used)
        /// </summary>
        /// <param name="newName">Not used</param>
        public override IFile Rename(string newName) => this;

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="content">Not used</param>
        /// <param name="mode">Not used</param>
        /// <param name="encoding">Not used</param>
        /// <returns>The result of the write or original content</returns>
        public override string Write(string content, FileMode mode = FileMode.Create, Encoding? encoding = null)
        {
            if (InternalFile is null)
                return "";
            var Method = (mode == FileMode.Append || mode == FileMode.Open) ? HttpMethod.Put : HttpMethod.Post;
            var Request = new HttpRequestMessage(Method, InternalFile);
            SetupData(Request, content);
            return SendRequest(Client, Request);
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="content">Not used</param>
        /// <param name="mode">Not used</param>
        /// <returns>The result of the write or original content</returns>
        public override byte[] Write(byte[] content, FileMode mode = FileMode.Create) => Write(content.ToString(Encoding.UTF8), mode).ToByteArray();

        /// <summary>
        /// Sends the request to the URL specified
        /// </summary>
        /// <param name="client">The client.</param>
        /// <param name="request">The web request object</param>
        /// <returns>The string returned by the service</returns>
        private static string SendRequest(HttpClient? client, HttpRequestMessage? request)
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
        private static void SetupData(HttpRequestMessage? request, string data)
        {
            if (request is null || string.IsNullOrEmpty(data))
                return;
            var ByteData = data.ToByteArray();
            request.Content = new ByteArrayContent(ByteData);
        }
    }
}