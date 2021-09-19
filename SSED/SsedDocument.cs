using System.Collections.Generic;
using System.Xml.Linq;

namespace SSED
{
    public class SsedDocument
    {
        public List<SsedElement> Elements = new();
        
        public XDocument ToHtml()
        {
            var root = new XElement("html");

            return root.Document;
        }
    }
}