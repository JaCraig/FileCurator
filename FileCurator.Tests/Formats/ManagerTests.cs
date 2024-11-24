using FileCurator.Formats;
using FileCurator.Formats.Interfaces;
using FileCurator.Tests.BaseClasses;
using Microsoft.Extensions.DependencyInjection;
using System.IO;
using Xunit;

namespace FileCurator.Tests.Formats
{
    public class ManagerTests : TestBaseClass<Manager>
    {
        public ManagerTests()
        {
            TestObject = null;
        }

        public static readonly TheoryData<string, string> FormatDataByFileContents = new()
        {
            {"TestCSV.csv","Text" },
            {"TestDefault.boop","Text" },
            {"TestHTML.htm","Text" },
            {"TestICal.ics","ICal" },
            {"TestMHTML.mht","MIME" },
            {"TestEml.eml","MIME" },
            {"TestMSG.msg","MSG" },
            {"TestTXT.txt","Text" },
            {"TestVCal.vcs","ICal" },
            {"TestVCF.vcf","vCard" },
            {"TestPPSX.ppsx","PowerPoint" },
            {"TestPPTX.pptx","PowerPoint" },
            {"TestXLSX.xlsx","Excel" },
            {"TestDOCX.docx","Word" },
            {"TestRSS2.rss","RSS" },
            {"TestRSS.rss","RSS" }
        };

        public static readonly TheoryData<string, string, string> FormatDataByMimeType = new()
        {
            {"TestDOCX.docx","Word","application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            {"TestDOCX.docx","Word","application/msword" },
            {"TestCSV.csv","Delimited files","text/csv" },
            {"TestDefault.boop","Text","text/boop" },
            {"TestHTML.htm","HTML","text/html" },
            {"TestHTML.htm","HTML","text/html; charset=utf-8" },
            {"TestICal.ics","ICal","text/calendar" },
            {"TestMHTML.mht","MIME","message/rfc822" },
            {"TestEml.eml","MIME","message/rfc822" },
            {"TestMSG.msg","MSG","APPLICATION/VND.MS-OUTLOOK" },
            {"TestPPSX.ppsx","PowerPoint","application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
            {"TestPPTX.pptx","PowerPoint","application/vnd.openxmlformats-officedocument.presentationml.slideshow" },
            {"TestRSS.rss","RSS","APPLICATION/RSS+XML" },
            {"TestRSS2.rss","RSS","APPLICATION/RSS+XML" },
            {"TestTXT.txt","Text","text/plain" },
            {"TestVCal.vcs","VCal","text/x-vcalendar" },
            {"TestVCF.vcf","vCard","text/vcard" },
            {"TestXLSX.xlsx","Excel","application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            {"TestXML.xml","XML","text/xml" }
        };

        public static readonly TheoryData<string, string> FormatDataByName = new()
        {
            {"TestDOCX.docx","Word" },
            {"TestCSV.csv","Delimited files" },
            {"TestHTML.htm","HTML" },
            {"TestICal.ics","ICal" },
            {"TestMHTML.mht","MIME" },
            {"TestEml.eml","MIME" },
            {"TestPPSX.ppsx","PowerPoint" },
            {"TestPPTX.pptx","PowerPoint" },
            {"TestRSS.rss","RSS" },
            {"TestRSS2.rss","RSS" },
            {"TestTXT.txt","Text" },
            {"TestVCal.vcs","VCal" },
            {"TestVCF.vcf","vCard" },
            {"TestXLSX.xlsx","Excel" },
            {"TestXML.xml","XML" }
        };

        [Theory]
        [MemberData(nameof(FormatDataByName))]
        public void FindFormatByName(string fileName, string expectedFormat)
        {
            var TestObject = new Manager(GetServiceProvider().GetServices<IFormat>());
            IFormat Format = TestObject.FindFormat("./TestData/" + fileName, null);
            Assert.Equal(expectedFormat, Format.DisplayName);
        }

        [Theory]
        [MemberData(nameof(FormatDataByFileContents))]
        public void FindFormatByStreamNoMimeType(string fileName, string expectedFormat)
        {
            var TestObject = new Manager(GetServiceProvider().GetServices<IFormat>());
            try
            {
                using FileStream TempFile = File.OpenRead("./TestData/" + fileName);
                IFormat Format = TestObject.FindFormat(TempFile, "");
                Assert.Equal(expectedFormat, Format.DisplayName);
            }
            catch { }
        }

        [Theory]
        [MemberData(nameof(FormatDataByMimeType))]
        public void FindFormatByStreamWithMimeType(string fileName, string expectedFormat, string mimeType)
        {
            var TestObject = new Manager(GetServiceProvider().GetServices<IFormat>());
            using FileStream TempFile = File.OpenRead("./TestData/" + fileName);
            IFormat Format = TestObject.FindFormat(TempFile, mimeType);
            Assert.Equal(expectedFormat, Format.DisplayName);
        }
    }
}