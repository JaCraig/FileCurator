using FileCurator.Formats.Data;
using FileCurator.Formats.HTML;
using FileCurator.Tests.BaseClasses;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats.HTML
{
    public class HTMLFormatTests : TestingDirectoryFixture
    {
        [Fact]
        public void Read()
        {
            var TestObject = new HTMLFormat();
            using (var TestFile = File.OpenRead("../../../TestData/TestHTML.htm"))
            {
                var Result = TestObject.Read(TestFile);
                Assert.Equal(@" Gut InstinctBlog Series RSS FeedTwitter FeedAspectusBlammo.NetCraig's Utility LibraryDotCacheDotExtensionEcho.NetGestalt.NetHaterAide ORMObjectCartographerYABOV Blog PostsCraig&rsquo;s Utility Library 3.2 ReleasedVersion 3.2 of Craig's Utility Library has been released.Craig&rsquo;s FrameworkThis post talks about Craig's Framework, a set of core classes that will be used in most of my MVC based projects going forward and how I plan to open source it.Personal Data ManagementStart of a project that I will be working on that will attempt to put the vast majority of my personal online life into one location.Craig&rsquo;s Utility Library 3.1 ReleasedNew version of Craig's Utility Library has been released.Small Changes, Big Speed UpsThis post talks about a basic change that I did to the bitmap extensions that gave me a large increase in speed.Craig&rsquo;s Utility Library 3.0 ReleasedCraig's Utility Library 3.0 has been releasedBuilding an DI ContainerThis post shows the basic code that is being used for the IoC container in the utilities library.Utility Library ProgressShows a bit of code that is being updated in my utility library.Why You Should Sign Your DLLsThis post explains why you should sign your DLLs.Setting Up Hg-GitThe issues with setting up a github repository when using Hg. Also mirror sites were added for CUL.About MeOnce upon a time, I was a developer for a small indie game company. Sadly after a couple of years, like many people, found that the stress and endless 80 hour weeks was just too much. Since then I've become a developer that for the past couple of years has focused on the .Net stack, with an emphasis on web development. During that time, I've set up this site as a location to host multiple open source projects, bits of code that I found useful, and my various rantings and ravings.The other bit of information that you should know about me is the fact that I like to reinvent the wheel. In my free time, creating that oblong wheel gives me a rush. Well, perhaps not a rush, but I enjoy it. Well sometimes I don't really enjoy it, but I learn from it. Sure it's not quite as round or as strong as the ones already made by others but it helps me become a better programmer. None of the code is particularly well written, but it's free and under the MIT license. That being said, if you read the articles or look at the various projects and learn something from them, even if it is what not to do, then I've done my job.LicenseAll code on my site is under the following license:Copyright (c) 2017 James CraigPermission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.THE SOFTWARE IS PROVIDED ""AS IS"", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.SeriesAJAX ControlsAOPASP.Net Web FormsClassifiersData TypesDatabaseEncryptionExchangeFile FormatsGeneralImage EditingIOLDAPLibraries And ReleasesLotusMathMultithreadingNetworkingOptimizationORMRandomizationReflectionSharePointSpeechValidationVisual StudioWeb AppsRecent PostsCraig&rsquo;s Utility Library 3.2 ReleasedVersion 3.2 of Craig's Utility Library has been released.Craig&rsquo;s FrameworkThis post talks about Craig's Framework, a set of core classes that will be used in most of my MVC based projects going forward and how I plan to open source it.Personal Data ManagementStart of a project that I will be working on that will attempt to put the vast majority of my personal online life into one location.Craig&rsquo;s Utility Library 3.1 ReleasedNew version of Craig's Utility Library has been released.Small Changes, Big Speed UpsThis post talks about a basic change that I did to the bitmap extensions that gave me a large increase in speed.Recent CommentsAdjusting Contrast of an Image in C#Roey - It is better to use ColorMatri...Enabling JSONP in WCFBLop - Just to inform :""Cross doma...Viewstate Compression When Using AJAXTahri Akram - I'm using compressing/decompre...Creating a Zip File in C#Nicholas Carey - Why not use DotNetZip from htt...Major Rework Happeningmark foley - Hi!Your site is amazing. ...DISCLAIMERThe opinions expressed herein are my own personal opinions and do not represent my employer's view in any way.&copy; Copyright 2010", Result.ToString());
            }
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