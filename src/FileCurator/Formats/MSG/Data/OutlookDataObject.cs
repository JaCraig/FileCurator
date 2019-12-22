#if NET462
/*
* OutlookDataObject - Drag and drop of outlook messages and attachments - http://www.iwantedue.com
* Copyright (C) 2008 David Ewen
*
* == BEGIN LICENSE ==
*
* Licensed under the terms of following license:
*
*  - The Code Project Open License 1.02 or later (the "CPOL")
*    http://www.codeproject.com/info/cpol10.aspx
*
* == END LICENSE ==
*
* This file defines the OutlookDataObject class used to gain access to dropped outlook
* messages and attachments
*/

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Forms;

namespace FileCurator.Formats.MSG
{
    /// <summary>
    /// Provides a format-independant machanism for transfering data with support for outlook
    /// messages and attachments.
    /// </summary>
    public class OutlookDataObject : System.Windows.Forms.IDataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OutlookDataObject"/> class.
        /// </summary>
        /// <param name="underlyingDataObject">The underlying data object to wrap.</param>
        public OutlookDataObject(System.Windows.Forms.IDataObject underlyingDataObject)
        {
            //get the underlying dataobject and its ComType IDataObject interface to it
            this.underlyingDataObject = underlyingDataObject;
            this.comUnderlyingDataObject = (System.Runtime.InteropServices.ComTypes.IDataObject)this.underlyingDataObject;

            //get the internal ole dataobject and its GetDataFromHGLOBLAL so it can be called later
            var innerDataField = this.underlyingDataObject.GetType().GetField("innerData", BindingFlags.NonPublic | BindingFlags.Instance);
            this.oleUnderlyingDataObject = (System.Windows.Forms.IDataObject)innerDataField.GetValue(this.underlyingDataObject);
            this.getDataFromHGLOBLALMethod = this.oleUnderlyingDataObject.GetType().GetMethod("GetDataFromHGLOBLAL", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Holds the <see cref="System.Runtime.InteropServices.ComTypes.IDataObject"/> interface to
        /// the <see cref="System.Windows.Forms.IDataObject"/> that this class is wrapping.
        /// </summary>
        private readonly System.Runtime.InteropServices.ComTypes.IDataObject comUnderlyingDataObject;

        /// <summary>
        /// Holds the <see cref="MethodInfo"/> of the "GetDataFromHGLOBLAL" method of the internal
        /// ole <see cref="System.Windows.Forms.IDataObject"/>.
        /// </summary>
        private readonly MethodInfo getDataFromHGLOBLALMethod;

        /// <summary>
        /// Holds the internal ole <see cref="System.Windows.Forms.IDataObject"/> to the
        /// <see cref="System.Windows.Forms.IDataObject"/> that this class is wrapping.
        /// </summary>
        private readonly System.Windows.Forms.IDataObject oleUnderlyingDataObject;

        /// <summary>
        /// Holds the <see cref="System.Windows.Forms.IDataObject"/> that this class is wrapping
        /// </summary>
        private readonly System.Windows.Forms.IDataObject underlyingDataObject;

        /// <summary>
        /// Retrieves the data associated with the specified class type format.
        /// </summary>
        /// <param name="format">
        /// A <see cref="T:System.Type"></see> representing the format of the data to retrieve. See
        /// <see cref="T:System.Windows.Forms.DataFormats"></see> for predefined formats.
        /// </param>
        /// <returns>The data associated with the specified format, or null.</returns>
        public object GetData(Type format)
        {
            return this.GetData(format.FullName);
        }

        /// <summary>
        /// Retrieves the data associated with the specified data format.
        /// </summary>
        /// <param name="format">
        /// The format of the data to retrieve. See
        /// <see cref="T:System.Windows.Forms.DataFormats"></see> for predefined formats.
        /// </param>
        /// <returns>The data associated with the specified format, or null.</returns>
        public object GetData(string format)
        {
            return this.GetData(format, true);
        }

        /// <summary>
        /// Retrieves the data associated with the specified data format, using a Boolean to
        /// determine whether to convert the data to the format.
        /// </summary>
        /// <param name="format">
        /// The format of the data to retrieve. See
        /// <see cref="T:System.Windows.Forms.DataFormats"></see> for predefined formats.
        /// </param>
        /// <param name="autoConvert">
        /// true to convert the data to the specified format; otherwise, false.
        /// </param>
        /// <returns>The data associated with the specified format, or null.</returns>
        public object GetData(string format, bool autoConvert)
        {
            //handle the "FileGroupDescriptor" and "FileContents" format request in this class otherwise pass through to underlying IDataObject
            switch (format)
            {
                case "FileGroupDescriptor":
                    //override the default handling of FileGroupDescriptor which returns a
                    //MemoryStream and instead return a string array of file names
                    IntPtr fileGroupDescriptorAPointer = IntPtr.Zero;
                    try
                    {
                        //use the underlying IDataObject to get the FileGroupDescriptor as a MemoryStream
                        var fileGroupDescriptorStream = (MemoryStream)this.underlyingDataObject.GetData("FileGroupDescriptor", autoConvert);
                        byte[] fileGroupDescriptorBytes = new byte[fileGroupDescriptorStream.Length];
                        fileGroupDescriptorStream.Read(fileGroupDescriptorBytes, 0, fileGroupDescriptorBytes.Length);
                        fileGroupDescriptorStream.Close();

                        //copy the file group descriptor into unmanaged memory
                        fileGroupDescriptorAPointer = Marshal.AllocHGlobal(fileGroupDescriptorBytes.Length);
                        Marshal.Copy(fileGroupDescriptorBytes, 0, fileGroupDescriptorAPointer, fileGroupDescriptorBytes.Length);

                        //marshal the unmanaged memory to to FILEGROUPDESCRIPTORA struct
                        var fileGroupDescriptorObject = Marshal.PtrToStructure(fileGroupDescriptorAPointer, typeof(NativeMethods.FILEGROUPDESCRIPTORA));
                        var fileGroupDescriptor = (NativeMethods.FILEGROUPDESCRIPTORA)fileGroupDescriptorObject;

                        //create a new array to store file names in of the number of items in the file group descriptor
                        string[] fileNames = new string[fileGroupDescriptor.cItems];

                        //get the pointer to the first file descriptor
                        var fileDescriptorPointer = (IntPtr)((int)fileGroupDescriptorAPointer + Marshal.SizeOf(fileGroupDescriptorAPointer));

                        //loop for the number of files acording to the file group descriptor
                        for (int fileDescriptorIndex = 0; fileDescriptorIndex < fileGroupDescriptor.cItems; fileDescriptorIndex++)
                        {
                            //marshal the pointer top the file descriptor as a FILEDESCRIPTORA struct and get the file name
                            var fileDescriptor = (NativeMethods.FILEDESCRIPTORA)Marshal.PtrToStructure(fileDescriptorPointer, typeof(NativeMethods.FILEDESCRIPTORA));
                            fileNames[fileDescriptorIndex] = fileDescriptor.cFileName;

                            //move the file descriptor pointer to the next file descriptor
                            fileDescriptorPointer = (IntPtr)((int)fileDescriptorPointer + Marshal.SizeOf(fileDescriptor));
                        }

                        //return the array of filenames
                        return fileNames;
                    }
                    finally
                    {
                        //free unmanaged memory pointer
                        Marshal.FreeHGlobal(fileGroupDescriptorAPointer);
                    }

                case "FileGroupDescriptorW":
                    //override the default handling of FileGroupDescriptorW which returns a
                    //MemoryStream and instead return a string array of file names
                    IntPtr fileGroupDescriptorWPointer = IntPtr.Zero;
                    try
                    {
                        //use the underlying IDataObject to get the FileGroupDescriptorW as a MemoryStream
                        var fileGroupDescriptorStream = (MemoryStream)this.underlyingDataObject.GetData("FileGroupDescriptorW");
                        byte[] fileGroupDescriptorBytes = new byte[fileGroupDescriptorStream.Length];
                        fileGroupDescriptorStream.Read(fileGroupDescriptorBytes, 0, fileGroupDescriptorBytes.Length);
                        fileGroupDescriptorStream.Close();

                        //copy the file group descriptor into unmanaged memory
                        fileGroupDescriptorWPointer = Marshal.AllocHGlobal(fileGroupDescriptorBytes.Length);
                        Marshal.Copy(fileGroupDescriptorBytes, 0, fileGroupDescriptorWPointer, fileGroupDescriptorBytes.Length);

                        //marshal the unmanaged memory to to FILEGROUPDESCRIPTORW struct
                        var fileGroupDescriptorObject = Marshal.PtrToStructure(fileGroupDescriptorWPointer, typeof(NativeMethods.FILEGROUPDESCRIPTORW));
                        var fileGroupDescriptor = (NativeMethods.FILEGROUPDESCRIPTORW)fileGroupDescriptorObject;

                        //create a new array to store file names in of the number of items in the file group descriptor
                        string[] fileNames = new string[fileGroupDescriptor.cItems];

                        //get the pointer to the first file descriptor
                        var fileDescriptorPointer = (IntPtr)((int)fileGroupDescriptorWPointer + Marshal.SizeOf(fileGroupDescriptorWPointer));

                        //loop for the number of files acording to the file group descriptor
                        for (int fileDescriptorIndex = 0; fileDescriptorIndex < fileGroupDescriptor.cItems; fileDescriptorIndex++)
                        {
                            //marshal the pointer top the file descriptor as a FILEDESCRIPTORW struct and get the file name
                            var fileDescriptor = (NativeMethods.FILEDESCRIPTORW)Marshal.PtrToStructure(fileDescriptorPointer, typeof(NativeMethods.FILEDESCRIPTORW));
                            fileNames[fileDescriptorIndex] = fileDescriptor.cFileName;

                            //move the file descriptor pointer to the next file descriptor
                            fileDescriptorPointer = (IntPtr)((int)fileDescriptorPointer + Marshal.SizeOf(fileDescriptor));
                        }

                        //return the array of filenames
                        return fileNames;
                    }
                    finally
                    {
                        //free unmanaged memory pointer
                        Marshal.FreeHGlobal(fileGroupDescriptorWPointer);
                    }

                case "FileContents":
                    //override the default handling of FileContents which returns the
                    //contents of the first file as a memory stream and instead return
                    //a array of MemoryStreams containing the data to each file dropped

                    //get the array of filenames which lets us know how many file contents exist
                    var fileContentNames = (string[])this.GetData("FileGroupDescriptor");

                    //create a MemoryStream array to store the file contents
                    MemoryStream[] fileContents = new MemoryStream[fileContentNames.Length];

                    //loop for the number of files acording to the file names
                    for (int fileIndex = 0; fileIndex < fileContentNames.Length; fileIndex++)
                    {
                        //get the data at the file index and store in array
                        fileContents[fileIndex] = this.GetData(format, fileIndex);
                    }

                    //return array of MemoryStreams containing file contents
                    return fileContents;
            }

            //use underlying IDataObject to handle getting of data
            return this.underlyingDataObject.GetData(format, autoConvert);
        }

        /// <summary>
        /// Retrieves the data associated with the specified data format at the specified index.
        /// </summary>
        /// <param name="format">
        /// The format of the data to retrieve. See
        /// <see cref="T:System.Windows.Forms.DataFormats"></see> for predefined formats.
        /// </param>
        /// <param name="index">The index of the data to retrieve.</param>
        /// <returns>
        /// A <see cref="MemoryStream"/> containing the raw data for the specified data format at
        /// the specified index.
        /// </returns>
        public MemoryStream GetData(string format, int index)
        {
            //create a FORMATETC struct to request the data with
            var formatetc = new FORMATETC
            {
                cfFormat = (short)DataFormats.GetFormat(format).Id,
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = index,
                ptd = new IntPtr(0),
                tymed = TYMED.TYMED_ISTREAM | TYMED.TYMED_ISTORAGE | TYMED.TYMED_HGLOBAL
            };

            //create STGMEDIUM to output request results into
            var medium = new STGMEDIUM();

            //using the Com IDataObject interface get the data using the defined FORMATETC
            this.comUnderlyingDataObject.GetData(ref formatetc, out medium);

            //retrieve the data depending on the returned store type
            switch (medium.tymed)
            {
                case TYMED.TYMED_ISTORAGE:
                    //to handle a IStorage it needs to be written into a second unmanaged
                    //memory mapped storage and then the data can be read from memory into
                    //a managed byte and returned as a MemoryStream

                    NativeMethods.IStorage iStorage = null;
                    NativeMethods.IStorage iStorage2 = null;
                    NativeMethods.ILockBytes iLockBytes = null;
                    System.Runtime.InteropServices.ComTypes.STATSTG iLockBytesStat;
                    try
                    {
                        //marshal the returned pointer to a IStorage object
                        iStorage = (NativeMethods.IStorage)Marshal.GetObjectForIUnknown(medium.unionmember);
                        Marshal.Release(medium.unionmember);

                        //create a ILockBytes (unmanaged byte array) and then create a IStorage using the byte array as a backing store
                        iLockBytes = NativeMethods.CreateILockBytesOnHGlobal(IntPtr.Zero, true);
                        iStorage2 = NativeMethods.StgCreateDocfileOnILockBytes(iLockBytes, 0x00001012, 0);

                        //copy the returned IStorage into the new IStorage
                        iStorage.CopyTo(0, null, IntPtr.Zero, iStorage2);
                        iLockBytes.Flush();
                        iStorage2.Commit(0);

                        //get the STATSTG of the ILockBytes to determine how many bytes were written to it
                        iLockBytesStat = new System.Runtime.InteropServices.ComTypes.STATSTG();
                        iLockBytes.Stat(out iLockBytesStat, 1);
                        var iLockBytesSize = (int)iLockBytesStat.cbSize;

                        //read the data from the ILockBytes (unmanaged byte array) into a managed byte array
                        byte[] iLockBytesContent = new byte[iLockBytesSize];
                        iLockBytes.ReadAt(0, iLockBytesContent, iLockBytesContent.Length, null);

                        //wrapped the managed byte array into a memory stream and return it
                        return new MemoryStream(iLockBytesContent);
                    }
                    finally
                    {
                        //release all unmanaged objects
                        Marshal.ReleaseComObject(iStorage2);
                        Marshal.ReleaseComObject(iLockBytes);
                        Marshal.ReleaseComObject(iStorage);
                    }

                case TYMED.TYMED_ISTREAM:
                    //to handle a IStream it needs to be read into a managed byte and
                    //returned as a MemoryStream

                    IStream iStream = null;
                    System.Runtime.InteropServices.ComTypes.STATSTG iStreamStat;
                    try
                    {
                        //marshal the returned pointer to a IStream object
                        iStream = (IStream)Marshal.GetObjectForIUnknown(medium.unionmember);
                        Marshal.Release(medium.unionmember);

                        //get the STATSTG of the IStream to determine how many bytes are in it
                        iStreamStat = new System.Runtime.InteropServices.ComTypes.STATSTG();
                        iStream.Stat(out iStreamStat, 0);
                        var iStreamSize = (int)iStreamStat.cbSize;

                        //read the data from the IStream into a managed byte array
                        byte[] iStreamContent = new byte[iStreamSize];
                        iStream.Read(iStreamContent, iStreamContent.Length, IntPtr.Zero);

                        //wrapped the managed byte array into a memory stream and return it
                        return new MemoryStream(iStreamContent);
                    }
                    finally
                    {
                        //release all unmanaged objects
                        Marshal.ReleaseComObject(iStream);
                    }

                case TYMED.TYMED_HGLOBAL:
                    //to handle a HGlobal the exisitng "GetDataFromHGLOBLAL" method is invoked via
                    //reflection

                    return (MemoryStream)this.getDataFromHGLOBLALMethod.Invoke(this.oleUnderlyingDataObject, new object[] { DataFormats.GetFormat((short)formatetc.cfFormat).Name, medium.unionmember });
            }

            return null;
        }

        /// <summary>
        /// Determines whether data stored in this instance is associated with, or can be converted
        /// to, the specified format.
        /// </summary>
        /// <param name="format">
        /// A <see cref="T:System.Type"></see> representing the format for which to check. See
        /// <see cref="T:System.Windows.Forms.DataFormats"></see> for predefined formats.
        /// </param>
        /// <returns>
        /// true if data stored in this instance is associated with, or can be converted to, the
        /// specified format; otherwise, false.
        /// </returns>
        public bool GetDataPresent(Type format)
        {
            return this.underlyingDataObject.GetDataPresent(format);
        }

        /// <summary>
        /// Determines whether data stored in this instance is associated with, or can be converted
        /// to, the specified format.
        /// </summary>
        /// <param name="format">
        /// The format for which to check. See <see cref="T:System.Windows.Forms.DataFormats"></see>
        /// for predefined formats.
        /// </param>
        /// <returns>
        /// true if data stored in this instance is associated with, or can be converted to, the
        /// specified format; otherwise false.
        /// </returns>
        public bool GetDataPresent(string format)
        {
            return this.underlyingDataObject.GetDataPresent(format);
        }

        /// <summary>
        /// Determines whether data stored in this instance is associated with the specified format,
        /// using a Boolean value to determine whether to convert the data to the format.
        /// </summary>
        /// <param name="format">
        /// The format for which to check. See <see cref="T:System.Windows.Forms.DataFormats"></see>
        /// for predefined formats.
        /// </param>
        /// <param name="autoConvert">
        /// true to determine whether data stored in this instance can be converted to the specified
        /// format; false to check whether the data is in the specified format.
        /// </param>
        /// <returns>
        /// true if the data is in, or can be converted to, the specified format; otherwise, false.
        /// </returns>
        public bool GetDataPresent(string format, bool autoConvert)
        {
            return this.underlyingDataObject.GetDataPresent(format, autoConvert);
        }

        /// <summary>
        /// Returns a list of all formats that data stored in this instance is associated with or
        /// can be converted to.
        /// </summary>
        /// <returns>
        /// An array of the names that represents a list of all formats that are supported by the
        /// data stored in this object.
        /// </returns>
        public string[] GetFormats()
        {
            return this.underlyingDataObject.GetFormats();
        }

        /// <summary>
        /// Gets a list of all formats that data stored in this instance is associated with or can
        /// be converted to, using a Boolean value to determine whether to retrieve all formats that
        /// the data can be converted to or only native data formats.
        /// </summary>
        /// <param name="autoConvert">
        /// true to retrieve all formats that data stored in this instance is associated with or can
        /// be converted to; false to retrieve only native data formats.
        /// </param>
        /// <returns>
        /// An array of the names that represents a list of all formats that are supported by the
        /// data stored in this object.
        /// </returns>
        public string[] GetFormats(bool autoConvert)
        {
            return this.underlyingDataObject.GetFormats(autoConvert);
        }

        /// <summary>
        /// Stores the specified data in this instance, using the class of the data for the format.
        /// </summary>
        /// <param name="data">The data to store.</param>
        public void SetData(object data)
        {
            this.underlyingDataObject.SetData(data);
        }

        /// <summary>
        /// Stores the specified data and its associated class type in this instance.
        /// </summary>
        /// <param name="format">
        /// A <see cref="T:System.Type"></see> representing the format associated with the data. See
        /// <see cref="T:System.Windows.Forms.DataFormats"></see> for predefined formats.
        /// </param>
        /// <param name="data">The data to store.</param>
        public void SetData(Type format, object data)
        {
            this.underlyingDataObject.SetData(format, data);
        }

        /// <summary>
        /// Stores the specified data and its associated format in this instance.
        /// </summary>
        /// <param name="format">
        /// The format associated with the data. See
        /// <see cref="T:System.Windows.Forms.DataFormats"></see> for predefined formats.
        /// </param>
        /// <param name="data">The data to store.</param>
        public void SetData(string format, object data)
        {
            this.underlyingDataObject.SetData(format, data);
        }

        /// <summary>
        /// Stores the specified data and its associated format in this instance, using a Boolean
        /// value to specify whether the data can be converted to another format.
        /// </summary>
        /// <param name="format">
        /// The format associated with the data. See
        /// <see cref="T:System.Windows.Forms.DataFormats"></see> for predefined formats.
        /// </param>
        /// <param name="autoConvert">
        /// true to allow the data to be converted to another format; otherwise, false.
        /// </param>
        /// <param name="data">The data to store.</param>
        public void SetData(string format, bool autoConvert, object data)
        {
            this.underlyingDataObject.SetData(format, autoConvert, data);
        }

        private static class NativeMethods
        {
            [DllImport("ole32.dll", PreserveSig = false)]
            public static extern ILockBytes CreateILockBytesOnHGlobal(IntPtr hGlobal, bool fDeleteOnRelease);

            [DllImport("OLE32.DLL", CharSet = CharSet.Auto, PreserveSig = false)]
            public static extern IntPtr GetHGlobalFromILockBytes(ILockBytes pLockBytes);

            [DllImport("OLE32.DLL", CharSet = CharSet.Unicode, PreserveSig = false)]
            public static extern IStorage StgCreateDocfileOnILockBytes(ILockBytes plkbyt, uint grfMode, uint reserved);

            [DllImport("kernel32.dll")]
            private static extern IntPtr GlobalLock(IntPtr hMem);

            [ComImport, Guid("0000000A-0000-0000-C000-000000000046"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
            public interface ILockBytes
            {
                void ReadAt([In, MarshalAs(UnmanagedType.U8)] long ulOffset, [Out, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] byte[] pv, [In, MarshalAs(UnmanagedType.U4)] int cb, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pcbRead);

                void WriteAt([In, MarshalAs(UnmanagedType.U8)] long ulOffset, IntPtr pv, [In, MarshalAs(UnmanagedType.U4)] int cb, [Out, MarshalAs(UnmanagedType.LPArray)] int[] pcbWritten);

                void Flush();

                void SetSize([In, MarshalAs(UnmanagedType.U8)] long cb);

                void LockRegion([In, MarshalAs(UnmanagedType.U8)] long libOffset, [In, MarshalAs(UnmanagedType.U8)] long cb, [In, MarshalAs(UnmanagedType.U4)] int dwLockType);

                void UnlockRegion([In, MarshalAs(UnmanagedType.U8)] long libOffset, [In, MarshalAs(UnmanagedType.U8)] long cb, [In, MarshalAs(UnmanagedType.U4)] int dwLockType);

                void Stat([Out]out System.Runtime.InteropServices.ComTypes.STATSTG pstatstg, [In, MarshalAs(UnmanagedType.U4)] int grfStatFlag);
            }

            [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("0000000B-0000-0000-C000-000000000046")]
            public interface IStorage
            {
                [return: MarshalAs(UnmanagedType.Interface)]
                IStream CreateStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

                [return: MarshalAs(UnmanagedType.Interface)]
                IStream OpenStream([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr reserved1, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage CreateStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.U4)] int grfMode, [In, MarshalAs(UnmanagedType.U4)] int reserved1, [In, MarshalAs(UnmanagedType.U4)] int reserved2);

                [return: MarshalAs(UnmanagedType.Interface)]
                IStorage OpenStorage([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, IntPtr pstgPriority, [In, MarshalAs(UnmanagedType.U4)] int grfMode, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.U4)] int reserved);

                void CopyTo(int ciidExclude, [In, MarshalAs(UnmanagedType.LPArray)] Guid[] pIIDExclude, IntPtr snbExclude, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest);

                void MoveElementTo([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In, MarshalAs(UnmanagedType.Interface)] IStorage stgDest, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName, [In, MarshalAs(UnmanagedType.U4)] int grfFlags);

                void Commit(int grfCommitFlags);

                void Revert();

                void EnumElements([In, MarshalAs(UnmanagedType.U4)] int reserved1, IntPtr reserved2, [In, MarshalAs(UnmanagedType.U4)] int reserved3, [MarshalAs(UnmanagedType.Interface)] out object ppVal);

                void DestroyElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsName);

                void RenameElement([In, MarshalAs(UnmanagedType.BStr)] string pwcsOldName, [In, MarshalAs(UnmanagedType.BStr)] string pwcsNewName);

                void SetElementTimes([In, MarshalAs(UnmanagedType.BStr)] string pwcsName, [In] System.Runtime.InteropServices.ComTypes.FILETIME pctime, [In] System.Runtime.InteropServices.ComTypes.FILETIME patime, [In] System.Runtime.InteropServices.ComTypes.FILETIME pmtime);

                void SetClass([In] ref Guid clsid);

                void SetStateBits(int grfStateBits, int grfMask);

                void Stat([Out]out System.Runtime.InteropServices.ComTypes.STATSTG pStatStg, int grfStatFlag);
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public sealed class FILEDESCRIPTORA
            {
                public uint dwFlags;
                public Guid clsid;
                public SIZEL sizel;
                public POINTL pointl;
                public uint dwFileAttributes;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public sealed class FILEDESCRIPTORW
            {
                public uint dwFlags;
                public Guid clsid;
                public SIZEL sizel;
                public POINTL pointl;
                public uint dwFileAttributes;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
                public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
                public uint nFileSizeHigh;
                public uint nFileSizeLow;

                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
                public string cFileName;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
            public sealed class FILEGROUPDESCRIPTORA
            {
                public uint cItems;
                public FILEDESCRIPTORA[] fgd;
            }

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public sealed class FILEGROUPDESCRIPTORW
            {
                public uint cItems;
                public FILEDESCRIPTORW[] fgd;
            }

            [StructLayout(LayoutKind.Sequential)]
            public sealed class POINTL
            {
                public int x;
                public int y;
            }

            [StructLayout(LayoutKind.Sequential)]
            public sealed class SIZEL
            {
                public int cx;
                public int cy;
            }
        }
    }
}
#endif