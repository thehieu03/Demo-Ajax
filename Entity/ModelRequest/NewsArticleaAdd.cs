using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.ModelRequest
{
    public class NewsArticleaAdd
    {
        public string? NewsTitle { get; set; }
        public string Headline { get; set; } = string.Empty;
        public string? NewsContent { get; set; }
        public string? NewsSource { get; set; }
        public short? CategoryId { get; set; }
        public bool? NewsStatus { get; set; }
    }
}
