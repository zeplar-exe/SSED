using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSED
{
    public class HtmlElement
    {
        public string OpeningTag;
        public readonly Dictionary<string, string> Attributes = new();
        public string ClosingTag;

        public bool UseNewlines = true;
        
        public string TextContent;
        public readonly List<HtmlElement> Content = new();

        public string ToHtml()
        {
            var builder = new StringBuilder();

            ClosingTag ??= OpeningTag;
            
            if (!string.IsNullOrEmpty(OpeningTag))
            {
                builder.Append('<');
                builder.Append(OpeningTag);

                foreach (var (key, value) in Attributes)
                {
                    builder.Append(' ');
                    builder.Append(key);
                    
                    builder.Append('=');
                    
                    builder.Append('"');
                    builder.Append(value);
                    builder.Append('"');
                }
                
                builder.Append('>');
            }

            if (UseNewlines)
                builder.AppendLine();

            if (TextContent == null)
            {
                foreach (var element in Content)
                {
                    builder.Append(element.ToHtml());

                    if (UseNewlines)
                        builder.AppendLine();
                }
            }
            else
            {
                builder.Append(TextContent);
                
                if (UseNewlines)
                    builder.AppendLine();
            }

            if (!string.IsNullOrEmpty(ClosingTag))
            {
                builder.Append("</");
                builder.Append(ClosingTag);
                builder.Append(">");
            }

            return builder.ToString();
        }
    }
}