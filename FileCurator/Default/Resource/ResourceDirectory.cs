﻿/*
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
using BigBook.ExtensionMethods;
using FileCurator.BaseClasses;
using FileCurator.HelperMethods;
using FileCurator.Interfaces;
using Microsoft.Extensions.DependencyInjection;
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
        /// <param name="credentials">The credentials.</param>
        public ResourceDirectory(string path, Credentials? credentials = null)
            : base(path, credentials)
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
        public override string FullName => AssemblyFrom is null ? $"resource://{Resource}" : $"resource://{AssemblyFrom.GetName()?.Name}/{Resource}";

        /// <summary>
        /// returns now
        /// </summary>
        public override DateTime Modified => DateTime.Now;

        /// <summary>
        /// Full path
        /// </summary>
        public override string Name => string.IsNullOrEmpty(InternalDirectory) ? "" : SplitPathRegex.Match(InternalDirectory).Groups["Assembly"].Value;

        /// <summary>
        /// Full path
        /// </summary>
        public override IDirectory? Parent { get; } = null;

        /// <summary>
        /// Root
        /// </summary>
        public override IDirectory? Root => AssemblyFrom is null ? null : new ResourceDirectory("resource://" + AssemblyFrom.GetName()?.Name + "/", Credentials);

        /// <summary>
        /// Size (returns 0)
        /// </summary>
        public override long Size { get; } = 0;

        /// <summary>
        /// Gets the split path regex.
        /// </summary>
        /// <value>The split path regex.</value>
        private static Regex SplitPathRegex { get; } = new Regex("^resource://((?<Assembly>[^/]*)/)?(?<FileName>.*)", RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets or sets the assembly this is from.
        /// </summary>
        /// <value>The assembly this is from.</value>
        private Assembly? AssemblyFrom
        {
            get
            {
                if (string.IsNullOrEmpty(InternalDirectory))
                    return null;
                var AssemblyName = SplitPathRegex.Match(InternalDirectory).Groups["Assembly"].Value;
                if (string.IsNullOrEmpty(AssemblyName))
                    return null;
                return Services.ServiceProvider?.GetService<IEnumerable<Assembly>>()?.FirstOrDefault(x => x.GetName()?.Name == AssemblyName);
            }
        }

        /// <summary>
        /// Gets the resource.
        /// </summary>
        /// <value>The resource.</value>
        private string Resource
        {
            get
            {
                if (string.IsNullOrEmpty(InternalDirectory))
                    return "";
                var Match = SplitPathRegex.Match(InternalDirectory)?.Groups["FileName"];
                if (Match is null)
                    return "";
                var ReturnValue = Match.Success ? Match.Value.Replace(new string(new char[] { Path.DirectorySeparatorChar }), "/").Replace("/", ".") : "";
                if (ReturnValue.EndsWith(".", StringComparison.Ordinal))
                    return ReturnValue.Left(ReturnValue.Length - 1);
                return ReturnValue;
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <returns>This</returns>
        public override IDirectory Create() => this;

        /// <summary>
        /// Not used
        /// </summary>
        public override IDirectory Delete() => this;

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IDirectory> EnumerateDirectories(string searchPattern, SearchOption options = SearchOption.TopDirectoryOnly) => Array.Empty<IDirectory>();

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="searchPattern"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override IEnumerable<IFile> EnumerateFiles(string searchPattern = "*", SearchOption options = SearchOption.TopDirectoryOnly)
        {
            if (AssemblyFrom is null)
                yield break;
            foreach (var TempFile in AssemblyFrom.GetManifestResourceNames() ?? Array.Empty<string>())
            {
                var TempResource = new ResourceFile($"resource://{AssemblyFrom.GetName()?.Name}/{TempFile}", Credentials);
                if (TempResource.FullName.StartsWith(FullName, StringComparison.OrdinalIgnoreCase))
                {
                    yield return TempResource;
                }
            }
        }

        /// <summary>
        /// Not used
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public override IDirectory Rename(string name) => this;
    }
}