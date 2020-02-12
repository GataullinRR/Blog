using System;

namespace Blog.Models
{
    public class IndexPageButtonModel
    {
        public IndexPageButtonModel(int pageIndex, string searhQuery, string content)
        {
            PageIndex = pageIndex;
            SearhQuery = searhQuery;
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public int PageIndex { get; set; }
        public string SearhQuery { get; set; }
        public string Content { get; set; }
    }
}
