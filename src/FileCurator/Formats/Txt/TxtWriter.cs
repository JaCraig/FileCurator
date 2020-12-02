/*
Copyright 2017 James Craig

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

using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileCurator.Formats.Txt
{
    /// <summary>
    /// Txt Writer
    /// </summary>
    /// <seealso cref="IGenericFileWriter"/>
    public class TxtWriter : IGenericFileWriter
    {
        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public bool Write(Stream writer, IGenericFile file)
        {
            var TempData = Encoding.UTF8.GetBytes(file.ToString());
            writer.Write(TempData, 0, TempData.Length);
            return true;
        }

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public async Task<bool> WriteAsync(Stream writer, IGenericFile file)
        {
            var TempData = Encoding.UTF8.GetBytes(file.ToString());
            await writer.WriteAsync(TempData, 0, TempData.Length).ConfigureAwait(false);
            return true;
        }
    }
}