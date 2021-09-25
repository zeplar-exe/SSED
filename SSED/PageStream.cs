using System;
using System.Collections.Generic;
using System.IO;
using Jammo.ParserTools;

namespace SSED
{
    public class PageStream : IParserStream
    {
        private readonly FileStream stream;

        public bool IsInitialized => stream == null;
        public string FilePath => stream.Name;

        public string Title;
        public string Guid;

        public SsedDocument Content;

        public PageStream(FileStream stream = null)
        {
            this.stream = stream;
        }

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

            var preceding = new LinkedList<SsedElement>();

            foreach (var element in Content.Elements)
            {
                switch (element)
                {
                    case IParagraphText:
                    {
                        if (preceding.First is null)
                        {
                            builder.StartElement("<p>");
                            builder.CurrentElement.UseNewlines = false;
                        }
                        else if (preceding.First.Value is not IParagraphText)
                        {
                            builder.EndElement();
                            builder.StartElement("<p>");
                        }
                        
                        break;
                    }
                    default:
                    {
                        if (builder.CurrentElement.OpeningTag != "<body>")
                            builder.EndElement();

                        break;
                    }
                }
                
                builder.AppendElement(element.ToHtmlElement());
                preceding.AddFirst(element);
            }
            
            builder.EndElement();
            builder.EndElement("</body>");

            return builder.ToDocument();
        }
    }
}