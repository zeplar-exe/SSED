using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jammo.ParserTools;

namespace SSED
{
    public abstract class SsedElement
    {
        protected string RawText;

        protected SsedElement(string rawText)
        {
            RawText = rawText;
        }

        internal static SsedElement FromToken(ElementToken token)
        {
            return token.Prefix switch
            {
                "B" => new BoldText(token.Content),
                "I" => new ItalicText(token.Content),
                "U" => new UnderlinedText(token.Content),
                "EMB" => new Embed(token.Content),
                _ => new PlainText(token.Content)
            };
        }
        
        public abstract HtmlElement ToHtmlElement();

        public override string ToString()
        {
            return RawText;
        }
    }

    public abstract class ParameterizedElement : SsedElement
    {
        public readonly Dictionary<string, string> Parameters = new();
        
        protected ParameterizedElement(string rawText) : base(rawText)
        {
            var lexer = new Lexer(rawText);
            var preceding = new LinkedList<LexerToken>();
            var currentIdentifier = string.Empty;
            
            foreach (var token in lexer)
            {
                if (token.Is(LexerTokenId.Equals) && preceding.Any())
                {
                    if (!lexer.PeekNext().Is(LexerTokenId.DoubleQuote))
                        continue;
                    
                    lexer.Skip();
                    
                    if (lexer.PeekNext().Is(LexerTokenId.Space)) // TODO: Fix this bug
                        lexer.SkipWhile(t => t.Is(LexerTokenId.Space));
                    
                    Parameters[currentIdentifier] = string.Empty;

                    foreach (var valueToken in lexer)
                    {
                        if (valueToken.Is(LexerTokenId.DoubleQuote) && 
                            !(preceding.First?.Value.Is(LexerTokenId.Backslash) ?? false))
                            break;
                        
                        Parameters[currentIdentifier] += valueToken;

                        preceding.AddFirst(valueToken);
                    }
                }
                
                if (token.Token.Type != BasicTokenType.Symbol &&
                    !token.Is(LexerTokenId.Space) &&
                    !token.Is(LexerTokenId.NewLine))
                    currentIdentifier += token;
                else if (!token.Is(LexerTokenId.Underscore) && !token.Is(LexerTokenId.Minus))
                    currentIdentifier = string.Empty;

                preceding.AddFirst(token);
            }
        }

        public abstract override HtmlElement ToHtmlElement();
        public abstract override string ToString();
    }

    public class Embed : ParameterizedElement
    {
        public Embed(string rawText) : base(rawText)
        {
            
        }
        
        public override HtmlElement ToHtmlElement()
        {
            var builder = new HtmlBuilder(new HtmlElement { OpeningTag = "div" });
            {
                builder.StartElement("div");
                {
                    if (Parameters.TryGetValue("header-color", out var headerColor))
                        builder.AppendStyle($"background-color", headerColor);

                    builder.AppendStyle("margin-left", "10px");
                    builder.AppendStyle("margin-right", "10px");

                    if (Parameters.TryGetValue("header", out var header))
                        builder.CurrentElement.TextContent = header;

                    builder.EndElement("div");
                }

                builder.StartElement("div");
                {
                    builder.CurrentElement.Attributes.Add("minheight", "100");

                    if (Parameters.TryGetValue("content-color", out var contentColor))
                        builder.AppendStyle($"background-color", contentColor);
                    
                    builder.AppendStyle("margin-left", "10px");
                    builder.AppendStyle("margin-right", "10px");

                    if (Parameters.TryGetValue("text", out var text))
                        builder.CurrentElement.TextContent = text;

                    builder.EndElement("div");
                }
            }
            return builder.CurrentElement;
        }

        public override string ToString()
        {
            var builder = new StringBuilder();

            foreach (var param in Parameters)
            {
                builder.Append('{');
                builder.Append(param.Key);
                builder.Append(", ");
                builder.AppendLine(param.Value);
                builder.Append('}');
                builder.Append(", ");
            }
            
            return builder.ToString();
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

        public LineBreak(string rawText) : base(rawText)
        {
            
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

        public PlainText(string rawText) : base(rawText)
        {
            
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

        public BoldText(string rawText) : base(rawText)
        {
            
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

        public ItalicText(string rawText) : base(rawText)
        {
            
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

        public UnderlinedText(string rawText) : base(rawText)
        {
            
        }
    }
}