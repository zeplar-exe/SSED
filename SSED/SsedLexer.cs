using System.Collections.Generic;
using System.Linq;
using Jammo.ParserTools;

namespace SSED
{
    public static partial class SsedParser
    {
        private static IEnumerable<SsedToken> Lex(string input)
        {
            var lexer = new Lexer(input);
            var currentToken = new Token();
            var preceding = new LinkedList<LexerToken>();

            foreach (var token in lexer)
            {
                switch (token.Id)
                {
                    case LexerTokenId.Space:
                    {
                        if (!string.IsNullOrEmpty(currentToken.Text))
                            yield return new TextToken(currentToken.Text, SsedTokenId.Text);

                        yield return new TextToken(token.RawToken, SsedTokenId.Whitespace);
                        currentToken = new Token();
                        
                        break;
                    }
                    case LexerTokenId.NewLine:
                    {
                        if (!string.IsNullOrEmpty(currentToken.Text))
                            yield return new TextToken(currentToken.Text, SsedTokenId.Text);

                        yield return new TextToken(token.RawToken, SsedTokenId.Newline);
                        currentToken = new Token();
                    
                        break;
                    }
                    case LexerTokenId.Caret:
                    {
                        if (lexer.PeekNext().Is(LexerTokenId.LeftParenthesis) &&
                            (!preceding.First?.Value.Is(LexerTokenId.Backslash) ?? false))
                        {
                            lexer.Skip();
                            
                            var prefix = currentToken.Text;
                            currentToken = new Token();
                            
                            foreach (var specialTextToken in lexer)
                            {
                                if (specialTextToken.Is(LexerTokenId.RightParenthesis))
                                {
                                    if (preceding.Any())
                                        if (!preceding.First.Value.Is(LexerTokenId.Backslash))
                                            break;
                                }

                                currentToken.Append(specialTextToken.RawToken);
                                preceding.AddFirst(specialTextToken);
                            }

                            yield return new ElementToken(prefix, currentToken.Text);
                            currentToken = new Token();
                        }
                        else
                        {
                            currentToken.Append(token.RawToken);
                        }
                        
                        break;
                    }
                    default:
                    {
                        currentToken.Append(token.RawToken);
                        
                        break;
                    }
                }

                preceding.AddFirst(token);
            }
            
            if (!string.IsNullOrEmpty(currentToken.Text))
                yield return new TextToken(currentToken.Text, SsedTokenId.Text);
        }

        private class Token
        {
            public string Text { get; private set; }

            public Token(string text = "")
            {
                Text = text;
            }

            public void Append(string text)
            {
                Text += text;
            }

            public override string ToString()
            {
                return Text;
            }
        }
    }

    public abstract class SsedToken
    {
        public readonly SsedTokenId Id;

        public SsedToken(SsedTokenId id)
        {
            Id = id;
        }

        public abstract override string ToString();
    }

    public class TextToken : SsedToken
    {
        public readonly string Text;
        
        public TextToken(string text, SsedTokenId id) : base(id)
        {
            Text = text;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class ElementToken : SsedToken
    {
        public readonly string Prefix;
        public readonly string Content;

        public ElementToken(string prefix, string content) : base(SsedTokenId.Element)
        {
            Prefix = prefix;
            Content = content;
        }

        public override string ToString()
        {
            return $"{Prefix}^({Content})";
        }
    }

    public enum SsedTokenId
    {
        Text, Element,
        Newline, Whitespace
    }
}