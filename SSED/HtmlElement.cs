using System;
using System.Collections.Generic;
using System.Text;

namespace SSED
{
    public class HtmlElement
    {
        public readonly StyleBuilder Style = new();
        
        public string OpeningTag;
        public readonly Dictionary<string, string> Attributes = new();
        public string ClosingTag;

        public bool UseNewlines = true;
        
        public string TextContent;
        public readonly List<HtmlElement> Content = new();

        public string ToHtml(int indentLevel = 0)
        {
            var builder = new StringBuilder();

            ClosingTag ??= OpeningTag;

            if (!string.IsNullOrEmpty(OpeningTag))
            {
                builder.Append('<');
                builder.Append(OpeningTag);

                Attributes["style"] = Style.ToString();

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
                    var html = element.ToHtml(indentLevel + 1);

                    foreach (var line in html.Split(Environment.NewLine))
                        builder.Append(UseNewlines ? Indent(indentLevel, line) : line);

                    if (UseNewlines)
                        builder.AppendLine();
                }
            }
            else
            {
                builder.Append(UseNewlines ? Indent(indentLevel, TextContent) : TextContent);

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

        private string Indent(int count, string text)
        {
            return text; // TODO: Fix inconsistent HTML newlines
            // return string.Concat(string.Concat(Enumerable.Repeat('\t', count)), text);
        }
    }
}