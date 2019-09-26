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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace FileCurator.Default
{
    /// <summary>
    /// Basic Resource file class
    /// </summary>
    public class ResourceFile : FileBase<string, ResourceFile>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ResourceFile()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the file</param>
        /// <param name="credentials">The credentials.</param>
        public ResourceFile(string path, Credentials credentials = null)
            : base(path, credentials)
        {
        }

        /// <summary>
        /// Time accessed (Just returns now)
        /// </summary>
        public override DateTime Accessed
        {
            get
            {
                if (AssemblyFrom == null)
                    return DateTime.Now;

                return new System.IO.FileInfo(AssemblyFrom.Location).LastAccessTime;
            }
        }

        /// <summary>
        /// Time created (Just returns now)
        /// </summary>
        public override DateTime Created
        {
            get
            {
                if (AssemblyFrom == null)
                    return DateTime.Now;

                return new System.IO.FileInfo(AssemblyFrom.Location).CreationTime;
            }
        }

        /// <summary>
        /// Directory base path
        /// </summary>
        public override IDirectory Directory
        {
            get
            {
                return AssemblyFrom == null ? null : new ResourceDirectory("resource://" + AssemblyFrom.GetName().Name + "/", Credentials);
            }
        }

        /// <summary>
        /// Does it exist? Always true.
        /// </summary>
        public override bool Exists => AssemblyFrom?.GetManifestResourceStream(Resource) != null;

        /// <summary>
        /// Extension (always empty)
        /// </summary>
        public override string Extension => Resource.Right(Resource.Length - Resource.LastIndexOf('.'));

        /// <summary>
        /// Full path
        /// </summary>
        public override string FullName
        {
            get
            {
                return AssemblyFrom == null ? $"resource://{Resource}" : $"resource://{AssemblyFrom.GetName().Name}/{Resource}";
            }
        }

        /// <summary>
        /// Size of the file
        /// </summary>
        public override long Length
        {
            get
            {
                if (AssemblyFrom == null)
                    return 0;
                using (Stream TempStream = AssemblyFrom.GetManifestResourceStream(Resource))
                {
                    return TempStream.Length;
                }
            }
        }

        /// <summary>
        /// Time modified (just returns now)
        /// </summary>
        public override DateTime Modified
        {
            get
            {
                if (AssemblyFrom == null)
                    return DateTime.Now;

                return new System.IO.FileInfo(AssemblyFrom.Location).LastWriteTime;
            }
        }

        /// <summary>
        /// Absolute path of the file (same as FullName)
        /// </summary>
        public override string Name => Resource;

        /// <summary>
        /// Gets the split path regex.
        /// </summary>
        /// <value>The split path regex.</value>
        private static Regex SplitPathRegex { get; } = new Regex("^resource://((?<Assembly>[^/]*)/)?(?<FileName>.*)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets or sets the assembly this is from.
        /// </summary>
        /// <value>The assembly this is from.</value>
        private Assembly AssemblyFrom
        {
            get
            {
                return Canister.Builder.Bootstrapper.Resolve<IEnumerable<Assembly>>()
                                                    .FirstOrDefault(x => x.GetName().Name == SplitPathRegex.Match(InternalFile).Groups["Assembly"].Value);
            }
        }

        /// <summary>
        /// Gets or sets the resource.
        /// </summary>
        /// <value>The resource.</value>
        private string Resource
        {
            get
            {
                var Match = SplitPathRegex.Match(InternalFile).Groups["FileName"];
                return Match.Success ? Match.Value.Replace("\\", "/").Replace("/", ".") : "";
            }
        }

        /// <summary>
        /// Copies the file to another directory
        /// </summary>
        /// <param name="directory">Directory to copy the file to</param>
        /// <param name="overwrite">Should the file overwrite another file if found</param>
        /// <returns>The newly created file</returns>
        public override IFile CopyTo(IDirectory directory, bool overwrite)
        {
            if (directory == null || !Exists)
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
            return "";
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
        /// Reads the Resource page
        /// </summary>
        /// <returns>The content as a string</returns>
        public override string Read()
        {
            if (InternalFile == null || AssemblyFrom == null)
                return "";
            using (StreamReader TempStream = new StreamReader(AssemblyFrom.GetManifestResourceStream(Resource)))
            {
                return TempStream.ReadToEnd();
            }
        }

        /// <summary>
        /// Reads the Resource page
        /// </summary>
        /// <returns>The content as a byte array</returns>
        public override byte[] ReadBinary()
        {
            if (InternalFile == null || AssemblyFrom == null)
                return Array.Empty<byte>();
            using (Stream Reader = AssemblyFrom.GetManifestResourceStream(Resource))
            {
                byte[] Buffer = new byte[1024];
                using (MemoryStream Temp = new MemoryStream())
                {
                    while (true)
                    {
                        var Count = Reader.Read(Buffer, 0, Buffer.Length);
                        if (Count <= 0)
                            return Temp.ToArray();
                        Temp.Write(Buffer, 0, Count);
                    }
                }
            }
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
            return "";
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="content">Not used</param>
        /// <param name="mode">Not used</param>
        /// <returns>The result of the write or original content</returns>
        public override byte[] Write(byte[] content, FileMode mode = FileMode.Create)
        {
            return Array.Empty<byte>();
        }
    }
}