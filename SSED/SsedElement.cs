using System.Xml.Linq;
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
                _ => new PlainText()
            };
        }
        
        internal virtual void Feed(BasicToken text)
        {
            RawText += text.Text;
        }
        
        public abstract HtmlElement ToHtmlElement();

        public override string ToString()
        {
            return RawText;
        }
    }
    
    public class PlainText : SsedElement, IParagraphText
    {
        public string Tag => "";

        public override HtmlElement ToHtmlElement()
        {
            return new HtmlElement
            {
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
                OpeningTag = "<ins>",
                TextContent = RawText,
                ClosingTag = "</pins>"
            };
        }
    }
}