using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class AnchorModel
    {
        public string Page { get; }
        public string Text { get; }
        public object RouteId { get; } = null;
        public bool IsShown { get; } = true;

        public AnchorModel(string text, string page, object routeId, bool isShown)
        {
            Text = text;
            Page = page;
            RouteId = routeId;
            IsShown = isShown;
        }

        public AnchorModel(string text, string page)
        {
            Text = text;
            Page = page;
        }
    }
}
