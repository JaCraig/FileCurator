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
using BigBook.ExtensionMethods;
using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.HTML;
using MimeKit;
using System.IO;
using System.Linq;

namespace FileCurator.Formats.Mime
{
    /// <summary>
    /// MIME file reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{IMessage}"/>
    public class MimeReader : ReaderBaseClass<IMessage>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier { get; } = new byte[] { 0x46, 0x72, 0x6F, 0x6D, 0x3A, 0x20, 0x3C };

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IMessage Read(Stream stream)
        {
            var ReturnObject = new GenericEmail();
            if (stream is null)
                return ReturnObject;
            MimeMessage? Message = GetMessage(stream);
            if (Message is null)
                return ReturnObject;
            if (!string.IsNullOrEmpty(Message.HtmlBody))
            {
                using var TempStream = new MemoryStream(Message.HtmlBody.ToByteArray());
                ReturnObject.Content = new HTMLFormat().Read(TempStream).Content;
            }
            else
            {
                ReturnObject.Content = Message.TextBody;
            }
            ReturnObject.BCC.Add(Message.Bcc.Select(x => x.Name));
            ReturnObject.CC.Add(Message.Cc.Select(x => x.Name));
            ReturnObject.From = Message.From.Mailboxes.FirstOrDefault()?.Address ?? "";
            ReturnObject.Sent = Message.Date.UtcDateTime;
            ReturnObject.Title = Message.Subject;
            ReturnObject.To.Add(Message.To.Select(x => x.Name));
            return ReturnObject;
        }

        private static MimeMessage? GetMessage(Stream stream)
        {
            try
            {
                var Parser = new MimeParser(stream);
                return Parser.ParseMessage();
            }
            catch
            {
                return null;
            }
        }
    }
}