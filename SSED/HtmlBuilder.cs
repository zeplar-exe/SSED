using System;
using System.Collections.Generic;
using System.Linq;

namespace SSED
{
    public class HtmlBuilder
    {
        private readonly HtmlElement root;
        private readonly Navigator<HtmlElement> navigator;

        public HtmlElement CurrentElement => navigator.Current;

        public HtmlBuilder(HtmlElement root)
        {
            this.root = root;
            navigator = new Navigator<HtmlElement>(root);
        }

        public void StartElement(string openingTag)
        {
            var element = new HtmlElement { OpeningTag = openingTag };
            
            CurrentElement?.Content.Add(element);
            navigator.MoveTo(element);
        }

        public void EndElement(string closingTag)
        {
            CurrentElement.ClosingTag = closingTag;
            navigator.MoveLast();
        }

        public void EndElement()
        {
            navigator.MoveLast();
        }

        public void AppendElement(HtmlElement element)
        {
            CurrentElement.Content.Add(element);
        }

        public void AppendText(string text)
        {
            CurrentElement.Content.Add(new HtmlElement { TextContent = text });
            CurrentElement.TextContent += text;
        }

        public HtmlDocument ToDocument() => new(root);

        private class Navigator<TItem>
        {
            private readonly List<TItem> previous = new();

            public TItem Current;

            public Navigator(TItem start = default)
            {
                Current = start;
            }

            public void MoveTo(TItem item)
            {
                previous.Add(Current);
                Current = item;
            }

            public void MoveLast()
            {
                Current = previous.LastOrDefault();
                previous.Remove(Current);
            }
        }
    }
}