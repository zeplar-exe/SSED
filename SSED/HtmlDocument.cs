using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SSED
{
    public class HtmlDocument
    {
        public readonly HtmlElement Root;

        public HtmlDocument(HtmlElement root)
        {
            Root = root;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            builder.AppendLine("<!DOCTYPE html>");
            builder.AppendLine(Root.ToHtml());

            return builder.ToString();
        }
    }
}