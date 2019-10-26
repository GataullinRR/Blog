using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    public class HistoryService : ServiceBase
    {
        const string LAST_URL_KEY = "LastGetRequestPath";
        public bool HasLastUri => httpContext.Session.Keys.Contains(LAST_URL_KEY);

        public HistoryService(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public void SaveCurrentURL()
        {
            if (httpContext.Request.Method == "GET")
            {
                var url = new UriBuilder
                {
                    Scheme = httpContext.Request.Scheme,
                    Host = httpContext.Request.Host.Host,
                    Port = httpContext.Request.Host.Port.GetValueOrDefault(80),
                    Path = httpContext.Request.Path.ToString(),
                    Query = httpContext.Request.QueryString.ToString()
                }.Uri.ToString();

                httpContext.Session.Set(LAST_URL_KEY, url.GetBytes(Encoding.UTF8));
            }
        }

        public string GetLastURL()
        {
            var has = httpContext.Session.TryGetValue(LAST_URL_KEY, out byte[] uri);
            if (!has)
            {
                // ToDo
            }

            return Encoding.UTF8.GetString(uri);
        }
    }
}
