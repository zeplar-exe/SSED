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
            var tokenizer = new Tokenizer(input);

            foreach (var token in tokenizer)
            {
                switch (state.Current)
                {
                    case ParserState.Any:
                    {
                        switch (token.Text)
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
                        tokenizer.TakeWhile(b => b.Type is BasicTokenType.Whitespace or BasicTokenType.Newline);
                        
                        var guid = tokenizer
                            .TakeWhile(b => b.Type is 
                                BasicTokenType.Alphabetical or
                                BasicTokenType.Numerical or 
                                BasicTokenType.Punctuation);

                        stream.Guid = string.Concat(guid.Select(g => g.Text)).Trim('\"');
                        
                        state.MoveLast();
                        
                        break;
                    }
                    case ParserState.Title:
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        tokenizer.TakeWhile(b => b.Text != "\"");
                        tokenizer.Next();
                        
                        var title = tokenizer
                            .TakeWhile(b => b.Text != "\"");

                        stream.Title = string.Concat(title.Select(t => t.Text));
                        
                        state.MoveLast();
                        
                        break;
                    }
                    case ParserState.Content:
                    {
                        stream.Content = SsedParser.Parse(
                            string.Concat(tokenizer.TakeWhile(t => t.Text != "EndContent")));
                        
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