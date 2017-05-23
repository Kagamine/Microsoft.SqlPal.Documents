using System.Collections.Generic;

namespace Microsoft.SqlPal.Documents.Models
{
    public class Page
    {
        public string Toc { get; set; }

        public string Content { get; set; }

        public string Endpoint { get; set; }

        public IDictionary<string, string> Nav { get; set; }

        public string Path { get; set; }
    }
}
