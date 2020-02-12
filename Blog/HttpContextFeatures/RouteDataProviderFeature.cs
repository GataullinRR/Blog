using System.Collections.Generic;

namespace Blog.HttpContextFeatures
{
    public class RouteDataProviderFeature
    {
        public IEnumerable<KeyValuePair<string, object>> RouteData { get; }

        public RouteDataProviderFeature(IEnumerable<KeyValuePair<string, object>> routeData)
        {
            RouteData = routeData;
        }
    }
}
