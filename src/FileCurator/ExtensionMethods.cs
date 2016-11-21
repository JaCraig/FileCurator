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

using FileCurator.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace FileCurator
{
    /// <summary>
    /// File extensions
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExtensionMethods
    {
        /// <summary>
        /// Deletes a list of files
        /// </summary>
        /// <param name="Files">List of files</param>
        public static void Delete(this IEnumerable<IFile> Files)
        {
            if (Files == null)
                return;
            Parallel.ForEach(Files, x => x.Delete());
        }

        /// <summary>
        /// Deletes a list of directories
        /// </summary>
        /// <param name="Directories">Directories to delete</param>
        public static void Delete(this IEnumerable<IDirectory> Directories)
        {
            if (Directories == null)
                return;
            Parallel.ForEach(Directories, x => x.Delete());
        }
    }
}