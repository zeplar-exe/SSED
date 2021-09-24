using System.Linq;
using NUnit.Framework;
using SSED;

namespace SSED_Tests
{
    [TestFixture]
    public class ElementTests
    {
        [Test]
        public void TestParameterizedElement()
        {
            var properties = "property=\"value\"";
            var testString = $"BeginContent EMB^({properties}) EndContent";

            var stream = PageParser.Parse(testString);
            Assert.True(((ParameterizedElement)stream.Content.Elements[1]).Parameters["property"] == "value");
        }
    }
}