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
            var preceeding = new LinkedList<BasicToken>();

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
                        var prefix = preceeding.Last?.Previous;

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
                                    break;
                                
                                element.Feed(specialToken);
                            }
                        }
                        
                        stream.Elements.Add(element);
                        
                        break;
                    }
                }
                
                preceeding.AddFirst(token);
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
                _ => new PlainText()
            };
        }
        
        internal virtual void Feed(BasicToken text)
        {
            RawText += text.Text;
        }

        public override string ToString()
        {
            return RawText;
        }
    }

    public class PlainText : SsedElement
    {
        
    }
    
    public class BoldText : SsedElement { }
    public class ItalicText : SsedElement { }
    public class UnderlinedText : SsedElement { }
}