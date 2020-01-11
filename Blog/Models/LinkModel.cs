using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class LinkModel
    {
        public LinkModel(string uRI, string content)
        {
            URI = uRI ?? throw new ArgumentNullException(nameof(uRI));
            Content = content ?? throw new ArgumentNullException(nameof(content));
        }

        public string URI { get; }
        public string Content { get; }
    }
}
