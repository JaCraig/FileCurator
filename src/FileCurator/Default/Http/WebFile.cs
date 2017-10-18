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
using System;
using System.IO;
using System.Net;
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
        /// <param name="credentials">The credentials.</param>
        public WebFile(string path, Credentials credentials = null)
            : this(string.IsNullOrEmpty(path) ? null : new Uri(path), credentials)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File to use</param>
        /// <param name="credentials">The credentials.</param>
        public WebFile(Uri file, Credentials credentials)
            : base(file, credentials)
        {
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
        public override IDirectory Directory => InternalFile == null ? null : new WebDirectory((string)InternalFile.AbsolutePath.Left(InternalFile.AbsolutePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1), Credentials);

        /// <summary>
        /// Does it exist? Always true.
        /// </summary>
        public override bool Exists => true;

        /// <summary>
        /// Extension (always empty)
        /// </summary>
        public override string Extension => "";

        /// <summary>
        /// Full path
        /// </summary>
        public override string FullName => InternalFile?.ToString() ?? "";

        /// <summary>
        /// Size of the file (always 0)
        /// </summary>
        public override long Length => 0;

        /// <summary>
        /// Time modified (just returns now)
        /// </summary>
        public override DateTime Modified => DateTime.Now;

        /// <summary>
        /// Absolute path of the file (same as FullName)
        /// </summary>
        public override string Name => InternalFile?.AbsolutePath ?? "";

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        public override IFile CopyTo(IDirectory directory, bool overwrite)
        {
            if (directory == null)
                return this;
            var File = new FileInfo(directory.FullName + "\\" + Name.Right(Name.Length - (Name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
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
            if (InternalFile == null)
                return "";
            var Request = WebRequest.Create(InternalFile) as HttpWebRequest;
            Request.Method = "DELETE";
            Request.ContentType = "text/xml";
            SetupData(Request, "");
            SetupCredentials(Request);
            return SendRequest(Request);
        }

        /// <summary>
        /// Moves the file (not used)
        /// </summary>
        /// <param name="directory">Not used</param>
        public override IFile MoveTo(IDirectory directory)
        {
            if (directory == null || !Exists)
                return this;
            var TempFile = new FileInfo(directory.FullName + "\\" + Name.Right(Name.Length - (Name.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
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
            if (InternalFile == null)
                return "";
            var Request = WebRequest.Create(InternalFile) as HttpWebRequest;
            Request.Method = "GET";
            Request.ContentType = "text/xml";
            SetupData(Request, "");
            SetupCredentials(Request);
            return SendRequest(Request);
        }

        /// <summary>
        /// Reads the web page
        /// </summary>
        /// <returns>The content as a byte array</returns>
        public override byte[] ReadBinary()
        {
            if (InternalFile == null)
                return new byte[0];
            return Read().ToByteArray();
        }

        /// <summary>
        /// Renames the file (not used)
        /// </summary>
        /// <param name="newName">Not used</param>
        public override IFile Rename(string newName)
        {
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="content">Not used</param>
        /// <param name="mode">Not used</param>
        /// <param name="encoding">Not used</param>
        /// <returns>The result of the write or original content</returns>
        public override string Write(string content, FileMode mode = FileMode.Create, Encoding encoding = null)
        {
            var Request = WebRequest.Create(InternalFile) as HttpWebRequest;
            if (Request == null)
                return "";
            if (mode.HasFlag(FileMode.Append) || mode.HasFlag(FileMode.Open))
                Request.Method = "PUT";
            else if (mode.HasFlag(FileMode.Create) || mode.HasFlag(FileMode.CreateNew))
                Request.Method = "POST";
            Request.ContentType = "text/xml";
            SetupData(Request, content);
            SetupCredentials(Request);
            return SendRequest(Request);
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="content">Not used</param>
        /// <param name="mode">Not used</param>
        /// <returns>The result of the write or original content</returns>
        public override byte[] Write(byte[] content, FileMode mode = FileMode.Create)
        {
            return Write(content.ToString(Encoding.UTF8), mode).ToByteArray();
        }

        /// <summary>
        /// Sends the request to the URL specified
        /// </summary>
        /// <param name="request">The web request object</param>
        /// <returns>The string returned by the service</returns>
        private static string SendRequest(HttpWebRequest request)
        {
            if (request == null)
                return "";
            using (HttpWebResponse Response = request.GetResponseAsync().GetAwaiter().GetResult() as HttpWebResponse)
            {
                if (Response.StatusCode != HttpStatusCode.OK)
                    return "";
                using (StreamReader Reader = new StreamReader(Response.GetResponseStream()))
                {
                    return Reader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Sets up any data that needs to be sent
        /// </summary>
        /// <param name="request">The web request object</param>
        /// <param name="data">Data to send with the request</param>
        private static void SetupData(HttpWebRequest request, string data)
        {
            if (request == null)
                return;
            if (string.IsNullOrEmpty(data))
            {
                return;
            }
            var ByteData = data.ToByteArray();
            using (Stream RequestStream = request.GetRequestStreamAsync().GetAwaiter().GetResult())
            {
                RequestStream.Write(ByteData, 0, ByteData.Length);
            }
        }

        /// <summary>
        /// Sets up any credentials (basic authentication, for OAuth, please use the OAuth class to
        /// create the URL)
        /// </summary>
        /// <param name="request">The web request object</param>
        private void SetupCredentials(HttpWebRequest request)
        {
            if (Credentials == null)
                return;
            if (!string.IsNullOrEmpty(Credentials?.UserName) && !string.IsNullOrEmpty(Credentials?.Password))
            {
                if (!string.IsNullOrEmpty(Credentials?.Domain))
                    request.Credentials = new NetworkCredential(Credentials?.UserName, Credentials?.Password, Credentials?.Domain);
                else
                    request.Credentials = new NetworkCredential(Credentials?.UserName, Credentials?.Password);
            }
            else
            {
                request.UseDefaultCredentials = true;
            }
        }
    }
}