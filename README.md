# FileCurator

[![Build status](https://ci.appveyor.com/api/projects/status/3qlv7ffawn9lhxav?svg=true)](https://ci.appveyor.com/project/JaCraig/filecurator)

FileCurator is a library used to simplify file access and management on your system. It aims to make accessing a local file as simple as accessing a URL or 3rd party system like Dropbox.

## Basic Usage

The system relies on an IoC wrapper called [Canister](https://github.com/JaCraig/Canister). While Canister has a built in IoC container, it's purpose is to actually wrap your container of choice in a way that simplifies setup and usage for other libraries that don't want to be tied to a specific IoC container. FileCurator uses it to detect and pull in file system providers. As such you must set up Canister in order to use FileCurator:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>(), typeof(FileCurator).GetTypeInfo().Assembly);
	
This line is required prior to using the extension methods, FileInfo, and DirectoryInfo classes for the first time. Once Canister is set up, you can call the classes provided:

    var MyFile = new FileInfo("~/MyFile.txt");
	MyFile = new FileInfo("./MyFile.txt");
	MyFile = new FileInfo("MyFile.txt");
	
The FileInfo and DirectoryInfo classes take a string for the file path as well as a user name, password, and domain, assuming the file system you are trying to reach requires it. It translates ~ and . to be the local base directory. From there you will have access to the file's contents and information.

## Adding File Systems

The system comes with a couple of built in file systems for dealing with local files, however you may wish to add other targets as well. In order to do this all that you need to do is create a class that inherits from IFileSystem, a class that inherits from IFile, and one for IDirectory.
	
After the classes are created, you must tell Canister where to look for it. So modify the initialization line accordingly:

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>(), typeof(FileCurator).GetTypeInfo().Assembly, typeof(MyFileSystem).GetTypeInfo().Assembly);
	
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

    Canister.Builder.CreateContainer(new List<ServiceDescriptor>(), typeof(FileCurator).GetTypeInfo().Assembly, typeof(MyLocalFileSystem).GetTypeInfo().Assembly);
	
From there the system will override the default "Relative Local" provider with your own.

## Installation

The library is available via Nuget with the package name "FileCurator". To install it run the following command in the Package Manager Console:

Install-Package FileCurator

## Build Process

In order to build the library you will require the following as a minimum:

1. Visual Studio 2015 with Update 3
2. .Net Core 1.0 SDK

Other than that, just clone the project and you should be able to load the solution and build without too much effort.