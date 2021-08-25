using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FileCurator.Formats.MSG
{
    /// <summary>
    /// Outlook storage
    /// </summary>
    /// <seealso cref="System.IDisposable"/>
    public class OutlookStorage : IDisposable
    {
        /*
         * Copyright (c) 2005 Oren J. Maurice <oymaurice@hazorea.org.il>
         *
         * Redistribution and use in source and binary forms, with or without modifica-
         * tion, are permitted provided that the following conditions are met:
         *
         *   1.  Redistributions of source code must retain the above copyright notice,
         *       this list of conditions and the following disclaimer.
         *
         *   2.  Redistributions in binary form must reproduce the above copyright
         *       notice, this list of conditions and the following disclaimer in the
         *       documentation and/or other materials provided with the distribution.
         *
         *   3.  The name of the author may not be used to endorse or promote products
         *       derived from this software without specific prior written permission.
         *
         * THIS SOFTWARE IS PROVIDED BY THE AUTHOR ``AS IS'' AND ANY EXPRESS OR IMPLIED
         * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MER-
         * CHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.  IN NO
         * EVENT SHALL THE AUTHOR BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPE-
         * CIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
         * PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS;
         * OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
         * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTH-
         * ERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED
         * OF THE POSSIBILITY OF SUCH DAMAGE.
         *
         * Alternatively, the contents of this file may be used under the terms of
         * the GNU General Public License version 2 (the "GPL"), in which case the
         * provisions of the GPL are applicable instead of the above. If you wish to
         * allow the use of your version of this file only under the terms of the
         * GPL and not to allow others to use your version of this file under the
         * BSD license, indicate your decision by deleting the provisions above and
         * replace them with the notice and other provisions required by the GPL. If
         * you do not delete the provisions above, a recipient may use your version
         * of this file under either the BSD or the GPL.
         */

        /// <summary>
        /// Initializes a new instance of the <see cref="OutlookStorage"/> class from a file.
        /// </summary>
        /// <param name="storageFilePath">The file to load.</param>
        private OutlookStorage(string storageFilePath)
        {
            //ensure provided file is an IStorage
            if (NativeMethods.StgIsStorageFile(storageFilePath) != 0)
            {
                throw new ArgumentException("The provided file is not a valid IStorage", nameof(storageFilePath));
            }

            //open and load IStorage from file
            NativeMethods.StgOpenStorage(storageFilePath, null, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_DENY_WRITE, IntPtr.Zero, 0, out NativeMethods.IStorage fileStorage);
            LoadStorage(fileStorage);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutlookStorage"/> class from a containing
        /// an IStorage.
        /// </summary>
        /// <param name="storageStream">The <see cref="Stream"/> containing an IStorage.</param>
        private OutlookStorage(Stream storageStream)
        {
            NativeMethods.IStorage? memoryStorage = null;
            NativeMethods.ILockBytes? memoryStorageBytes = null;
            try
            {
                //read stream into buffer
                byte[] buffer = new byte[storageStream.Length];
                storageStream.Read(buffer, 0, buffer.Length);

                //create a ILockBytes (unmanaged byte array) and write buffer into it
                NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                memoryStorageBytes.WriteAt(0, buffer, buffer.Length, null);

                //ensure provided stream data is an IStorage
                if (NativeMethods.StgIsStorageILockBytes(memoryStorageBytes) != 0)
                {
                    throw new ArgumentException("The provided stream is not a valid IStorage", nameof(storageStream));
                }

                //open and load IStorage on the ILockBytes
                NativeMethods.StgOpenStorageOnILockBytes(memoryStorageBytes, null, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_DENY_WRITE, IntPtr.Zero, 0, out memoryStorage);
                LoadStorage(memoryStorage);
            }
            catch
            {
                if (memoryStorage != null)
                {
                    Marshal.ReleaseComObject(memoryStorage);
                }
            }
            finally
            {
                if (memoryStorageBytes != null)
                {
                    Marshal.ReleaseComObject(memoryStorageBytes);
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OutlookStorage"/> class on the specified
        /// <see cref="NativeMethods.IStorage"/>.
        /// </summary>
        /// <param name="storage">The storage to create the <see cref="OutlookStorage"/> on.</param>
        private OutlookStorage(NativeMethods.IStorage? storage)
        {
            LoadStorage(storage);
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the is
        /// reclaimed by garbage collection.
        /// </summary>
        ~OutlookStorage()
        {
            Dispose();
        }

        /// <summary>
        /// Gets the received time.
        /// </summary>
        /// <value>The received time.</value>
        public DateTime? ReceivedTime => (DateTime?)GetMapiProperty(PR_MESSAGE_DELIVERY_TIME);

        /// <summary>
        /// Gets the sent time.
        /// </summary>
        /// <value>The sent time.</value>
        public DateTime? SentTime => (DateTime?)GetMapiProperty(PR_CLIENT_SUBMIT_TIME);

        /// <summary>
        /// Gets a value indicating whether this instance is the top level outlook message.
        /// </summary>
        /// <value><c>true</c> if this instance is the top level outlook message; otherwise, <c>false</c>.</value>
        private bool IsTopParent => parentMessage is null;

        /// <summary>
        /// Gets the top level outlook message from a sub message at any level.
        /// </summary>
        /// <value>The top level outlook message.</value>
        private OutlookStorage TopParent
        {
            get
            {
                if (parentMessage != null)
                {
                    return parentMessage.TopParent;
                }
                return this;
            }
        }

        /// <summary>
        /// The statistics for all streams in the IStorage associated with this instance.
        /// </summary>
        public Dictionary<string, System.Runtime.InteropServices.ComTypes.STATSTG> streamStatistics = new Dictionary<string, System.Runtime.InteropServices.ComTypes.STATSTG>();

        /// <summary>
        /// The statistics for all storgages in the IStorage associated with this instance.
        /// </summary>
        public Dictionary<string, System.Runtime.InteropServices.ComTypes.STATSTG> subStorageStatistics = new Dictionary<string, System.Runtime.InteropServices.ComTypes.STATSTG>();

        private const int ATTACH_EMBEDDED_MSG = 5;

        //attachment constants
        private const string ATTACH_STORAGE_PREFIX = "__attach_version1.0_#";

        private const int MAPI_CC = 2;

        private const int MAPI_TO = 1;

        //name id storage name in root storage
        private const string NAMEID_STORAGE = "__nameid_version1.0";

        private const string PR_ATTACH_CONTENT_ID = "3712";

        private const string PR_ATTACH_DATA = "3701";

        private const string PR_ATTACH_FILENAME = "3704";

        private const string PR_ATTACH_LONG_FILENAME = "3707";

        private const string PR_ATTACH_METHOD = "3705";

        private const string PR_BODY = "1000";

        private const string PR_CLIENT_SUBMIT_TIME = "0039";

        private const string PR_DISPLAY_NAME = "3001";

        private const string PR_EMAIL = "39FE";

        private const string PR_EMAIL_2 = "403E";

        private const string PR_MESSAGE_DELIVERY_TIME = "0E06";

        //not sure why but email address is in this property sometimes cant find any documentation on it
        private const string PR_RECIPIENT_TYPE = "0C15";

        private const string PR_RENDERING_POSITION = "370B";

        private const string PR_RTF_COMPRESSED = "1009";

        private const string PR_SENDER_NAME = "0C1A";

        //msg constants
        private const string PR_SUBJECT = "0037";

        //property stream constants
        private const string PROPERTIES_STREAM = "__properties_version1.0";

        private const int PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP = 8;

        private const int PROPERTIES_STREAM_HEADER_EMBEDED = 24;

        private const int PROPERTIES_STREAM_HEADER_TOP = 32;

        //recipient constants
        private const string RECIP_STORAGE_PREFIX = "__recip_version1.0_#";

        /// <summary>
        /// Indicates wether this instance has been disposed.
        /// </summary>
        private bool disposed;

        /// <summary>
        /// A reference to the parent message that this message may belong to.
        /// </summary>
        private OutlookStorage parentMessage;

        /// <summary>
        /// Header size of the property stream in the IStorage associated with this instance.
        /// </summary>
        private int propHeaderSize = PROPERTIES_STREAM_HEADER_TOP;

        /// <summary>
        /// The IStorage associated with this instance.
        /// </summary>
        private NativeMethods.IStorage? storage_;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                //ensure only disposed once
                disposed = true;

                //call virtual disposing method to let sub classes clean up
                Disposing();
                if (storage_ != null)
                {
                    //release COM storage object and suppress finalizer
                    ReferenceManager.RemoveItem(storage_);
                    Marshal.ReleaseComObject(storage_);
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Gets the raw value of the MAPI property.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The raw value of the MAPI property.</returns>
        public object? GetMapiProperty(string propIdentifier)
        {
            //try get prop value from stream or storage

            //if not found in stream or storage try get prop value from property stream

            return GetMapiPropertyFromStreamOrStorage(propIdentifier) ?? GetMapiPropertyFromPropertyStream(propIdentifier);
        }

        /// <summary>
        /// Gets the value of the MAPI property as a byte array.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a byte array.</returns>
        public byte[]? GetMapiPropertyBytes(string propIdentifier)
        {
            return GetMapiProperty(propIdentifier) as byte[];
        }

        /// <summary>
        /// Gets the value of the MAPI property as a short.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a short.</returns>
        public short GetMapiPropertyInt16(string propIdentifier)
        {
            return (short)GetMapiProperty(propIdentifier)!;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a integer.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a integer.</returns>
        public int GetMapiPropertyInt32(string propIdentifier)
        {
            return (int)GetMapiProperty(propIdentifier)!;
        }

        /// <summary>
        /// Gets the value of the MAPI property as a string.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property as a string.</returns>
        public string? GetMapiPropertyString(string propIdentifier)
        {
            return GetMapiProperty(propIdentifier) as string;
        }

        /// <summary>
        /// Gets the data in the specified stream as a string using the specifed encoding to decode
        /// the stream data.
        /// </summary>
        /// <param name="streamName">Name of the stream to get string data for.</param>
        /// <param name="streamEncoding">The encoding to decode the stream data with.</param>
        /// <returns>The data in the specified stream as a string.</returns>
        public string GetStreamAsString(string streamName, Encoding streamEncoding)
        {
            var streamReader = new StreamReader(new MemoryStream(GetStreamBytes(streamName)), streamEncoding);
            var streamContent = streamReader.ReadToEnd();
            streamReader.Close();

            return streamContent;
        }

        /// <summary>
        /// Gets the data in the specified stream as a byte array.
        /// </summary>
        /// <param name="streamName">Name of the stream to get data for.</param>
        /// <returns>A byte array containg the stream data.</returns>
        public byte[] GetStreamBytes(string streamName)
        {
            //get statistics for stream
            System.Runtime.InteropServices.ComTypes.STATSTG streamStatStg = streamStatistics[streamName];

            byte[] iStreamContent;
            System.Runtime.InteropServices.ComTypes.IStream? stream = null;
            try
            {
                //open stream from the storage
                stream = storage_?.OpenStream(streamStatStg.pwcsName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, 0);

                //read the stream into a managed byte array
                iStreamContent = new byte[streamStatStg.cbSize];
                stream?.Read(iStreamContent, iStreamContent.Length, IntPtr.Zero);
            }
            finally
            {
                if (stream != null)
                {
                    Marshal.ReleaseComObject(stream);
                }
            }

            //return the stream bytes
            return iStreamContent;
        }

        /// <summary>
        /// Gives sub classes the chance to free resources during object disposal.
        /// </summary>
        protected virtual void Disposing() { }

        /// <summary>
        /// Processes sub streams and storages on the specified storage.
        /// </summary>
        /// <param name="storage">The storage to get sub streams and storages for.</param>
        protected virtual void LoadStorage(NativeMethods.IStorage? storage)
        {
            storage_ = storage;

            //ensures memory is released
            ReferenceManager.AddItem(storage_);

            NativeMethods.IEnumSTATSTG? storageElementEnum = null;
            try
            {
                //enum all elements of the storage
                storage?.EnumElements(0, IntPtr.Zero, 0, out storageElementEnum);

                //iterate elements
                while (true)
                {
                    //get 1 element out of the com enumerator
                    System.Runtime.InteropServices.ComTypes.STATSTG[] elementStats = new System.Runtime.InteropServices.ComTypes.STATSTG[1];
                    storageElementEnum.Next(1, elementStats, out uint elementStatCount);

                    //break loop if element not retrieved
                    if (elementStatCount != 1)
                    {
                        break;
                    }

                    System.Runtime.InteropServices.ComTypes.STATSTG elementStat = elementStats[0];
                    switch (elementStat.type)
                    {
                        case 1:
                            //element is a storage. add its statistics object to the storage dictionary
                            subStorageStatistics.Add(elementStat.pwcsName, elementStat);
                            break;

                        case 2:
                            //element is a stream. add its statistics object to the stream dictionary
                            streamStatistics.Add(elementStat.pwcsName, elementStat);
                            break;
                    }
                }
            }
            finally
            {
                //free memory
                if (storageElementEnum != null)
                {
                    Marshal.ReleaseComObject(storageElementEnum);
                }
            }
        }

        /// <summary>
        /// Gets the MAPI property value from the property stream in this storage.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property or null if not found.</returns>
        private object? GetMapiPropertyFromPropertyStream(string propIdentifier)
        {
            //if no property stream return null
            if (!streamStatistics.ContainsKey(PROPERTIES_STREAM))
            {
                return null;
            }

            //get the raw bytes for the property stream
            var propBytes = GetStreamBytes(PROPERTIES_STREAM);

            //iterate over property stream in 16 byte chunks starting from end of header
            for (int i = propHeaderSize; i < propBytes.Length; i += 16)
            {
                //get property type located in the 1st and 2nd bytes as a unsigned short value
                var propType = BitConverter.ToUInt16(propBytes, i);

                //get property identifer located in 3nd and 4th bytes as a hexdecimal string
                var propIdent = new byte[] { propBytes[i + 3], propBytes[i + 2] };
                var propIdentString = BitConverter.ToString(propIdent).Replace("-", "");

                //if this is not the property being gotten continue to next property
                if (propIdentString != propIdentifier)
                {
                    continue;
                }

                //depending on prop type use method to get property value
                switch (propType)
                {
                    case NativeMethods.PT_I2:
                        return BitConverter.ToInt16(propBytes, i + 8);

                    case NativeMethods.PT_LONG:
                        return BitConverter.ToInt32(propBytes, i + 8);

                    case NativeMethods.PT_SYSTIME:
                        var fileTime = BitConverter.ToInt64(propBytes, i + 8);
                        return DateTime.FromFileTime(fileTime);

                    default:
                        throw new ApplicationException("MAPI property has an unsupported type and can not be retrieved.");
                }
            }

            //property not found return null
            return null;
        }

        /// <summary>
        /// Gets the MAPI property value from a stream or storage in this storage.
        /// </summary>
        /// <param name="propIdentifier">The 4 char hexadecimal prop identifier.</param>
        /// <returns>The value of the MAPI property or null if not found.</returns>
        private object? GetMapiPropertyFromStreamOrStorage(string propIdentifier)
        {
            //get list of stream and storage identifiers which map to properties
            var propKeys = new List<string>();
            propKeys.AddRange(streamStatistics.Keys);
            propKeys.AddRange(subStorageStatistics.Keys);

            //determine if the property identifier is in a stream or sub storage
            string? propTag = null;
            ushort propType = NativeMethods.PT_UNSPECIFIED;
            foreach (string propKey in propKeys)
            {
                if (propKey.StartsWith("__substg1.0_" + propIdentifier, StringComparison.Ordinal))
                {
                    propTag = propKey.Substring(12, 8);
                    propType = ushort.Parse(propKey.Substring(16, 4), System.Globalization.NumberStyles.HexNumber);
                    break;
                }
            }

            //depending on prop type use method to get property value
            string containerName = "__substg1.0_" + propTag;
            return propType switch
            {
                NativeMethods.PT_UNSPECIFIED => null,
                NativeMethods.PT_STRING8 => GetStreamAsString(containerName, Encoding.UTF8),
                NativeMethods.PT_UNICODE => GetStreamAsString(containerName, Encoding.Unicode),
                NativeMethods.PT_BINARY => GetStreamBytes(containerName),
                NativeMethods.PT_OBJECT => NativeMethods.CloneStorage(storage_.OpenStorage(containerName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0), true),
                _ => throw new ApplicationException("MAPI property has an unsupported type and can not be retrieved."),
            };
        }

        /// <summary>
        /// Recipient type
        /// </summary>
        public enum RecipientType
        {
            /// <summary>
            /// To
            /// </summary>
            To,

            /// <summary>
            /// CC
            /// </summary>
            CC,

            /// <summary>
            /// The unknown
            /// </summary>
            Unknown
        }

        /// <summary>
        /// Summary description for CLZF.
        /// </summary>
        public static class CLZF
        {
            /*
             This program is free software; you can redistribute it and/or modify
             it under the terms of the GNU General Public License as published by
             the Free Software Foundation; either version 2 of the License, or
             (at your option) any later version.

             You should have received a copy of the GNU General Public License
             along with this program; if not, write to the Free Software Foundation,
             Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA
            */

            /*
             * Prebuffered bytes used in RTF-compressed format (found them in RTFLIB32.LIB)
            */

            private const string prebuf = "{\\rtf1\\ansi\\mac\\deff0\\deftab720{\\fonttbl;}" +
                            "{\\f0\\fnil \\froman \\fswiss \\fmodern \\fscript " +
                "\\fdecor MS Sans SerifSymbolArialTimes New RomanCourier" +
                "{\\colortbl\\red0\\green0\\blue0\n\r\\par " +
                "\\pard\\plain\\f0\\fs20\\b\\i\\u\\tab\\tx";

            private static readonly uint[] CRC32_TABLE =
                        {
                0x00000000, 0x77073096, 0xEE0E612C, 0x990951BA, 0x076DC419,
                0x706AF48F, 0xE963A535, 0x9E6495A3, 0x0EDB8832, 0x79DCB8A4,
                0xE0D5E91E, 0x97D2D988, 0x09B64C2B, 0x7EB17CBD, 0xE7B82D07,
                0x90BF1D91, 0x1DB71064, 0x6AB020F2, 0xF3B97148, 0x84BE41DE,
                0x1ADAD47D, 0x6DDDE4EB, 0xF4D4B551, 0x83D385C7, 0x136C9856,
                0x646BA8C0, 0xFD62F97A, 0x8A65C9EC, 0x14015C4F, 0x63066CD9,
                0xFA0F3D63, 0x8D080DF5, 0x3B6E20C8, 0x4C69105E, 0xD56041E4,
                0xA2677172, 0x3C03E4D1, 0x4B04D447, 0xD20D85FD, 0xA50AB56B,
                0x35B5A8FA, 0x42B2986C, 0xDBBBC9D6, 0xACBCF940, 0x32D86CE3,
                0x45DF5C75, 0xDCD60DCF, 0xABD13D59, 0x26D930AC, 0x51DE003A,
                0xC8D75180, 0xBFD06116, 0x21B4F4B5, 0x56B3C423, 0xCFBA9599,
                0xB8BDA50F, 0x2802B89E, 0x5F058808, 0xC60CD9B2, 0xB10BE924,
                0x2F6F7C87, 0x58684C11, 0xC1611DAB, 0xB6662D3D, 0x76DC4190,
                0x01DB7106, 0x98D220BC, 0xEFD5102A, 0x71B18589, 0x06B6B51F,
                0x9FBFE4A5, 0xE8B8D433, 0x7807C9A2, 0x0F00F934, 0x9609A88E,
                0xE10E9818, 0x7F6A0DBB, 0x086D3D2D, 0x91646C97, 0xE6635C01,
                0x6B6B51F4, 0x1C6C6162, 0x856530D8, 0xF262004E, 0x6C0695ED,
                0x1B01A57B, 0x8208F4C1, 0xF50FC457, 0x65B0D9C6, 0x12B7E950,
                0x8BBEB8EA, 0xFCB9887C, 0x62DD1DDF, 0x15DA2D49, 0x8CD37CF3,
                0xFBD44C65, 0x4DB26158, 0x3AB551CE, 0xA3BC0074, 0xD4BB30E2,
                0x4ADFA541, 0x3DD895D7, 0xA4D1C46D, 0xD3D6F4FB, 0x4369E96A,
                0x346ED9FC, 0xAD678846, 0xDA60B8D0, 0x44042D73, 0x33031DE5,
                0xAA0A4C5F, 0xDD0D7CC9, 0x5005713C, 0x270241AA, 0xBE0B1010,
                0xC90C2086, 0x5768B525, 0x206F85B3, 0xB966D409, 0xCE61E49F,
                0x5EDEF90E, 0x29D9C998, 0xB0D09822, 0xC7D7A8B4, 0x59B33D17,
                0x2EB40D81, 0xB7BD5C3B, 0xC0BA6CAD, 0xEDB88320, 0x9ABFB3B6,
                0x03B6E20C, 0x74B1D29A, 0xEAD54739, 0x9DD277AF, 0x04DB2615,
                0x73DC1683, 0xE3630B12, 0x94643B84, 0x0D6D6A3E, 0x7A6A5AA8,
                0xE40ECF0B, 0x9309FF9D, 0x0A00AE27, 0x7D079EB1, 0xF00F9344,
                0x8708A3D2, 0x1E01F268, 0x6906C2FE, 0xF762575D, 0x806567CB,
                0x196C3671, 0x6E6B06E7, 0xFED41B76, 0x89D32BE0, 0x10DA7A5A,
                0x67DD4ACC, 0xF9B9DF6F, 0x8EBEEFF9, 0x17B7BE43, 0x60B08ED5,
                0xD6D6A3E8, 0xA1D1937E, 0x38D8C2C4, 0x4FDFF252, 0xD1BB67F1,
                0xA6BC5767, 0x3FB506DD, 0x48B2364B, 0xD80D2BDA, 0xAF0A1B4C,
                0x36034AF6, 0x41047A60, 0xDF60EFC3, 0xA867DF55, 0x316E8EEF,
                0x4669BE79, 0xCB61B38C, 0xBC66831A, 0x256FD2A0, 0x5268E236,
                0xCC0C7795, 0xBB0B4703, 0x220216B9, 0x5505262F, 0xC5BA3BBE,
                0xB2BD0B28, 0x2BB45A92, 0x5CB36A04, 0xC2D7FFA7, 0xB5D0CF31,
                0x2CD99E8B, 0x5BDEAE1D, 0x9B64C2B0, 0xEC63F226, 0x756AA39C,
                0x026D930A, 0x9C0906A9, 0xEB0E363F, 0x72076785, 0x05005713,
                0x95BF4A82, 0xE2B87A14, 0x7BB12BAE, 0x0CB61B38, 0x92D28E9B,
                0xE5D5BE0D, 0x7CDCEFB7, 0x0BDBDF21, 0x86D3D2D4, 0xF1D4E242,
                0x68DDB3F8, 0x1FDA836E, 0x81BE16CD, 0xF6B9265B, 0x6FB077E1,
                0x18B74777, 0x88085AE6, 0xFF0F6A70, 0x66063BCA, 0x11010B5C,
                0x8F659EFF, 0xF862AE69, 0x616BFFD3, 0x166CCF45, 0xA00AE278,
                0xD70DD2EE, 0x4E048354, 0x3903B3C2, 0xA7672661, 0xD06016F7,
                0x4969474D, 0x3E6E77DB, 0xAED16A4A, 0xD9D65ADC, 0x40DF0B66,
                0x37D83BF0, 0xA9BCAE53, 0xDEBB9EC5, 0x47B2CF7F, 0x30B5FFE9,
                0xBDBDF21C, 0xCABAC28A, 0x53B39330, 0x24B4A3A6, 0xBAD03605,
                0xCDD70693, 0x54DE5729, 0x23D967BF, 0xB3667A2E, 0xC4614AB8,
                0x5D681B02, 0x2A6F2B94, 0xB40BBE37, 0xC30C8EA1, 0x5A05DF1B,
                0x2D02EF8D
            };

            private static byte[]? COMPRESSED_RTF_PREBUF;
            /* The lookup table used in the CRC32 calculation */
            /*
             * Calculates the CRC32 of the given bytes.
             * The CRC32 calculation is similar to the standard one as demonstrated
             * in RFC 1952, but with the inversion (before and after the calculation)
             * ommited.
             *
             * @param buf the byte array to calculate CRC32 on
             * @param off the offset within buf at which the CRC32 calculation will start
             * @param len the number of bytes on which to calculate the CRC32
             * @return the CRC32 value.
             */

            /// <summary>
            /// Calculates the cr C32.
            /// </summary>
            /// <param name="buf">The buf.</param>
            /// <param name="off">The off.</param>
            /// <param name="len">The length.</param>
            /// <returns></returns>
            static public int CalculateCRC32(byte[] buf, int off, int len)
            {
                uint c = 0;
                int end = off + len;
                for (int i = off; i < end; i++)
                {
                    //!!!!        c = CRC32_TABLE[(c ^ buf[i]) & 0xFF] ^ (c >>> 8);
                    c = CRC32_TABLE[(c ^ buf[i]) & 0xFF] ^ (c >> 8);
                }
                return (int)c;
            }

            /*
                 * Returns an unsigned 32-bit value from little-endian ordered bytes.
                 *
                 * @param   buf a byte array from which byte values are taken
                 * @param   offset the offset within buf from which byte values are taken
                 * @return  an unsigned 32-bit value as a long.
            */

            /// <summary>
            /// Decompresses the RTF.
            /// </summary>
            /// <param name="src">The source.</param>
            /// <returns></returns>
            /// <exception cref="Exception">
            /// Invalid compressed-RTF header or compressed-RTF data size mismatch or compressed-RTF
            /// CRC32 failed or Unknown compression type (magic number + magic + )
            /// </exception>
            public static byte[] DecompressRTF(byte[] src)
            {
                byte[] dst; // destination for uncompressed bytes
                int inPos = 0; // current position in src array
                int outPos = 0; // current position in dst array

                COMPRESSED_RTF_PREBUF = Encoding.ASCII.GetBytes(prebuf);

                // get header fields (as defined in RTFLIB.H)
                if (src is null || src.Length < 16)
                    throw new Exception("Invalid compressed-RTF header");

                var compressedSize = (int)GetU32(src, inPos);
                inPos += 4;
                var uncompressedSize = (int)GetU32(src, inPos);
                inPos += 4;
                var magic = (int)GetU32(src, inPos);
                inPos += 4;
                var crc32 = (int)GetU32(src, inPos);
                inPos += 4;

                if (compressedSize != src.Length - 4) // check size excluding the size field itself
                    throw new Exception("compressed-RTF data size mismatch");

                if (crc32 != CalculateCRC32(src, 16, src.Length - 16))
                    throw new Exception("compressed-RTF CRC32 failed");

                // process the data
                if (magic == 0x414c454d)
                { // magic number that identifies the stream as a uncompressed stream
                    dst = new byte[uncompressedSize];
                    Array.Copy(src, inPos, dst, outPos, uncompressedSize); // just copy it as it is
                }
                else if (magic == 0x75465a4c)
                { // magic number that identifies the stream as a compressed stream
                    dst = new byte[COMPRESSED_RTF_PREBUF.Length + uncompressedSize];
                    Array.Copy(COMPRESSED_RTF_PREBUF, 0, dst, 0, COMPRESSED_RTF_PREBUF.Length);
                    outPos = COMPRESSED_RTF_PREBUF.Length;
                    int flagCount = 0;
                    int flags = 0;
                    while (outPos < dst.Length)
                    {
                        // each flag byte flags 8 literals/references, 1 per bit
                        flags = (flagCount++ % 8 == 0) ? GetU8(src, inPos++) : flags >> 1;
                        if ((flags & 1) == 1)
                        { // each flag bit is 1 for reference, 0 for literal
                            var offset = GetU8(src, inPos++);
                            var length = GetU8(src, inPos++);
                            //!!!!!!!!!            offset = (offset << 4) | (length >>> 4); // the offset relative to block start
                            offset = (offset << 4) | (length >> 4); // the offset relative to block start
                            length = (length & 0xF) + 2; // the number of bytes to copy
                                                         // the decompression buffer is supposed to
                                                         // wrap around back to the beginning when
                                                         // the end is reached. we save the need for
                                                         // such a buffer by pointing straight into
                                                         // the data buffer, and simulating this
                                                         // behaviour by modifying the pointers appropriately.
                            offset = ((outPos / 4096) * 4096) + offset;
                            if (offset >= outPos) // take from previous block
                                offset -= 4096;
                            // note: can't use System.arraycopy, because the referenced bytes can
                            // cross through the current out position.
                            int end = offset + length;
                            while (offset < end)
                                dst[outPos++] = dst[offset++];
                        }
                        else
                        { // literal
                            dst[outPos++] = src[inPos++];
                        }
                    }
                    // copy it back without the prebuffered data
                    src = dst;
                    dst = new byte[uncompressedSize];
                    Array.Copy(src, COMPRESSED_RTF_PREBUF.Length, dst, 0, uncompressedSize);
                }
                else
                { // unknown magic number
                    throw new Exception("Unknown compression type (magic number " + magic + ")");
                }

                return dst;
            }

            /// <summary>
            /// Gets the u32.
            /// </summary>
            /// <param name="buf">The buf.</param>
            /// <param name="offset">The offset.</param>
            /// <returns></returns>
            public static long GetU32(byte[] buf, int offset)
            {
                return ((buf[offset] & 0xFF) | ((buf[offset + 1] & 0xFF) << 8) | ((buf[offset + 2] & 0xFF) << 16) | ((buf[offset + 3] & 0xFF) << 24)) & 0x00000000FFFFFFFFL;
            }

            /*
             * Returns an unsigned 8-bit value from a byte array.
             *
             * @param   buf a byte array from which byte value is taken
             * @param   offset the offset within buf from which byte value is taken
             * @return  an unsigned 8-bit value as an int.
             */

            /// <summary>
            /// Gets the u8.
            /// </summary>
            /// <param name="buf">The buf.</param>
            /// <param name="offset">The offset.</param>
            /// <returns></returns>
            public static int GetU8(byte[] buf, int offset)
            {
                return buf[offset] & 0xFF;
            }

            /*
              * Decompresses compressed-RTF data.
              *
              * @param   src the compressed-RTF data bytes
              * @return  an array containing the decompressed bytes.
              * @throws  IllegalArgumentException if src does not contain valid                                                                                                                                            *          compressed-RTF bytes.
            */
        }

        /// <summary>
        /// Attachment
        /// </summary>
        /// <seealso cref="OutlookStorage"/>
        public class Attachment : OutlookStorage
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Attachment"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public Attachment(OutlookStorage message)
                : base(message.storage_)
            {
                GC.SuppressFinalize(message);
                propHeaderSize = PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP;
            }

            /// <summary>
            /// Gets the content id.
            /// </summary>
            /// <value>The content id.</value>
            public string? ContentId => GetMapiPropertyString(PR_ATTACH_CONTENT_ID);

            /// <summary>
            /// Gets the data.
            /// </summary>
            /// <value>The data.</value>
            public byte[]? Data => GetMapiPropertyBytes(PR_ATTACH_DATA);

            /// <summary>
            /// Gets the filename.
            /// </summary>
            /// <value>The filename.</value>
            public string? Filename
            {
                get
                {
                    var filename = GetMapiPropertyString(PR_ATTACH_LONG_FILENAME);
                    if (string.IsNullOrEmpty(filename))
                    {
                        filename = GetMapiPropertyString(PR_ATTACH_FILENAME);
                    }
                    if (string.IsNullOrEmpty(filename))
                    {
                        filename = GetMapiPropertyString(PR_DISPLAY_NAME);
                    }
                    return filename;
                }
            }

            /// <summary>
            /// Gets the rendering posisiton.
            /// </summary>
            /// <value>The rendering posisiton.</value>
            public int RenderingPosisiton => GetMapiPropertyInt32(PR_RENDERING_POSITION);
        }

        /// <summary>
        /// Message
        /// </summary>
        /// <seealso cref="OutlookStorage"/>
        public class Message : OutlookStorage
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Message"/> class from a msg file.
            /// </summary>
            /// <param name="msgfile">The msgfile.</param>
            public Message(string msgfile) : base(msgfile) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="Message"/> class from a containing an IStorage.
            /// </summary>
            /// <param name="storageStream">The <see cref="Stream"/> containing an IStorage.</param>
            public Message(Stream storageStream) : base(storageStream) { }

            /// <summary>
            /// Initializes a new instance of the <see cref="Message"/> class on the specified <see cref="NativeMethods.IStorage"/>.
            /// </summary>
            /// <param name="storage">The storage to create the <see cref="Message"/> on.</param>
            private Message(NativeMethods.IStorage? storage)
                : base(storage)
            {
                propHeaderSize = PROPERTIES_STREAM_HEADER_TOP;
            }

            /// <summary>
            /// Gets the list of attachments in the outlook message.
            /// </summary>
            /// <value>The list of attachments in the outlook message.</value>
            public List<Attachment> Attachments { get; } = new List<Attachment>();

            /// <summary>
            /// Gets the body of the outlook message in RTF format.
            /// </summary>
            /// <value>The body of the outlook message in RTF format.</value>
            public string? BodyRTF
            {
                get
                {
                    //get value for the RTF compressed MAPI property
                    var rtfBytes = GetMapiPropertyBytes(PR_RTF_COMPRESSED);

                    //return null if no property value exists
                    if (rtfBytes is null || rtfBytes.Length == 0)
                    {
                        return null;
                    }

                    //decompress the rtf value
                    rtfBytes = CLZF.DecompressRTF(rtfBytes);

                    //encode the rtf value as an ascii string and return
                    return Encoding.ASCII.GetString(rtfBytes);
                }
            }

            /// <summary>
            /// Gets the body of the outlook message in plain text format.
            /// </summary>
            /// <value>The body of the outlook message in plain text format.</value>
            public string? BodyText => GetMapiPropertyString(PR_BODY);

            /// <summary>
            /// Gets the display value of the contact that sent the email.
            /// </summary>
            /// <value>The display value of the contact that sent the email.</value>
            public string? From => GetMapiPropertyString(PR_SENDER_NAME);

            /// <summary>
            /// Gets the list of sub messages in the outlook message.
            /// </summary>
            /// <value>The list of sub messages in the outlook message.</value>
            public List<Message> Messages { get; } = new List<Message>();

            /// <summary>
            /// Gets the list of recipients in the outlook message.
            /// </summary>
            /// <value>The list of recipients in the outlook message.</value>
            public List<Recipient> Recipients { get; } = new List<Recipient>();

            /// <summary>
            /// Gets the subject of the outlook message.
            /// </summary>
            /// <value>The subject of the outlook message.</value>
            public string? Subject => GetMapiPropertyString(PR_SUBJECT);

            /// <summary>
            /// Saves this <see cref="Message"/> to the specified file name.
            /// </summary>
            /// <param name="fileName">Name of the file.</param>
            public void Save(string fileName)
            {
                var saveFileStream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite);
                Save(saveFileStream);
                saveFileStream.Close();
            }

            /// <summary>
            /// Saves this <see cref="Message"/> to the specified stream.
            /// </summary>
            /// <param name="stream">The stream to save to.</param>
            public void Save(Stream stream)
            {
                //get statistics for stream
                OutlookStorage saveMsg = this;

                byte[] memoryStorageContent;
                NativeMethods.IStorage? memoryStorage = null;
                NativeMethods.IStorage? nameIdSourceStorage = null;
                NativeMethods.ILockBytes? memoryStorageBytes = null;
                try
                {
                    //create a ILockBytes (unmanaged byte array) and then create a IStorage using the byte array as a backing store
                    NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                    NativeMethods.StgCreateDocfileOnILockBytes(memoryStorageBytes, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, out memoryStorage);

                    //copy the save storage into the new storage
                    saveMsg.storage_.CopyTo(0, null, IntPtr.Zero, memoryStorage);
                    memoryStorageBytes.Flush();
                    memoryStorage.Commit(0);

                    //if not the top parent then the name id mapping needs to be copied from top parent to this message and the property stream header needs to be padded by 8 bytes
                    if (!IsTopParent)
                    {
                        //create a new name id storage and get the source name id storage to copy from
                        NativeMethods.IStorage? nameIdStorage = memoryStorage.CreateStorage(NAMEID_STORAGE, NativeMethods.STGM.CREATE | NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, 0);
                        nameIdSourceStorage = TopParent.storage_.OpenStorage(NAMEID_STORAGE, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0);

                        //copy the name id storage from the parent to the new name id storage
                        nameIdSourceStorage.CopyTo(0, null, IntPtr.Zero, nameIdStorage);

                        //get the property bytes for the storage being copied
                        var props = saveMsg.GetStreamBytes(PROPERTIES_STREAM);

                        //create new array to store a copy of the properties that is 8 bytes larger than the old so the header can be padded
                        byte[] newProps = new byte[props.Length + 8];

                        //insert 8 null bytes from index 24 to 32. this is because a top level object property header requires a 32 byte header
                        Buffer.BlockCopy(props, 0, newProps, 0, 24);
                        Buffer.BlockCopy(props, 24, newProps, 32, props.Length - 24);

                        //remove the copied prop bytes so it can be replaced with the padded version
                        memoryStorage.DestroyElement(PROPERTIES_STREAM);

                        //create the property stream again and write in the padded version
                        var propStream = memoryStorage.CreateStream(PROPERTIES_STREAM, NativeMethods.STGM.READWRITE | NativeMethods.STGM.SHARE_EXCLUSIVE, 0, 0);
                        propStream.Write(newProps, newProps.Length, IntPtr.Zero);
                    }

                    //commit changes to the storage
                    memoryStorage.Commit(0);
                    memoryStorageBytes.Flush();

                    //get the STATSTG of the ILockBytes to determine how many bytes were written to it
                    memoryStorageBytes.Stat(out System.Runtime.InteropServices.ComTypes.STATSTG memoryStorageBytesStat, 1);

                    //read the bytes into a managed byte array
                    memoryStorageContent = new byte[memoryStorageBytesStat.cbSize];
                    memoryStorageBytes.ReadAt(0, memoryStorageContent, memoryStorageContent.Length, null);

                    //write storage bytes to stream
                    stream.Write(memoryStorageContent, 0, memoryStorageContent.Length);
                }
                finally
                {
                    if (nameIdSourceStorage != null)
                    {
                        Marshal.ReleaseComObject(nameIdSourceStorage);
                    }

                    if (memoryStorage != null)
                    {
                        Marshal.ReleaseComObject(memoryStorage);
                    }

                    if (memoryStorageBytes != null)
                    {
                        Marshal.ReleaseComObject(memoryStorageBytes);
                    }
                }
            }

            /// <summary>
            /// Gives sub classes the chance to free resources during object disposal.
            /// </summary>
            protected override void Disposing()
            {
                //dispose sub storages
                foreach (OutlookStorage subMsg in Messages)
                {
                    subMsg.Dispose();
                }

                //dispose sub storages
                foreach (OutlookStorage recip in Recipients)
                {
                    recip.Dispose();
                }

                //dispose sub storages
                foreach (OutlookStorage attach in Attachments)
                {
                    attach.Dispose();
                }
            }

            /// <summary>
            /// Processes sub storages on the specified storage to capture attachment and recipient data.
            /// </summary>
            /// <param name="storage">The storage to check for attachment and recipient data.</param>
            protected override void LoadStorage(NativeMethods.IStorage? storage)
            {
                base.LoadStorage(storage);

                foreach (System.Runtime.InteropServices.ComTypes.STATSTG storageStat in subStorageStatistics.Values)
                {
                    //element is a storage. get it and add its statistics object to the sub storage dictionary
                    var subStorage = storage_.OpenStorage(storageStat.pwcsName, IntPtr.Zero, NativeMethods.STGM.READ | NativeMethods.STGM.SHARE_EXCLUSIVE, IntPtr.Zero, 0);

                    //run specific load method depending on sub storage name prefix
                    if (storageStat.pwcsName.StartsWith(RECIP_STORAGE_PREFIX, StringComparison.Ordinal))
                    {
                        var recipient = new Recipient(new OutlookStorage(subStorage));
                        Recipients.Add(recipient);
                    }
                    else if (storageStat.pwcsName.StartsWith(ATTACH_STORAGE_PREFIX, StringComparison.Ordinal))
                    {
                        LoadAttachmentStorage(subStorage);
                    }
                    else
                    {
                        //release sub storage
                        Marshal.ReleaseComObject(subStorage);
                    }
                }
            }

            /// <summary>
            /// Loads the attachment data out of the specified storage.
            /// </summary>
            /// <param name="storage">The attachment storage.</param>
            private void LoadAttachmentStorage(NativeMethods.IStorage storage)
            {
                //create attachment from attachment storage
                var attachment = new Attachment(new OutlookStorage(storage));

                //if attachment is a embeded msg handle differently than an normal attachment
                var attachMethod = attachment.GetMapiPropertyInt32(PR_ATTACH_METHOD);
                if (attachMethod == ATTACH_EMBEDDED_MSG)
                {
                    //create new Message and set parent and header size
                    var subMsg = new Message(attachment.GetMapiProperty(PR_ATTACH_DATA) as NativeMethods.IStorage)
                    {
                        parentMessage = this,
                        propHeaderSize = PROPERTIES_STREAM_HEADER_EMBEDED
                    };

                    //add to messages list
                    Messages.Add(subMsg);
                }
                else
                {
                    //add attachment to attachment list
                    Attachments.Add(attachment);
                }
            }
        }

        /// <summary>
        /// Recipient
        /// </summary>
        /// <seealso cref="OutlookStorage"/>
        public class Recipient : OutlookStorage
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Recipient"/> class.
            /// </summary>
            /// <param name="message">The message.</param>
            public Recipient(OutlookStorage message)
                : base(message.storage_)
            {
                GC.SuppressFinalize(message);
                propHeaderSize = PROPERTIES_STREAM_HEADER_ATTACH_OR_RECIP;
            }

            /// <summary>
            /// Gets the display name.
            /// </summary>
            /// <value>The display name.</value>
            public string? DisplayName => GetMapiPropertyString(PR_DISPLAY_NAME);

            /// <summary>
            /// Gets the recipient email.
            /// </summary>
            /// <value>The recipient email.</value>
            public string? Email
            {
                get
                {
                    var email = GetMapiPropertyString(PR_EMAIL);
                    if (string.IsNullOrEmpty(email))
                    {
                        email = GetMapiPropertyString(PR_EMAIL_2);
                    }
                    return email;
                }
            }

            /// <summary>
            /// Gets the recipient type.
            /// </summary>
            /// <value>The recipient type.</value>
            public RecipientType Type
            {
                get
                {
                    var recipientType = GetMapiPropertyInt32(PR_RECIPIENT_TYPE);
                    return recipientType switch
                    {
                        MAPI_TO => RecipientType.To,
                        MAPI_CC => RecipientType.CC,
                        _ => RecipientType.Unknown,
                    };
                }
            }
        }

        /// <summary>
        /// Native methods
        /// </summary>
        protected static class NativeMethods
        {
            /// <summary>
            /// The p t_ apptime
            /// </summary>
            public const ushort PT_APPTIME = 7;

            /// <summary>
            /// The p t_ binary
            /// </summary>
            public const ushort PT_BINARY = 258;

            /// <summary>
            /// The p t_ boolean
            /// </summary>
            public const ushort PT_BOOLEAN = 11;

            /// <summary>
            /// The p t_ CLSID
            /// </summary>
            public const ushort PT_CLSID = 72;

            /// <summary>
            /// The p t_ currency
            /// </summary>
            public const ushort PT_CURRENCY = 6;

            /// <summary>
            /// The p t_ double
            /// </summary>
            public const ushort PT_DOUBLE = 5;

            /// <summary>
            /// The p t_ error
            /// </summary>
            public const ushort PT_ERROR = 10;

            /// <summary>
            /// The p t_ i2
            /// </summary>
            public const ushort PT_I2 = 2;

            /// <summary>
            /// The p t_ i8
            /// </summary>
            public const ushort PT_I8 = 20;

            /// <summary>
            /// The p t_ long
            /// </summary>
            public const ushort PT_LONG = 3;

            /// <summary>
            /// The p t_ null
            /// </summary>
            public const ushort PT_NULL = 1;

            /// <summary>
            /// The p t_ object
            /// </summary>
            public const ushort PT_OBJECT = 13;

            /// <summary>
            /// The pt r4
            /// </summary>
            public const ushort PT_R4 = 4;

            /// <summary>
            /// The p t_ strin g8
            /// </summary>
            public const ushort PT_STRING8 = 30;

            /// <summary>
            /// The p t_ systime
            /// </summary>
            public const ushort PT_SYSTIME = 64;

            /// <summary>
            /// The p t_ unicode
            /// </summary>
            public const ushort PT_UNICODE = 31;

            /// <summary>
            /// The p t_ unspecified
            /// </summary>
            public const ushort PT_UNSPECIFIED = 0;

            /// <summary>
            /// Clones the storage.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="closeSource">if set to <c>true</c> [close source].</param>
            /// <returns></returns>
            public static IStorage? CloneStorage(IStorage source, bool closeSource)
            {
                NativeMethods.IStorage? memoryStorage = null;
                NativeMethods.ILockBytes? memoryStorageBytes = null;
                try
                {
                    //create a ILockBytes (unmanaged byte array) and then create a IStorage using the byte array as a backing store
                    CreateILockBytesOnHGlobal(IntPtr.Zero, true, out memoryStorageBytes);
                    StgCreateDocfileOnILockBytes(memoryStorageBytes, STGM.CREATE | STGM.READWRITE | STGM.SHARE_EXCLUSIVE, 0, out memoryStorage);

                    //copy the source storage into the new storage
                    source.CopyTo(0, null, IntPtr.Zero, memoryStorage);
                    memoryStorageBytes.Flush();
                    memoryStorage.Commit(0);

                    //ensure memory is released
                    ReferenceManager.AddItem(memoryStorage);
                }
                catch
                {
                    if (memoryStorage != null)
                    {
                        Marshal.ReleaseComObject(memoryStorage);
                    }
                }
                finally
                {
                    if (memoryStorageBytes != null)
                    {
                        Marshal.ReleaseComObject(memoryStorageBytes);
                    }

                    if (closeSource)
                    {
                        Marshal.ReleaseComObject(source);
                    }
                }

                return memoryStorage;
            }

            /// <summary>
            /// Creates the i lock bytes on h global.
            /// </summary>
            /// <param name="hGlobal">The h global.</param>
            /// <param name="fDeleteOnRelease">if set to <c>true</c> [f delete on release].</param>
            /// <param name="ppLkbyt">The pp lkbyt.</param>
            /// <returns></returns>
            [DllImport("ole32.DLL")]
            public static extern int CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease, out ILockBytes ppLkbyt);

            /// <summary>
            /// STGs the create docfile on i lock bytes.
            /// </summary>
            /// <param name="plkbyt">The plkbyt.</param>
            /// <param name="grfMode">The GRF mode.</param>
            /// <param name="reserved">The reserved.</param>
            /// <param name="ppstgOpen">The PPSTG open.</param>
            /// <returns></returns>
            [DllImport("ole32.DLL")]
            public static extern int StgCreateDocfileOnILockBytes(ILockBytes plkbyt, STGM grfMode, uint reserved, out IStorage ppstgOpen);

            /// <summary>
            /// STGs the is storage file.
            /// </summary>
            /// <param name="wcsName">Name of the WCS.</param>
            /// <returns></returns>
            [DllImport("ole32.DLL")]
            public static extern int StgIsStorageFile([MarshalAs(UnmanagedType.LPWStr)] string wcsName);

            /// <summary>
            /// STGs the is storage i lock bytes.
            /// </summary>
            /// <param name="plkbyt">The plkbyt.</param>
            /// <returns></returns>
            [DllImport("ole32.DLL")]
            public static extern int StgIsStorageILockBytes(ILockBytes plkbyt);

            /// <summary>
            /// STGs the open storage.
            /// </summary>
            /// <param name="wcsName">Name of the WCS.</param>
            /// <param name="pstgPriority">The PSTG priority.</param>
            /// <param name="grfMode">The GRF mode.</param>
            /// <param name="snbExclude">The SNB exclude.</param>
            /// <param name="reserved">The reserved.</param>
            /// <param name="ppstgOpen">The PPSTG open.</param>
            /// <returns></returns>
            [DllImport("ole32.DLL")]
            public static extern int StgOpenStorage([MarshalAs(UnmanagedType.LPWStr)] string wcsName, IStorage? pstgPriority, STGM grfMode, IntPtr snbExclude, int reserved, out IStorage ppstgOpen);

            //[DllImport("ole32.DLL", CharSet = CharSet.Auto, PreserveSig = false)]
            //public static extern IntPtr GetHGlobalFromILockBytes(ILockBytes pLockBytes);
            /// <summary>
            /// STGs the open storage on i lock bytes.
            /// </summary>
            /// <param name="plkbyt">The plkbyt.</param>
            /// <param name="pstgPriority">The PSTG priority.</param>
            /// <param name="grfMode">The GRF mode.</param>
            /// <param name="snbExclude">The SNB exclude.</param>
            /// <param name="reserved">The reserved.</param>
            /// <param name="ppstgOpen">The PPSTG open.</param>
            [DllImport("ole32.DLL")]
            public static extern void StgOpenStorageOnILockBytes(ILockBytes plkbyt, IStorage? pstgPriority, STGM grfMode, IntPtr snbExclude, uint reserved, out IStorage ppstgOpen);

            [DllImport("kernel32.dll")]
            private static extern IntPtr GlobalLock(IntPtr hMem);

            /// <summary>
            /// STGM?
            /// </summary>
            [Flags]
            public enum STGM
            {
                /// <summary>
                /// The direct
                /// </summary>
                DIRECT = 0x00000000,

                /// <summary>
                /// The read
                /// </summary>
                READ = DIRECT,

                /// <summary>
                /// The failifthere
                /// </summary>
                FAILIFTHERE = DIRECT,

                /// <summary>
                /// The write
                /// </summary>
                WRITE = 0x00000001,

                /// <summary>
                /// The readwrite
                /// </summary>
                READWRITE = 0x00000002,

                /// <summary>
                /// The shar e_ exclusive
                /// </summary>
                SHARE_EXCLUSIVE = 0x00000010,

                /// <summary>
                /// The shar e_ den y_ write
                /// </summary>
                SHARE_DENY_WRITE = 0x00000020,

                /// <summary>
                /// The shar e_ den y_ read
                /// </summary>
                SHARE_DENY_READ = SHARE_EXCLUSIVE | SHARE_DENY_WRITE,

                /// <summary>
                /// The shar e_ den y_ none
                /// </summary>
                SHARE_DENY_NONE = 0x00000040,

                /// <summary>
                /// The create
                /// </summary>
                CREATE = 0x00001000,

                /// <summary>
                /// The transacted
                /// </summary>
                TRANSACTED = 0x00010000,

                /// <summary>
                /// The convert
                /// </summary>
                CONVERT = 0x00020000,

                /// <summary>
                /// The priority
                /// </summary>
                PRIORITY = 0x00040000,

                /// <summary>
                /// The noscratch
                /// </summary>
                NOSCRATCH = 0x00100000,

                /// <summary>
                /// The nosnapshot
                /// </summary>
                NOSNAPSHOT = 0x00200000,

                /// <summary>
                /// The direc t_ SWMR
                /// </summary>
                DIRECT_SWMR = 0x00400000,

                /// <summary>
                /// The deleteonrelease
                /// </summary>
                DELETEONRELEASE = 0x04000000,

                /// <summary>
                /// The simple
                /// </summary>
                SIMPLE = 0x08000000
            }

            /// <summary>
            /// Enum STATSTG
            /// </summary>
            [ComImport, Guid("0000000D-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface IEnumSTATSTG
            {
                /// <summary>
                /// Nexts the specified celt.
                /// </summary>
                /// <param name="celt">The celt.</param>
                /// <param name="rgelt">The rgelt.</param>
                /// <param name="pceltFetched">The pcelt fetched.</param>
                void Next(uint celt, [MarshalAs(UnmanagedType.LPArray), Out] System.Runtime.InteropServices.ComTypes.STATSTG[] rgelt, out uint pceltFetched);

                /// <summary>
                /// Skips the specified celt.
                /// </summary>
                /// <param name="celt">The celt.</param>
                void Skip(uint celt);

                /// <summary>
                /// Resets this instance.
                /// </summary>
                void Reset();

                /// <summary>
                /// Clones this instance.
                /// </summary>
                /// <returns></returns>
                [return: MarshalAs(UnmanagedType.Interface)]
                IEnumSTATSTG Clone();
            }

            /// <summary>
            /// ILock bytes
            /// </summary>
            [ComImport, Guid("0000000A-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface ILockBytes
            {
                /// <summary>
                /// Reads at.
                /// </summary>
                /// <param name="ulOffset">The ul offset.</param>
                /// <param name="pv">The pv.</param>
                /// <param name="cb">The cb.</param>
                /// <param name="pcbRead">The PCB read.</param>
                void ReadAt([In, MarshalAs(UnmanagedType.U8)] long ulOffset, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, [In, MarshalAs(UnmanagedType.U4)] int cb, [Out, MarshalAs(UnmanagedType.LPArray)] int[]? pcbRead);

                /// <summary>
                /// Writes at.
                /// </summary>
                /// <param name="ulOffset">The ul offset.</param>
                /// <param name="pv">The pv.</param>
                /// <param name="cb">The cb.</param>
                /// <param name="pcbWritten">The PCB written.</param>
                void WriteAt([In, MarshalAs(UnmanagedType.U8)] long ulOffset, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, [In, MarshalAs(UnmanagedType.U4)] int cb, [Out, MarshalAs(UnmanagedType.LPArray)] int[]? pcbWritten);

                /// <summary>
                /// Flushes this instance.
                /// </summary>
                void Flush();

                /// <summary>
                /// Sets the size.
                /// </summary>
                /// <param name="cb">The cb.</param>
                void SetSize([In, MarshalAs(UnmanagedType.U8)] long cb);

                /// <summary>
                /// Locks the region.
                /// </summary>
                /// <param name="libOffset">The library offset.</param>
                /// <param name="cb">The cb.</param>
                /// <param name="dwLockType">Type of the dw lock.</param>
                void LockRegion([In, MarshalAs(UnmanagedType.U8)] long libOffset, [In, MarshalAs(UnmanagedType.U8)] long cb, [In, MarshalAs(UnmanagedType.U4)] int dwLockType);

                /// <summary>
                /// Unlocks the region.
                /// </summary>
                /// <param name="libOffset">The library offset.</param>
                /// <param name="cb">The cb.</param>
                /// <param name="dwLockType">Type of the dw lock.</param>
                void UnlockRegion([In, MarshalAs(UnmanagedType.U8)] long libOffset, [In, MarshalAs(UnmanagedType.U8)] long cb, [In, MarshalAs(UnmanagedType.U4)] int dwLockType);

                /// <summary>
                /// Stats the specified pstatstg.
                /// </summary>
                /// <param name="pstatstg">The pstatstg.</param>
                /// <param name="grfStatFlag">The GRF stat flag.</param>
                void Stat([Out] out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [In, MarshalAs(UnmanagedType.U4)] int grfStatFlag);
            }

            /// <summary>
            /// Storage
            /// </summary>
            [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000B-0000-0000-C000-000000000046")]
            public interface IStorage
            {
                /// <summary>
                /// Creates the stream.
                /// </summary>
                /// <param name="pwcsName">Name of the PWCS.</param>
                /// <param name="grfMode">The GRF mode.</param>
                /// <param name="reserved1">The reserved1.</param>
                /// <param name="reserved2">The reserved2.</param>
                /// <returns></returns>
                [return: MarshalAs(UnmanagedType.Interface)]
                System.Runtime.InteropServices.ComTypes.IStream CreateStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

                /// <summary>
                /// Opens the stream.
                /// </summary>
                /// <param name="pwcsName">Name of the PWCS.</param>
                /// <param name="reserved1">The reserved1.</param>
                /// <param name="grfMode">The GRF mode.</param>
                /// <param name="reserved2">The reserved2.</param>
                /// <returns></returns>
                [return: MarshalAs(UnmanagedType.Interface)]
                System.Runtime.InteropServices.ComTypes.IStream OpenStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr reserved1, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

                /// <summary>
                /// Creates the storage.
                /// </summary>
                /// <param name="pwcsName">Name of the PWCS.</param>
                /// <param name="grfMode">The GRF mode.</param>
                /// <param name="reserved1">The reserved1.</param>
                /// <param name="reserved2">The reserved2.</param>
                /// <returns></returns>
                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage CreateStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

                /// <summary>
                /// Opens the storage.
                /// </summary>
                /// <param name="pwcsName">Name of the PWCS.</param>
                /// <param name="pstgPriority">The PSTG priority.</param>
                /// <param name="grfMode">The GRF mode.</param>
                /// <param name="snbExclude">The SNB exclude.</param>
                /// <param name="reserved">The reserved.</param>
                /// <returns></returns>
                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage OpenStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr pstgPriority, [In, MarshalAs(UnmanagedType.U4)] STGM grfMode, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.U4)] int reserved);

                /// <summary>
                /// Copies to.
                /// </summary>
                /// <param name="ciidExclude">The ciid exclude.</param>
                /// <param name="pIIDExclude">The p iid exclude.</param>
                /// <param name="snbExclude">The SNB exclude.</param>
                /// <param name="stgDest">The STG dest.</param>
                void CopyTo(int ciidExclude, [In, MarshalAs(UnmanagedType.LPArray)] Guid[]? pIIDExclude, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest);

                /// <summary>
                /// Moves the element to.
                /// </summary>
                /// <param name="pwcsName">Name of the PWCS.</param>
                /// <param name="stgDest">The STG dest.</param>
                /// <param name="pwcsNewName">New name of the PWCS.</param>
                /// <param name="grfFlags">The GRF flags.</param>
                void MoveElementTo([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName, [In, MarshalAs(UnmanagedType.U4)] int grfFlags);

                /// <summary>
                /// Commits the specified GRF commit flags.
                /// </summary>
                /// <param name="grfCommitFlags">The GRF commit flags.</param>
                void Commit(int grfCommitFlags);

                /// <summary>
                /// Reverts this instance.
                /// </summary>
                void Revert();

                /// <summary>
                /// Enums the elements.
                /// </summary>
                /// <param name="reserved1">The reserved1.</param>
                /// <param name="reserved2">The reserved2.</param>
                /// <param name="reserved3">The reserved3.</param>
                /// <param name="ppVal">The pp value.</param>
                void EnumElements([In, MarshalAs(UnmanagedType.U4)] int reserved1, IntPtr reserved2, [In, MarshalAs(UnmanagedType.U4)] int reserved3, [MarshalAs(UnmanagedType.Interface)] out IEnumSTATSTG ppVal);

                /// <summary>
                /// Destroys the element.
                /// </summary>
                /// <param name="pwcsName">Name of the PWCS.</param>
                void DestroyElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsName);

                /// <summary>
                /// Renames the element.
                /// </summary>
                /// <param name="pwcsOldName">Old name of the PWCS.</param>
                /// <param name="pwcsNewName">New name of the PWCS.</param>
                void RenameElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName);

                /// <summary>
                /// Sets the element times.
                /// </summary>
                /// <param name="pwcsName">Name of the PWCS.</param>
                /// <param name="pctime">The pctime.</param>
                /// <param name="patime">The patime.</param>
                /// <param name="pmtime">The pmtime.</param>
                void SetElementTimes([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

                /// <summary>
                /// Sets the class.
                /// </summary>
                /// <param name="clsid">The CLSID.</param>
                void SetClass([In] ref Guid clsid);

                /// <summary>
                /// Sets the state bits.
                /// </summary>
                /// <param name="grfStateBits">The GRF state bits.</param>
                /// <param name="grfMask">The GRF mask.</param>
                void SetStateBits(int grfStateBits, int grfMask);

                /// <summary>
                /// Stats the specified p stat STG.
                /// </summary>
                /// <param name="pStatStg">The p stat STG.</param>
                /// <param name="grfStatFlag">The GRF stat flag.</param>
                void Stat([Out] out System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
            }

            /* (Reserved for interface use) type doesn't matter to caller */
            /* NULL property value */
            /* Signed 16-bit value */
            /* Signed 32-bit value */
            /* 4-byte floating point */
            /* Floating point double */
            /* Signed 64-bit int (decimal w/    4 digits right of decimal pt) */
            /* Application time */
            /* 32-bit error value */
            /* 16-bit boolean (non-zero true) */
            /* Embedded object in a property */
            /* 8-byte signed integer */
            /* Null terminated 8-bit character string */
            /* Null terminated Unicode string */
            /* FILETIME 64-bit int w/ number of 100ns periods since Jan 1,1601 */
            /* OLE GUID */
            /* Uninterpreted (counted byte array) */
        }

        private class ReferenceManager
        {
            private ReferenceManager()
            {
            }

            ~ReferenceManager()
            {
                foreach (object trackingObject in trackingObjects)
                {
                    if (trackingObject != null)
                    {
                        Marshal.ReleaseComObject(trackingObject);
                    }
                }
            }

            private static readonly ReferenceManager instance = new ReferenceManager();

            private readonly List<object> trackingObjects = new List<object>();

            public static void AddItem(object? track)
            {
                lock (instance)
                {
                    if (!instance.trackingObjects.Contains(track!))
                    {
                        instance.trackingObjects.Add(track!);
                    }
                }
            }

            public static void RemoveItem(object track)
            {
                lock (instance)
                {
                    if (instance.trackingObjects.Contains(track))
                    {
                        instance.trackingObjects.Remove(track);
                    }
                }
            }
        }
    }
}