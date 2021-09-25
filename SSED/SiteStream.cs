using System;
using System.Collections.Generic;
using System.IO;
using Jammo.ParserTools;

namespace SSED
{
    public class SiteStream : IParserStream
    {
        private readonly FileStream stream;

        public bool IsInitialized => stream == null;
        public string FilePath => stream.Name;

        public string SsedVersion = "";
        public string LandingPageGuid;

        public readonly List<string> PageLocations = new();
        public readonly List<Link> SidebarLinks = new();

        public SiteStream(FileStream stream = null)
        {
            this.stream = stream;
        }
        
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