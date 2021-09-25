using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammo.ParserTools;

namespace SSED
{
    public abstract class SsedElement
    {
        protected string RawText = string.Empty;

        internal static SsedElement FromPrefix(string prefix)
        {
            return prefix switch
            {
                "B" => new BoldText(),
                "I" => new ItalicText(),
                "U" => new UnderlinedText(),
                "EMB" => new Embed(),
                _ => new PlainText()
            };
        }
        
        internal virtual void Feed(LexerToken token)
        {
            RawText += token.Token.Text;
        }
        
        public abstract HtmlElement ToHtmlElement();

        public override string ToString()
        {
            return RawText;
        }
    }

    public abstract class ParameterizedElement : SsedElement
    {
        private readonly List<LexerToken> previous = new();
        private string currentKey;
        private bool definingParameter;
        private bool isEscaping;
        
        public readonly Dictionary<string, string> Parameters = new();

        internal override void Feed(LexerToken token)
        {
            if (token.Is(LexerTokenId.Backslash))
            {
                isEscaping = !isEscaping;
            }
            
            if (token.ToString() == "=")
            {
                var last = previous.LastOrDefault();
                
                if (last is not { Id: LexerTokenId.Alphabetic })
                    return;

                currentKey = last.Token.Text;
                
                Parameters[currentKey] = string.Empty;
            }
            else if (currentKey != null)
            {
                if (token.Is(LexerTokenId.DoubleQuote))
                {
                    if (!isEscaping)
                    {
                        definingParameter = !definingParameter;
                        
                        return;
                    }
                }

                if (definingParameter)
                    Parameters[currentKey] += token.ToString();
            }

            isEscaping = false;
            
            previous.Add(token);
        }
        
        public abstract override HtmlElement ToHtmlElement();
        public abstract override string ToString();
    }

    public class Embed : ParameterizedElement
    {
        public override HtmlElement ToHtmlElement()
        {
            var builder = new HtmlBuilder(new HtmlElement { OpeningTag = "div" });

            builder.StartElement("div");
            builder.CurrentElement.Attributes.Add("height", "40");
            builder.CurrentElement.Attributes.Add("width", "50%");

            var styleBuilder = new StringBuilder();
            // TODO: Create Style and StyleBuilder, grab styles using recursion

            if (Parameters.TryGetValue("headercolor", out string headerColor))
                styleBuilder.Append($"background-color:{headerColor};");

            styleBuilder.Append("margin:10;");
            
            builder.CurrentElement.Attributes.Add("style", styleBuilder.ToString());
            
            if (Parameters.TryGetValue("header", out var header))
                builder.CurrentElement.TextContent = header;

            styleBuilder.Clear();
            builder.EndElement("div");
            
            builder.StartElement("div");
            builder.CurrentElement.Attributes.Add("minheight", "100");

            if (Parameters.TryGetValue("contentcolor", out string contentColor)) // TODO: Allow -, _, etc in names
                styleBuilder.Append($"background-color:{contentColor};");
            
            builder.CurrentElement.Attributes.Add("style", styleBuilder.ToString());
            
            if (Parameters.TryGetValue("text", out var text))
                builder.CurrentElement.TextContent = text;
            
            builder.EndElement("div");
            
            return builder.CurrentElement;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }
    }
    
    public class LineBreak : SsedElement, IParagraphText
    {
        public override HtmlElement ToHtmlElement()
        {
            return new HtmlElement
            {
                UseNewlines = false,
                ClosingTag = "br"
            };
        }
    }

    public class PlainText : SsedElement, IParagraphText
    {
        public override HtmlElement ToHtmlElement()
        {
            return new HtmlElement
            {
                UseNewlines = false,
                TextContent = RawText
            };
        }
    }
    
    public class BoldText : SsedElement, IParagraphText
    {
        public override HtmlElement ToHtmlElement()
        {
            return new HtmlElement
            {
                UseNewlines = false,
                OpeningTag = "b",
                TextContent = RawText,
            };
        }
    }
    public class ItalicText : SsedElement, IParagraphText
    {
        public override HtmlElement ToHtmlElement()
        {
            return new HtmlElement
            {
                UseNewlines = false,
                OpeningTag = "i",
                TextContent = RawText,
            };
        }
    }
    public class UnderlinedText : SsedElement, IParagraphText
    {
        public override HtmlElement ToHtmlElement()
        {
            return new HtmlElement
            {
                UseNewlines = false,
                OpeningTag = "ins",
                TextContent = RawText,
            };
        }
    }
}