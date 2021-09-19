using System;
using System.Collections.Generic;
using System.Linq;
using Jammo.ParserTools;

namespace SSED
{
    public static class SiteParser
    {
        public static SiteStream Parse(string input)
        {
            input = input.Replace("\t", string.Concat(Enumerable.Repeat(' ', 4))).Trim();
            
            var stream = new SiteStream();
            var tokenizer = new Tokenizer(input);
            var state = new StateMachine<ParserState>(ParserState.Any);
            
            foreach (var token in tokenizer)
            {
                switch (state.Current)
                {
                    case ParserState.Any:
                    {
                        switch (token.Text)
                        {
                            case "Version":
                            {
                                state.MoveTo(ParserState.VersionDefinition);
                                break;
                            }
                            case "LandingPage":
                            {
                                state.MoveTo(ParserState.LandingPageDefinition);
                                break;
                            }
                            case "BeginPages":
                            {
                                state.MoveTo(ParserState.PageDefinitions);
                                break;
                            }
                            case "BeginSidebar":
                            {
                                state.MoveTo(ParserState.SidebarDefinition);
                                break;
                            }
                        }
                        
                        break;
                    }
                    case ParserState.VersionDefinition:
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        tokenizer.TakeWhile(b => b.Type is BasicTokenType.Whitespace or BasicTokenType.Newline);
                        
                        var version = tokenizer
                            .TakeWhile(b => b.Type is 
                                BasicTokenType.Alphabetical or
                                BasicTokenType.Numerical or 
                                BasicTokenType.Punctuation);

                        stream.SsedVersion = string.Concat(version.Select(v => v.Text));
                        
                        state.MoveLast();
                        
                        break;
                    }
                    case ParserState.LandingPageDefinition:
                    {
                        // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                        tokenizer.TakeWhile(b => b.Type is BasicTokenType.Whitespace or BasicTokenType.Newline);
                        
                        var guid = tokenizer
                            .TakeWhile(b => b.Type is 
                                BasicTokenType.Alphabetical or
                                BasicTokenType.Numerical or 
                                BasicTokenType.Punctuation);

                        stream.LandingPageGuid = string.Concat(guid.Select(g => g.Text));
                        
                        state.MoveLast();
                        
                        break;   
                    }
                    case ParserState.PageDefinitions:
                    {
                        if (token.Text == "EndPages")
                        {
                            state.MoveLast();
                            break;
                        }

                        foreach (var pageToken in tokenizer)
                        {
                            if (pageToken.Type is BasicTokenType.Whitespace or BasicTokenType.Newline)
                                continue;

                            if (pageToken.Text == "(")
                            {
                                state.MoveTo(ParserState.PageDefinition);
                                break;
                            }
                        }

                        break;
                    }
                    case ParserState.PageDefinition:
                    {
                        var location = token.ToString();
                        
                        foreach (var pageToken in tokenizer)
                        {
                            if (pageToken.Text == ")")
                            {
                                stream.PageLocations.Add(location);
                                state.MoveLast();
                                break;
                            }

                            location += pageToken.Text;
                        }

                        break;
                    }
                    case ParserState.SidebarDefinition:
                    {
                        if (token.Text == "EndSidebar")
                        {
                            state.MoveLast();
                            break;
                        }
                        
                        break;
                    }
                }
            }

            return stream;
        }

        private enum ParserState
        {
            Any,
            VersionDefinition,
            LandingPageDefinition,
            PageDefinitions,
            PageDefinition,
            SidebarDefinition,
        }
    }
}