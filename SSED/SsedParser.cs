using System;
using System.Collections.Generic;
using System.Xml.Linq;
using Jammo.ParserTools;

namespace SSED
{
    public class SsedParser
    {
        public static SsedDocument Parse(string input)
        {
            var stream = new SsedDocument();
            var state = new StateMachine<ParserState>(ParserState.Text);
            var tokenizer = new Tokenizer(input);
            var preceding = new LinkedList<BasicToken>();

            SsedElement textElement = new PlainText();

            foreach (var token in tokenizer)
            {
                switch (state.Current)
                {
                    case ParserState.Text:
                    {
                        if (token.Type is BasicTokenType.Alphabetical or BasicTokenType.Numerical)
                        {
                            if (tokenizer.PeekNext().Text == "^")
                            {
                                state.MoveTo(ParserState.SpecialItem);
                                stream.Elements.Add(textElement);

                                textElement = new PlainText();
                                
                                break;
                            }
                        }

                        textElement.Feed(token);
                        
                        break;
                    }
                    case ParserState.SpecialItem:
                    {
                        var prefix = preceding.First;

                        if (prefix == null)
                        {
                            state.MoveLast();
                            break;
                        }

                        if (tokenizer.Next().Text != "(")
                        {
                            state.MoveLast();
                            break;
                        }

                        var element = SsedElement.FromPrefix(prefix.Value.Text);

                        var isEscaping = false;
                        foreach (var specialToken in tokenizer)
                        {
                            if (specialToken.Text == "\\")
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
                                if (specialToken.Text == ")" && !isEscaping)
                                {
                                    state.MoveLast();
                                    break;
                                }

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