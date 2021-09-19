using System;
using System.IO;
using Jammo.ParserTools;

namespace SSED
{
    public class PageStream : IParserStream
    {
        private FileStream stream;

        public bool IsInitialized => stream == null;
        public string FilePath => stream.Name;

        public string Title;
        public string Guid;

        public SsedDocument Content;
        
        public void Parse()
        {
            throw new NotImplementedException();
        }

        public void Write()
        {
            throw new NotImplementedException();
        }

        public void WriteTo(string path)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            stream?.Dispose();
        }
    }
}