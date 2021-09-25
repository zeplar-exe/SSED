using System.Collections.Generic;
using Jammo.ParserTools;

namespace SSED
{
    public class SsedParser
    {
        public static SsedDocument Parse(string input)
        {
            var stream = new SsedDocument();
            var state = new StateMachine<ParserState>(ParserState.Text);
            var lexer = new Lexer(new Tokenizer(input));
            var preceding = new LinkedList<LexerToken>();

            SsedElement textElement = new PlainText();

            foreach (var token in lexer)
            {
                switch (state.Current)
                {
                    case ParserState.Text:
                    {
                        if (token.Is(LexerTokenId.Alphabetic) || 
                            token.Is(LexerTokenId.AlphaNumeric) ||
                            token.Is(LexerTokenId.Numeric))
                        {
                            if (lexer.PeekNext().Is(LexerTokenId.Caret))
                            {
                                state.MoveTo(ParserState.SpecialItem);
                                stream.Elements.Add(textElement);

                                textElement = new PlainText();
                                
                                break;
                            }
                        }

                        if (token.Is(LexerTokenId.NewLine))
                        {
                            stream.Elements.Add(textElement);
                            stream.Elements.Add(new LineBreak());

                            textElement = new PlainText();
                                
                            break;
                        }

                        textElement.Feed(token);
                        
                        break;
                    }
                    case ParserState.SpecialItem:
                    {
                        var prefix = preceding.First?.Value.Token.Text;

                        if (prefix == null)
                        {
                            state.MoveLast();
                            break;
                        }

                        if (!lexer.Next().Is(LexerTokenId.LeftParenthesis))
                        {
                            state.MoveLast();
                            break;
                        }

                        var element = SsedElement.FromPrefix(prefix);

                        var isEscaping = false;
                        foreach (var specialToken in lexer)
                        {
                            if (specialToken.Is(LexerTokenId.Backslash))
                            {
                                if (isEscaping)
                                {
                                    element.Feed(specialToken);
                                    isEscaping = false;
                                }
                                else
                                {
                                    isEscaping = true;
                                }
                            }
                            else
                            {
                                if (specialToken.Is(LexerTokenId.RightParenthesis) && !isEscaping)
                                {
                                    state.MoveLast();
                                    break;
                                }
                                
                                isEscaping = false;

                                element.Feed(specialToken);
                            }
                        }
                        
                        stream.Elements.Add(element);
                        
                        break;
                    }
                }
                
                preceding.AddFirst(token);
            }
            
            if (!string.IsNullOrEmpty(textElement.ToString()))
                stream.Elements.Add(textElement);
            
            return stream;
        }

        private enum ParserState
        {
            Text,
            SpecialItem
        }
    }
}