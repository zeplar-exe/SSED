using System;
using System.Collections.Generic;
using System.Linq;
using Jammo.ParserTools;

namespace SSED
{
    public static partial class SsedParser
    {
        public static SsedDocument Parse(string input)
        {
            var stream = new SsedDocument();
            var text = string.Empty;
            
            foreach (var token in Lex(input))
            {
                Console.WriteLine(token.Id);
                Console.WriteLine(token.ToString());
                switch (token.Id)
                {
                    case SsedTokenId.Text:
                    case SsedTokenId.Whitespace:
                    {
                        text += token.ToString();
                        
                        break;
                    }
                    case SsedTokenId.Element when token is ElementToken special:
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            stream.Elements.Add(new PlainText(text));
                            text = string.Empty;
                        }
                        
                        stream.Elements.Add(SsedElement.FromToken(special));
                        
                        break;
                    }
                    case SsedTokenId.Newline:
                    {
                        if (!string.IsNullOrEmpty(text))
                        {
                            stream.Elements.Add(new PlainText(text));
                            text = string.Empty;
                        }
                        
                        stream.Elements.Add(new LineBreak(token.ToString()));
                        
                        break;
                    }
                }
            }
            
            if (!string.IsNullOrEmpty(text))
                stream.Elements.Add(new PlainText(text));

            return stream;
        }
    }
}