using System;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Jammo.ParserTools;

namespace SSED
{
    public class PageStream : IParserStream
    {
        private FileStream stream;

        public bool IsInitialized => stream == null;
        public string FilePath => stream.Name;

        public string Title;
        public string Guid;

        public SsedDocument Content;
        
        public void Parse()
        {
            throw new NotImplementedException();
        }

        public void Write()
        {
            throw new NotImplementedException();
        }

        public void WriteTo(string path)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            stream?.Dispose();
        }

        public HtmlDocument ToHtml()
        {
            var builder = new HtmlBuilder();
            
            builder.StartElement("<meta>");

            if (Title != null)
            {
                builder.StartElement("<title>");
                builder.CurrentElement.TextContent = Title;
                builder.EndElement("</title>");
            }

            builder.EndElement("</meta>");
            
            
            builder.StartElement("<body>");
            builder.StartElement("<p>");

            foreach (var element in Content.Elements)
            {
                builder.AppendElement(element.ToHtmlElement());
            }
            
            builder.EndElement("</p>");
            builder.EndElement("</body>");

            return builder.ToDocument();
        }
    }
}