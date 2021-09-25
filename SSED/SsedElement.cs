using System;
using System.Collections.Generic;
using System.Linq;
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
            throw new NotImplementedException();
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
                ClosingTag = "</br>"
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
                OpeningTag = "<b>",
                TextContent = RawText,
                ClosingTag = "</b>"
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
                OpeningTag = "<i>",
                TextContent = RawText,
                ClosingTag = "</i>"
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
                OpeningTag = "<ins>",
                TextContent = RawText,
                ClosingTag = "</ins>"
            };
        }
    }
}