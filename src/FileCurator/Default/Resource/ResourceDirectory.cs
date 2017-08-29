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
using FileCurator.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FileCurator.Default
{
    /// <summary>
    /// Directory class
    /// </summary>
    public class ResourceDirectory : DirectoryBase<string, ResourceDirectory>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ResourceDirectory()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="path">Path to the directory</param>
        /// <param name="domain">Domain of the user (optional)</param>
        /// <param name="password">Password to be used to access the directory (optional)</param>
        /// <param name="userName">User name to be used to access the directory (optional)</param>
        public ResourceDirectory(string path, string userName = "", string password = "", string domain = "")
            : base(path, userName, password, domain)
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
        public override string FullName => InternalDirectory;

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Modified => DateTime.Now;

        /// <summary>
        /// Full path
        /// </summary>
        public override string Name => SplitPathRegex.Match(InternalDirectory).Groups["Assembly"].Value;

        /// <summary>
        /// Full path
        /// </summary>
        public override IDirectory Parent => null;

        /// <summary>
        /// Root
        /// </summary>
        public override IDirectory Root => this;

        /// <summary>
        /// Size (returns 0)
        /// </summary>
        public override long Size => 0;

        /// <summary>
        /// Gets the split path regex.
        /// </summary>
        /// <value>The split path regex.</value>
        private static Regex SplitPathRegex => new Regex(@"^resource://(?<Assembly>[^/]*)/?", RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets or sets the assembly this is from.
        /// </summary>
        /// <value>The assembly this is from.</value>
        private Assembly AssemblyFrom
        {
            get
            {
                var AssemblyName = SplitPathRegex.Match(InternalDirectory).Groups["Assembly"].Value;
                var Assemblies = Canister.Builder.Bootstrapper.Resolve<IEnumerable<Assembly>>();
                return Assemblies.FirstOrDefault(x => x.GetName().Name == AssemblyName);
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <returns>This</returns>
        public override IDirectory Create()
        {
            return this;
        }

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Delete()
        {
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
            return new List<ResourceDirectory>();
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (AssemblyFrom == null)
                return new List<IFile>();
            var TempData = AssemblyFrom.GetManifestResourceNames() ?? new string[0];
            return TempData.Select(x => new ResourceFile(FullName + x, UserName, Password, Domain));
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override IDirectory Rename(string name)
        {
            return this;
        }
    }
}