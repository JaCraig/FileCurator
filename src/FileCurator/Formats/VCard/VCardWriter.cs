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

using BigBook;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.Interfaces;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace FileCurator.Formats.VCard
{
    /// <summary>
    /// VCard writer
    /// </summary>
    /// <seealso cref="IGenericFileWriter"/>
    public class VCardWriter : IGenericFileWriter
    {
        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public bool Write(Stream writer, IGenericFile file)
        {
            if (file is ICard FileCard)
                return WriterCard(writer, FileCard);
            return false;
        }

        /// <summary>
        /// Writes the file to the specified writer.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="file">The file.</param>
        /// <returns>True if it writes successfully, false otherwise.</returns>
        public Task<bool> WriteAsync(Stream writer, IGenericFile file)
        {
            if (file is ICard FileCard)
                return WriterCardAsync(writer, FileCard);
            return Task.FromResult(false);
        }

        /// <summary>
        /// Generates the card.
        /// </summary>
        /// <param name="fileCard">The file card.</param>
        /// <returns></returns>
        private static string GenerateCard(ICard fileCard)
        {
            return new StringBuilder().Append("BEGIN:VCARD\r\nVERSION:2.1\r\n")
                .AppendFormat(CultureInfo.CurrentCulture, "FN:{0}\r\n", fileCard.FullName)
                .AppendFormat(CultureInfo.CurrentCulture, "N:{0}\r\n", $"{fileCard.LastName};{fileCard.FirstName};{fileCard.MiddleName};{fileCard.Prefix};{fileCard.Suffix}")
                .AppendLine(fileCard.DirectDial.ToString(x => $"TEL;TYPE={x.Type}:{x.Number}", "\r\n"))
                .AppendLine(fileCard.Email.ToString(x => $"EMAIL;TYPE={x.Type}:{x.EmailAddress}", "\r\n"))
                .AppendLine(fileCard.Addresses.ToString(x => $"ADR;TYPE={x.Type}:;{x.Name};{x.Street};{x.City};{x.StateOrProvence};{x.ZipCode};{x.Country}", "\n"))
                .AppendFormat(CultureInfo.CurrentCulture, "TITLE:{0}\r\n", fileCard.Title)
                .AppendFormat(CultureInfo.CurrentCulture, "ORG:{0}\r\n", fileCard.Organization)
                .AppendFormat(CultureInfo.CurrentCulture, "URL:{0}\r\n", fileCard.Url)
                .AppendFormat(CultureInfo.CurrentCulture, "END:VCARD\r\n")
                .ToString();
        }

        /// <summary>
        /// Writers the card.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="fileCard">The file card.</param>
        /// <returns></returns>
        private bool WriterCard(Stream writer, ICard fileCard)
        {
            var Result = GenerateCard(fileCard);
            var ByteData = Encoding.UTF8.GetBytes(Result);
            writer.Write(ByteData, 0, ByteData.Length);
            return true;
        }

        /// <summary>
        /// Writers the card.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="fileCard">The file card.</param>
        /// <returns></returns>
        private async Task<bool> WriterCardAsync(Stream writer, ICard fileCard)
        {
            string Result = GenerateCard(fileCard);
            var ByteData = Encoding.UTF8.GetBytes(Result);
            await writer.WriteAsync(ByteData, 0, ByteData.Length).ConfigureAwait(false);
            return true;
        }
    }
}