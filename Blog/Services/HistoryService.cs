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
        public bool HasLastUri => Services.HttpContext.Session.Keys.Contains(LAST_URL_KEY);

        public HistoryService(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public void SaveCurrentURL()
        {
            if (Services.HttpContext.Request.Method == "GET")
            {
                var url = new UriBuilder
                {
                    Scheme = Services.HttpContext.Request.Scheme,
                    Host = Services.HttpContext.Request.Host.Host,
                    Port = Services.HttpContext.Request.Host.Port.GetValueOrDefault(80),
                    Path = Services.HttpContext.Request.Path.ToString(),
                    Query = Services.HttpContext.Request.QueryString.ToString()
                }.Uri.ToString();

                Services.HttpContext.Session.Set(LAST_URL_KEY, url.GetBytes(Encoding.UTF8));
            }
        }

        public string GetLastURL()
        {
            var has = Services.HttpContext.Session.TryGetValue(LAST_URL_KEY, out byte[] uri);
            if (!has)
            {
                // ToDo
            }

            return Encoding.UTF8.GetString(uri);
        }
    }
}
