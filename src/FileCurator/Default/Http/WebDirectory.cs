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
using FileCurator.Enums;
using FileCurator.HelperMethods;
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

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
        /// <param name="domain">Domain of the user (optional)</param>
        /// <param name="password">Password to be used to access the directory (optional)</param>
        /// <param name="userName">User name to be used to access the directory (optional)</param>
        public WebDirectory(string path, string userName = "", string password = "", string domain = "")
            : this(string.IsNullOrEmpty(path) ? null : new Uri(path), userName, password, domain)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directory">Internal directory</param>
        /// <param name="domain">Domain of the user (optional)</param>
        /// <param name="password">Password to be used to access the directory (optional)</param>
        /// <param name="userName">User name to be used to access the directory (optional)</param>
        public WebDirectory(Uri directory, string userName = "", string password = "", string domain = "")
            : base(directory, userName, password, domain)
        {
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
        public override bool Exists => true;

        /// <summary>
        /// Full path
        /// </summary>
        public override string FullName => InternalDirectory?.AbsolutePath ?? "";

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Modified => DateTime.Now;

        /// <summary>
        /// Full path
        /// </summary>
        public override string Name => InternalDirectory?.AbsolutePath ?? "";

        /// <summary>
        /// Full path
        /// </summary>
        public override IDirectory Parent => InternalDirectory == null ? null : new WebDirectory((string)InternalDirectory.AbsolutePath.Left(InternalDirectory.AbsolutePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1), UserName, Password, Domain);

        /// <summary>
        /// Root
        /// </summary>
        public override IDirectory Root => InternalDirectory == null ? null : new WebDirectory(InternalDirectory.Scheme + "://" + InternalDirectory.Host, UserName, Password, Domain);

        /// <summary>
        /// Size (returns 0)
        /// </summary>
        public override long Size => 0;

        /// <summary>
        /// Copies the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to copy to</param>
        /// <param name="options">Options</param>
        /// <returns>Newly created directory</returns>
        public override IDirectory CopyTo(IDirectory directory, CopyOptions options = CopyOptions.CopyAlways)
        {
            if (directory == null)
                return this;
            string TempName = Name;
            if (TempName == "/")
                TempName = "index.html";
            var NewDirectory = new FileInfo(directory.FullName + "\\" + TempName.Right(TempName.Length - (TempName.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), UserName, Password, Domain);
            var OldFile = new FileInfo(InternalDirectory.AbsoluteUri, UserName, Password, Domain);
            NewDirectory.Write(OldFile.Read(), FileMode.Create);
            return directory;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Create()
        {
            var Request = WebRequest.Create(InternalDirectory) as HttpWebRequest;
            Request.Method = "POST";
            Request.ContentType = "text/xml";
            SetupData(Request, "");
            SetupCredentials(Request);
            SendRequest(Request);
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Delete()
        {
            if (InternalDirectory == null)
                return this;
            var Request = WebRequest.Create(InternalDirectory) as HttpWebRequest;
            Request.Method = "DELETE";
            Request.ContentType = "text/xml";
            SetupData(Request, "");
            SetupCredentials(Request);
            SendRequest(Request);
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption options = SearchOption.TopDirectoryOnly)
        {
            return new List<WebDirectory>();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            return new List<WebFile>();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="name"></param>
        public override IDirectory Rename(string name)
        {
            return this;
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
            using (HttpWebResponse Response = request.GetResponseAsync().Result as HttpWebResponse)
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
            using (Stream RequestStream = request.GetRequestStreamAsync().Result)
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
            if (!string.IsNullOrEmpty(UserName) && !string.IsNullOrEmpty(Password))
            {
                request.Credentials = new NetworkCredential(UserName, Password);
            }
        }
    }
}