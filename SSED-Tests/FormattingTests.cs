using System;
using System.Linq;
using NUnit.Framework;
using SSED;

namespace SSED_Tests
{
    [TestFixture]
    public class FormattingTests
    {
        [TestFixture]
        public class SiteTests
        {
            [Test]
            public void TestSiteVersion()
            {
                var version = "12a0";
                var testString = $"SomeProperty\nVersion {version}";

                var stream = SiteParser.Parse(testString);
                Assert.True(stream.SsedVersion == version);
            }

            [Test]
            public void TestSiteLandingPage()
            {
                var guid = Guid.NewGuid().ToString();
                var testString = $"LandingPage {guid}";

                var stream = SiteParser.Parse(testString);
                Assert.True(stream.LandingPageGuid == guid);
            }

            [Test]
            public void TestSitePages()
            {
                var page = "relative/location";
                var testString = $"BeginPages ({page}) EndPages";

                var stream = SiteParser.Parse(testString);
                Assert.True(stream.PageLocations.First() == page);
            }
        }

        [TestFixture]
        public class TestPage
        {
            [Test]
            public void TestPageTitle()
            {
                var title = "My Page";
                var testString = $"Title \"{title}\"";

                var stream = PageParser.Parse(testString);
                Assert.True(stream.Title == title);
            }

            [Test]
            public void TestPageGuid()
            {
                var guid = Guid.NewGuid().ToString();
                var testString = $"Guid \"{guid}\"";
                
                var stream = PageParser.Parse(testString);
                Assert.True(stream.Guid == guid);
            }

            [Test]
            public void TestPlainTextContent()
            {
                var content = "Some text, also, did I mention that I did x, y, AND z in the same day?";
                var testString = $"BeginContent {content} EndContent";

                var stream = PageParser.Parse(testString);
                Assert.True(stream.Content.Elements[0].ToString().Trim() == content);
            }

            [Test]
            public void TestBoldText()
            {
                var bold = "bold text";
                var content = $"Some plain text and some B^({bold}).";
                var testString = $"BeginContent {content} EndContent";

                var stream = PageParser.Parse(testString);
                Assert.True(stream.Content.Elements[1].GetType() == typeof(BoldText));
            }
            
            [Test]
            public void TestUnderlinedText()
            {
                var important = "important text";
                var content = $"Some B^(bold) text and some U^({important}).";
                var testString = $"BeginContent {content} EndContent";
                
                var stream = PageParser.Parse(testString);
                Assert.True(stream.Content.Elements[3].GetType() == typeof(UnderlinedText));
            }
        }
    }
}