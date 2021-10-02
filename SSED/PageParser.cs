using System;
using System.Linq;
using Jammo.ParserTools;

namespace SSED
{
    public static class PageParser
    {
        public static PageStream Parse(string input)
        {
            var stream = new PageStream();
            var state = new StateMachine<ParserState>(ParserState.Any);
            var lexer = new Lexer(input);

            foreach (var token in lexer)
            {
                switch (state.Current)
                {
                    case ParserState.Any:
                    {
                        switch (token.RawToken)
                        {
                            case "Guid":
                                state.MoveTo(ParserState.Guid);
                                break;
                            case "Title":
                                state.MoveTo(ParserState.Title);
                                break;
                            case "BeginContent":
                                state.MoveTo(ParserState.Content);
                                break;
                        }
                        
                        break;
                    }
                    case ParserState.Guid:
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        if (token.Is(LexerTokenId.Space) || token.Is(LexerTokenId.NewLine))
                            break;

                        if (!token.Is(LexerTokenId.DoubleQuote))
                        {
                            state.MoveLast();
                            break;
                        }

                        var guid = lexer.TakeWhile(b => !b.Is(LexerTokenId.DoubleQuote));
                        
                        stream.Guid = string.Concat(guid.Select(g => g.RawToken));
                        
                        state.MoveLast();
                        
                        break;
                    }
                    case ParserState.Title:
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        lexer.TakeWhile(b => !b.Is(LexerTokenId.DoubleQuote));
                        lexer.Skip();
                        
                        var title = lexer
                            .TakeWhile(b => !b.Is(LexerTokenId.DoubleQuote));

                        stream.Title = string.Concat(title.Select(t => t.RawToken));
                        
                        state.MoveLast();
                        
                        break;
                    }
                    case ParserState.Content:
                    {
                        stream.Content = SsedParser.Parse(
                            string.Concat(lexer.TakeWhile(t => t.RawToken != "EndContent")));

                        lexer.Skip();
                        
                        break;
                    }
                }
            }

            return stream;
        }

        private enum ParserState
        {
            Any,
            Guid,
            Title,
            Content
        }
    }
}