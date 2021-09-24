using System.Collections.Generic;
using System.Text;

namespace SSED
{
    public class HtmlElement
    {
        public string OpeningTag;
        public string ClosingTag;

        public bool UseNewlines = true;
        
        public string TextContent;
        public readonly List<HtmlElement> Content = new();

        public string ToHtml()
        {
            var builder = new StringBuilder();

            builder.Append(OpeningTag);

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

            builder.Append(ClosingTag);

            return builder.ToString();
        }
    }
}