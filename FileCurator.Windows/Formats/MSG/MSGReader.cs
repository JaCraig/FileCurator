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

using FileCurator.Formats.BaseClasses;
using FileCurator.Formats.Data;
using FileCurator.Formats.Data.Interfaces;
using FileCurator.Formats.MSG;
using System;
using System.IO;
using System.Linq;
using static FileCurator.Formats.MSG.OutlookStorage;

namespace FileCurator.Windows.Formats.MSG
{
    /// <summary>
    /// MSG Reader
    /// </summary>
    /// <seealso cref="FileCurator.Formats.Interfaces.IGenericFileReader{IMessage}"/>
    public class MSGReader : ReaderBaseClass<IMessage>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier => new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1, 0x00 };

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IMessage Read(Stream stream)
        {
            using (var message = new OutlookStorage.Message(stream))
            {
                var ReturnValue = new GenericEmail
                {
                    Content = message.BodyText,
                    Title = message.Subject,
                    From = message.From,
                    Sent = message.SentTime
                };
                AddRecipients(message, RecipientType.Unknown, x => ReturnValue.BCC.Add(x));
                AddRecipients(message, RecipientType.CC, x => ReturnValue.CC.Add(x));
                AddRecipients(message, RecipientType.To, x => ReturnValue.To.Add(x));
                return ReturnValue;
            }
        }

        /// <summary>
        /// Adds the recipients.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        /// <param name="action">The action.</param>
        private void AddRecipients(Message message, RecipientType type, Action<string> action)
        {
            var Result = message.Recipients.Where(x => x.Type == type).Select(x => x.Email);
            foreach (var Item in Result)
            {
                action(Item);
            }
        }
    }
}