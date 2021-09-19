using System.Collections.Generic;

namespace SSED
{
    public class SidebarItem
    {
        public Link Link;
        public List<Link> NestedLinks = new();
    }
}