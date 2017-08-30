# FileCurator

[![Build status](https://ci.appveyor.com/api/projects/status/3qlv7ffawn9lhxav?svg=true)](https://ci.appveyor.com/project/JaCraig/filecurator)

FileCurator is a library used to simplify file access and management on your system. It aims to make accessing a local file as simple as accessing a URL or 3rd party system like Dropbox.

## Basic Usage

The system relies on an IoC wrapper called [Canister](https://github.com/JaCraig/Canister). While Canister has a built in IoC container, it's purpose is to actually wrap your container of choice in a way that simplifies setup and usage for other libraries that don't want to be tied to a specific IoC container. FileCurator uses it to detect and pull in file system providers. As such you must set up Canister in order to use FileCurator:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
                .RegisterFileCurator()
                .Build();
	
This line is required prior to using the extension methods, FileInfo, and DirectoryInfo classes for the first time. Once Canister is set up, you can call the classes provided:

    var MyFile = new FileInfo("~/MyFile.txt");
	MyFile = new FileInfo("./MyFile.txt");
	MyFile = new FileInfo("MyFile.txt");
	MyFile = new FileInfo("http://www.google.com");
	MyFile = new FileInfo("resource://MyDLL/MyDLL.Resources.MyFile.txt");
	
The FileInfo and DirectoryInfo classes take a string for the file path as well as a user name, password, and domain, assuming the file system you are trying to reach requires it. It translates ~ and . to be the local base directory. From there you will have access to the file's contents and information. Similarly you can pass in web addresses or the location of embedded resource files and will be able to read them accordingly.

## Embedded Resources

For embedded resources, the syntax is:

    resource://MyDLL/MyDLL.Resources.Directory.MyFile.txt
    
Where resource:// lets the system know you want to retrieve an embedded resource. MyDLL is the name of the Assembly that the resource is found in. And MyFile.txt is the name of the file. Depending on where you placed the file the path inside the project will be the Resources.Directory portion of the above example. In the above case it was placed in the /Resources/Directory folder inside the assembly. Instead of slashes the system separates them with a period instead. If you placed the resources at the base of the project, then the Resouces.Directory portion can be left out and it would just be:

    resource://MyDLL/MyDLL.MyFile.txt

Another item to keep in mind is that you must register the assembly that you want to be able to pull the resource from with Canister:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
    		.RegisterFileCurator()
		.AddAssembly(typeof(TypeInTheAssemblyWithTheResources).GetTypeInfo().Assembly)
		.Build();

## Adding File Systems

The system comes with a couple of built in file systems for dealing with local files, however you may wish to add other targets as well. In order to do this all that you need to do is create a class that inherits from IFileSystem, a class that inherits from IFile, and one for IDirectory.
	
After the classes are created, you must tell Canister where to look for it. So modify the initialization line accordingly:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
    		.RegisterFileCurator()
		.AddAssembly(typeof(MyFileSystem).GetTypeInfo().Assembly)
		.Build();
	
From there the system will find the new provider and use it when called.

## Overriding File Systems

By default the system comes with a couple of file systems for dealing with local files. However it is possible to override these by simply creating a class that inherits from IFileSystem and setting the correct Name to match the one that you wish to override. There is a base class called LocalFileSystemBase that can help with most of the functions for the file system as well. For instance to override the "Relative Local" system with your own you would do the following:

    public class MyLocalFileSystem : LocalFileSystemBase
    {
        /// <summary>
        /// Name of the file system
        /// </summary>
        public override string Name { get { return "Relative Local"; } }

        /// <summary>
        /// Relative starter
        /// </summary>
        protected override string HandleRegexString { get { return @"^[~|\.]"; } }

        /// <summary>
        /// Gets the absolute path of the variable passed in
        /// </summary>
        /// <param name="path">Path to convert to absolute</param>
        /// <returns>The absolute path of the path passed in</returns>
        protected override string AbsolutePath(string path)
        {
            ...
        }
    }
	
After the class is created, you must tell Canister where to look for it. So modify the initialization line accordingly:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
    		.RegisterFileCurator()
		.AddAssembly(typeof(MyLocalFileSystem).GetTypeInfo().Assembly)
		.Build();
	
From there the system will override the default "Relative Local" provider with your own.

## Parsing Files

FileCurator also has a number of file formats that it understands and can parse:

* CSV
* TSV
* Tab delimited
* Excel (XLSX files only)
* HTML files
* ICS (iCalendar files)
* EML
* MHT
* PowerPoint (PPTX and PPSX)
* RSS
* VCS (vCal files)
* VCF (vCard files)
* Word (DOCX files only)
* XML
* And of course TXT files...

There are also a few items that are not .Net Core/.Net Standard supported in the FileCurator.Windows package:

* PDF
* MSG files
* RTF

Once a .Net Standard library is available to parse these items that is open sourced (and without a funky license), these will be moved into the main library. Anyway, in order to parse a file you would do the following:

    var MyFile = new FileInfo("~/MyFile.txt").Parse();
	
The above code opens the MyFile.txt document and parses it into a IGenericFile object. This object contains a Content property, a Title property, and a Meta property. For the above text file, only the Content property is filled in. However you can also do this:

    var MyEmail = new FileInfo("~/MyEmail.eml").Parse();
	
This will take the content of the email and place it in the Content property, the subject of the email is in Title. However you may be saying, what about To, or BCC, or From fields? That's why there is another Parse method:

    var MyEmail = new FileInfo("~/MyEmail.eml").Parse<IMessage>();
	
This time we get back an IMessage object instead of an IGenericFile object. And the IMessage object has fields for To, BCC, CC, From, Sent date, etc. The Parse<>() method takes any type that inherits from IGenericFile. The built in types are:

* IMessage
* ITable
* IFeed
* ICard
* ICalendar

And each of these correspond to a particular set of file formats:

* IMessage - EML, MHT, and MSG files.
* ITable - Delimited (CSV, TSV, etc.) and Excel files.
* IFeed - RSS files.
* ICard - vCards
* ICalendar - iCal and vCal files.

All other file types are parsed as IGenericFile objects. And calling for an object of type A when the parser returns type B will throw an exception. So if you have no idea what the file is, it's best to just use the Parse() method instead.

Writing an object to a file is similarly simple:

    var MyTable = new GenericTable();
    MyTable.Columns.Add("Column Header 1");
    MyTable.Columns.Add("Column Header 2");
    MyTable.Rows.Add(new GenericRow());
    MyTable.Rows[0].Cells.Add(new GenericCell("My Data"));
    MyTable.Rows[0].Cells.Add(new GenericCell("Goes Here"));
    new FileInfo("~/MyFile.xlsx").Write(MyTable);
	
The above code creates a table object with 2 column headers and a single row containing two cells, the first contains "My Data" and the second contains "Goes Here". The FileInfo object then takes the extension of the file that you are saving to and sends it to the proper format handler for writing the data to disk. In the above case it would be the Excel handler. You can similarly take the ITable object and save it as a CSV:

    new FileInfo("~/MyFile.csv").Write(MyTable);
	
No other code needs to change, just the file extension and it saves it properly as a CSV.

There are also extension methods to work with Streams instead of just FileInfo objects:

    using(var TempStream = new MemoryStream())
	{
	    TempStream.Write(new GenericFile("This is my content","My Title",""), MimeType.Word);
	}
	
The above code would write to the TempStream object a word doc that contains "This is my content" in the body and have a title of "My Title". You can similarly parse Stream objects like the FileInfo object but the only difference is that it takes in a MimeType object. This is to help it figure out what sort of file is in the stream. However for unknown files you can specify MimeType.Unknown. The system will then try its best to figure out what the file is and act accordingly.

## Writing Your Own Format Parser

All format parsers must inherit from the IFormat<TFile> interface. However there is a base class to help simplify some of the process called FormatBaseClass<TFileReader, TFileWriter, TFile>, but it is not required. As an example:

    /// <summary>
    /// Text format
    /// </summary>
    /// <seealso cref="BaseClasses.FormatBaseClass{TxtReader, TxtWriter, IGenericFile}"/>
    public class TxtFormat : FormatBaseClass<TxtReader, TxtWriter, IGenericFile>
    {
        /// <summary>
        /// Gets the content types.
        /// </summary>
        /// <value>The content types.</value>
        public override string[] ContentTypes => new[] { "TEXT/PLAIN" };

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        public override string DisplayName => "Text";

        /// <summary>
        /// Gets or sets the file types.
        /// </summary>
        /// <value>The file types.</value>
        public override string[] FileTypes => new[] { "TXT" };
    }
	
The above class is the TXT file parser. It also has a reader class:

    /// <summary>
    /// TXT file reader
    /// </summary>
    /// <seealso cref="Interfaces.IGenericFileReader{IGenericFile}"/>
    public class TxtReader : ReaderBaseClass<IGenericFile>
    {
        /// <summary>
        /// Gets the header identifier.
        /// </summary>
        /// <value>The header identifier.</value>
        public override byte[] HeaderIdentifier => new byte[0];

        /// <summary>
        /// Reads the specified stream.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>The file</returns>
        public override IGenericFile Read(Stream stream)
        {
            return new GenericFile(stream.ReadAll(), "", "");
        }
    }
	
And a writer class:

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
    }
	
You can create something similar for your formats as well. Just make sure that you tell Canister where to look for it. So modify the initialization line accordingly:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>())
    	.RegisterFileCurator()
		.AddAssembly(typeof(TxtFormat).GetTypeInfo().Assembly)
		.Build();
		
From there the system will automatically pick up your format and use it when appropriate. You can also override the existing formats with your own. You just need to state the content type and file types that you wish to intercept and it will use your items instead of the corresponding items in FileCurator.

## Installation

The library is available via Nuget with the package name "FileCurator". To install it run the following command in the Package Manager Console:

Install-Package FileCurator

The file parsers that are not .Net Standard yet are also available with the package name of "FileCurator.Windows". To install it run the following command in the Package Manager Console:

Install-Package FileCurator.Windows

This package, however, requires the full version of .Net and is not considered stable. As things become available in .Net Standard, they will be moved out of there and new items may move in as formats are added.

## Build Process

In order to build the library you will require the following as a minimum:

1. Visual Studio 2017
2. .Net Core 1.0 SDK

Other than that, just clone the project and you should be able to load the solution and build without too much effort.
