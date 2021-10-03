using System.Collections.Generic;
using System.Text;

namespace SSED
{
    public class StyleBuilder
    {
        private readonly StringBuilder builder = new();
        private readonly Dictionary<string, string> style = new();
        
        public IReadOnlyDictionary<string, string> Style => style;

        public void Append(string id, string value)
        {
            builder.Append(id);
            builder.Append(":");
            builder.Append(value);
            builder.Append(";");
            
            style.Add(id, value);
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}