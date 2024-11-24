using BigBook;
using BigBook.ExtensionMethods;
using FileCurator.BaseClasses;
using FileCurator.Enums;
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace FileCurator.Default.Ftp
{
    /// <summary>
    /// Ftp directory
    /// </summary>
    /// <seealso cref="DirectoryBase{Uri, FtpDirectory}"/>
    public class FtpDirectory : DirectoryBase<Uri, FtpDirectory>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FtpDirectory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="credentials">The credentials.</param>
        public FtpDirectory(string path, Credentials? credentials = null)
            : this(string.IsNullOrEmpty(path) ? null : new Uri(path), credentials)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="directory">Internal directory</param>
        /// <param name="credentials">The credentials.</param>
        public FtpDirectory(Uri? directory, Credentials? credentials = null)
            : base(directory, credentials)
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
        public override IDirectory? Parent => InternalDirectory is null ? null : new FtpDirectory(InternalDirectory.AbsolutePath.Left(InternalDirectory.AbsolutePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1), Credentials);

        /// <summary>
        /// Root
        /// </summary>
        public override IDirectory? Root => InternalDirectory is null ? null : new FtpDirectory(InternalDirectory.Scheme + "://" + InternalDirectory.Host, Credentials);

        /// <summary>
        /// Size (returns 0)
        /// </summary>
        public override long Size { get; } = 0;

        /// <summary>
        /// Copies the directory to the specified parent directory
        /// </summary>
        /// <param name="directory">Directory to copy to</param>
        /// <param name="options">Options</param>
        /// <returns>Newly created directory</returns>
        public override IDirectory CopyTo(IDirectory directory, CopyOptions options = CopyOptions.CopyAlways)
        {
            if (directory is null || string.IsNullOrEmpty(directory.FullName))
                return this;
            var TempName = Name;
            var NewDirectory = new FileInfo(directory.FullName + Path.DirectorySeparatorChar + TempName.Right(TempName.Length - (TempName.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1)), Credentials);
            var OldFile = new FileInfo(InternalDirectory.AbsoluteUri, Credentials);
            NewDirectory.Write(OldFile.Read(), FileMode.Create);
            return directory;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Create()
        {
            if (!(WebRequest.Create(InternalDirectory) is FtpWebRequest Request))
                return this;
            Request.Method = WebRequestMethods.Ftp.MakeDirectory;
            SetupData(Request, null);
            SetupCredentials(Request);
            SendRequest(Request);
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Delete()
        {
            if (!(WebRequest.Create(InternalDirectory) is FtpWebRequest Request))
                return this;
            Request.Method = WebRequestMethods.Ftp.RemoveDirectory;
            SetupData(Request, null);
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
            if (!(WebRequest.Create(InternalDirectory) is FtpWebRequest Request))
                return Array.Empty<IDirectory>();
            Request.Method = WebRequestMethods.Ftp.ListDirectory;
            SetupData(Request, null);
            SetupCredentials(Request);
            var Data = SendRequest(Request);
            var Folders = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            SetupData(Request, null);
            SetupCredentials(Request);
            Data = SendRequest(Request);
            var DetailedFolders = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var Directories = new List<IDirectory>();
            for (int i = 0; i < Folders.Length; i++)
            {
                var DetailedFolder = Array.Find(DetailedFolders, x => x.EndsWith(Folders[i], StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(DetailedFolder)
                    && DetailedFolder.StartsWith("d", StringComparison.OrdinalIgnoreCase)
                    && !DetailedFolder.EndsWith(".", StringComparison.OrdinalIgnoreCase))
                {
                    Directories.Add(new DirectoryInfo(FullName + "/" + Folders[i], Credentials));
                }
            }
            return Directories;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (!(WebRequest.Create(InternalDirectory) is FtpWebRequest Request))
                return this;
            Request.Method = WebRequestMethods.Ftp.ListDirectory;
            SetupData(Request, null);
            SetupCredentials(Request);
            var Data = SendRequest(Request);
            var Folders = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            Request.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            SetupData(Request, null);
            SetupCredentials(Request);
            Data = SendRequest(Request);
            var DetailedFolders = Data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

            var Directories = new List<IFile>();
            for (int i = 0; i < Folders.Length; i++)
            {
                var DetailedFolder = Array.Find(DetailedFolders, x => x.EndsWith(Folders[i], StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(DetailedFolder) && !DetailedFolder.StartsWith("d", StringComparison.OrdinalIgnoreCase))
                {
                    Directories.Add(new FileInfo(FullName + "/" + Folders[i], Credentials));
                }
            }
            return Directories;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="name"></param>
        public override IDirectory Rename(string name)
        {
            if (!(WebRequest.Create(InternalDirectory) is FtpWebRequest Request))
                return this;
            Request.Method = WebRequestMethods.Ftp.Rename;
            Request.RenameTo = Name;
            SetupData(Request, null);
            SetupCredentials(Request);
            SendRequest(Request);
            InternalDirectory = new Uri(FullName + "/" + Name);
            return this;
        }

        /// <summary>
        /// Sends the request to the URL specified
        /// </summary>
        /// <param name="request">The web request object</param>
        /// <returns>The string returned by the service</returns>
        private static string SendRequest(FtpWebRequest request)
        {
            using FtpWebResponse? Response = request.GetResponse() as FtpWebResponse;
            using StreamReader Reader = new StreamReader(Response?.GetResponseStream());
            return Reader.ReadToEnd();
        }

        /// <summary>
        /// Sets up any credentials (basic authentication, for OAuth, please use the OAuth class to
        /// create the URL)
        /// </summary>
        /// <param name="request">The web request object</param>
        private void SetupCredentials(FtpWebRequest request)
        {
            if (Credentials is null)
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

        /// <summary>
        /// Sets up any data that needs to be sent
        /// </summary>
        /// <param name="request">The web request object</param>
        /// <param name="data">Data to send with the request</param>
        private void SetupData(FtpWebRequest request, byte[]? data)
        {
            request.UsePassive = true;
            request.KeepAlive = false;
            request.UseBinary = true;
            request.EnableSsl = FullName.ToUpperInvariant().StartsWith("FTPS", StringComparison.OrdinalIgnoreCase);
            if (data is null)
            {
                request.ContentLength = 0;
                return;
            }
            request.ContentLength = data.Length;
            using Stream RequestStream = request.GetRequestStream();
            RequestStream.Write(data, 0, data.Length);
        }
    }
}