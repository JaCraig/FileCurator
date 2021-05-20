using FileCurator.Formats.Data;
using FileCurator.Formats.HTML;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.HTML
{
    public class HTMLFormatTests : TestBaseClass<HTMLFormat>
    {
        public HTMLFormatTests()
        {
            TestObject = new HTMLFormat();
        }

        [Fact]
        public void Read()
        {
            var TestObject = new HTMLFormat();
            using var TestFile = File.OpenRead("../../../TestData/TestHTML.htm");
            var Result = TestObject.Read(TestFile);
            Assert.Equal(@"Gut Instinct Blog Series RSS Feed Twitter Feed Aspectus Blammo.Net Craig's Utility Library DotCache DotExtension Echo.Net Gestalt.Net HaterAide ORM ObjectCartographer YABOV Blog Posts Craig's Utility Library 3.2 Released Version 3.2 of Craig's Utility Library has been released. Craig's Framework This post talks about Craig's Framework, a set of core classes that will be used in most of my MVC based projects going forward and how I plan to open source it. Personal Data Management Start of a project that I will be working on that will attempt to put the vast majority of my personal online life into one location. Craig's Utility Library 3.1 Released New version of Craig's Utility Library has been released. Small Changes, Big Speed Ups This post talks about a basic change that I did to the bitmap extensions that gave me a large increase in speed. Craig's Utility Library 3.0 Released Craig's Utility Library 3.0 has been released Building an DI Container This post shows the basic code that is being used for the IoC container in the utilities library. Utility Library Progress Shows a bit of code that is being updated in my utility library. Why You Should Sign Your DLLs This post explains why you should sign your DLLs. Setting Up Hg-Git The issues with setting up a github repository when using Hg. Also mirror sites were added for CUL. About Me Once upon a time, I was a developer for a small indie game company. Sadly after a couple of years, like many people, found that the stress and endless 80 hour weeks was just too much. Since then I've become a developer that for the past couple of years has focused on the .Net stack, with an emphasis on web development. During that time, I've set up this site as a location to host multiple open source projects, bits of code that I found useful, and my various rantings and ravings. The other bit of information that you should know about me is the fact that I like to reinvent the wheel. In my free time, creating that oblong wheel gives me a rush. Well, perhaps not a rush, but I enjoy it. Well sometimes I don't really enjoy it, but I learn from it. Sure it's not quite as round or as strong as the ones already made by others but it helps me become a better programmer. None of the code is particularly well written, but it's free and under the MIT license. That being said, if you read the articles or look at the various projects and learn something from them, even if it is what not to do, then I've done my job. License All code on my site is under the following license: Copyright (c) 2017 James Craig Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. Series AJAX Controls AOP ASP.Net Web Forms Classifiers Data Types Database Encryption Exchange File Formats General Image Editing IO LDAP Libraries And Releases Lotus Math Multithreading Networking Optimization ORM Randomization Reflection SharePoint Speech Validation Visual Studio Web Apps Recent Posts Craig's Utility Library 3.2 Released Version 3.2 of Craig's Utility Library has been released. Craig's Framework This post talks about Craig's Framework, a set of core classes that will be used in most of my MVC based projects going forward and how I plan to open source it. Personal Data Management Start of a project that I will be working on that will attempt to put the vast majority of my personal online life into one location. Craig's Utility Library 3.1 Released New version of Craig's Utility Library has been released. Small Changes, Big Speed Ups This post talks about a basic change that I did to the bitmap extensions that gave me a large increase in speed. Recent Comments Adjusting Contrast of an Image in C# Roey - It is better to use ColorMatri... Enabling JSONP in WCF BLop - Just to inform :""Cross doma... Viewstate Compression When Using AJAX Tahri Akram - I'm using compressing/decompre... Creating a Zip File in C# Nicholas Carey - Why not use DotNetZip from htt... Major Rework Happening mark foley - Hi!Your site is amazing. ... DISCLAIMER The opinions expressed herein are my own personal opinions and do not represent my employer's view in any way. &copy; Copyright 2010", Result.ToString());
        }

        [Fact]
        public void Write()
        {
            var TestObject = new HTMLFormat();
            using (var ResultFile = File.Open("./Results/TestHTMLWrite.htm", FileMode.OpenOrCreate))
            {
                Assert.True(TestObject.Write(ResultFile, new GenericFile("<html><body>Yay Testing</body></html>", "", "")));
            }
            using (var ResultFile = File.Open("./Results/TestHTMLWrite.htm", FileMode.OpenOrCreate))
            {
                var Result = TestObject.Read(ResultFile);
                Assert.Equal("Yay Testing", Result.ToString());
            }
        }
    }
}