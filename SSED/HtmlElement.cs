using System.Collections.Generic;
using System.Text;

namespace SSED
{
    public class HtmlElement
    {
        public string OpeningTag;
        public string ClosingTag;
        
        public string TextContent;
        public readonly List<HtmlElement> Content = new();

        public string ToHtml()
        {
            var builder = new StringBuilder();

            builder.AppendLine(OpeningTag);

            if (TextContent == null)
            {
                foreach (var element in Content)
                    builder.AppendLine(element.ToHtml());
            }
            else
                builder.AppendLine(TextContent);
            
            builder.Append(ClosingTag);

            return builder.ToString();
        }
    }
}