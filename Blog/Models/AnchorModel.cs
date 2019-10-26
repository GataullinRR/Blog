using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class AnchorModel
    {
        public string Page { get; private set; }
        public string Text { get; private set; }
        public object RouteId { get; private set; } = null;
        public string RouteIdParameterName { get; private set; } = "id";
        public bool IsShown { get; private set; } = true;

        public string Controller { get; private set; }
        public string Action { get; private set; }

        private AnchorModel() { }

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

        public AnchorModel(string text, string page, string routeIdParameterName, object routeId, bool isShown)
        {
            Text = text;
            Page = page;
            RouteId = routeId;
            RouteIdParameterName = routeIdParameterName;
            IsShown = isShown;
        }

        public static AnchorModel ToController(string text, string controller, string action, string routeIdParameterName, object routeId, bool isShown)
        {
            return new AnchorModel()
            {
                Text = text,
                Controller = controller,
                Action = action,
                RouteIdParameterName = routeIdParameterName,
                RouteId = routeId,
                IsShown = isShown
            };
        }
    }
}
