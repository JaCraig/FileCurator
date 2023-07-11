using BigBook;
using FileCurator.BaseClasses;
using FileCurator.Interfaces;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace FileCurator.Default.Ftp
{
    /// <summary>
    /// Ftp File
    /// </summary>
    /// <seealso cref="FileBase{Uri, FtpFile}"/>
    public class FtpFile : FileBase<Uri, FtpFile>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FtpFile()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="credentials">The credentials.</param>
        public FtpFile(string path, Credentials? credentials = null)
            : this(string.IsNullOrEmpty(path) ? null : new Uri(path), credentials)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="file">File to use</param>
        /// <param name="credentials">The credentials.</param>
        public FtpFile(Uri? file, Credentials? credentials)
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
        public override IDirectory? Directory => InternalFile is null ? null : new FtpDirectory(InternalFile.AbsolutePath.Left(InternalFile.AbsolutePath.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) - 1), Credentials);

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
            if (!(WebRequest.Create(InternalFile) is FtpWebRequest Request))
                return "";
            Request.Method = WebRequestMethods.Ftp.DeleteFile;
            SetupData(Request, null);
            SetupCredentials(Request);
            return SendRequest(Request);
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
            if (!(WebRequest.Create(InternalFile) is FtpWebRequest Request))
                return "";
            Request.Method = WebRequestMethods.Ftp.DownloadFile;
            SetupData(Request, null);
            SetupCredentials(Request);
            return SendRequest(Request);
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
        public override IFile Rename(string newName)
        {
            if (!(WebRequest.Create(InternalFile) is FtpWebRequest Request))
                return this;
            Request.Method = WebRequestMethods.Ftp.Rename;
            Request.RenameTo = newName;
            SetupData(Request, null);
            SetupCredentials(Request);
            SendRequest(Request);
            InternalFile = new Uri(Directory?.FullName + "/" + newName);
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="content">Not used</param>
        /// <param name="mode">Not used</param>
        /// <param name="encoding">Not used</param>
        /// <returns>The result of the write or original content</returns>
        public override string Write(string content, FileMode mode = FileMode.Create, Encoding? encoding = null)
        {
            return Write(content.ToByteArray(encoding), mode).ToString(Encoding.UTF8);
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="content">Not used</param>
        /// <param name="mode">Not used</param>
        /// <returns>The result of the write or original content</returns>
        public override byte[] Write(byte[] content, FileMode mode = FileMode.Create)
        {
            if (!(WebRequest.Create(InternalFile) is FtpWebRequest Request))
                return Array.Empty<byte>();
            Request.Method = WebRequestMethods.Ftp.UploadFile;
            SetupData(Request, content);
            SetupCredentials(Request);
            return SendRequest(Request).ToByteArray();
        }

        /// <summary>
        /// Sends the request to the URL specified
        /// </summary>
        /// <param name="request">The web request object</param>
        /// <returns>The string returned by the service</returns>
        private static string SendRequest(FtpWebRequest request)
        {
            using FtpWebResponse? Response = request.GetResponse() as FtpWebResponse;
            if (Response is null)
                return "";
            using StreamReader Reader = new StreamReader(Response.GetResponseStream());
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
            request.EnableSsl = Name.ToUpperInvariant().StartsWith("FTPS", StringComparison.OrdinalIgnoreCase);
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