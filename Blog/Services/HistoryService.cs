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
        const string PREV_LAST_URL_KEY = "PrevLastGetRequestPath";
        const string LAST_URL_KEY = "LastGetRequestPath";
        public bool HasLastUri => S.HttpContext.Session.Keys.Contains(LAST_URL_KEY);

        public HistoryService(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        public void SaveCurrentURL()
        {
            if (S.HttpContext.Request.Method == "GET")
            {
                var url = S.URIProvider.GetCurrentRequestURI();
                var lastURL = getByKeyOrIndex(LAST_URL_KEY);
                if (lastURL != url) // do nothing if we have current url in the history
                {
                    S.HttpContext.Session.Set(PREV_LAST_URL_KEY, lastURL.GetBytes(Encoding.UTF8));
                    S.HttpContext.Session.Set(LAST_URL_KEY, url.GetBytes(Encoding.UTF8));
                }
            }
        }

        /// <summary>
        /// Gets last not same, last get request URL
        /// </summary>
        /// <returns></returns>
        public string GetLastURL()
        {
            var previousURL = getByKeyOrIndex(PREV_LAST_URL_KEY);
            var lastURL = getByKeyOrIndex(LAST_URL_KEY);
            var currentURL = S.URIProvider.GetCurrentRequestURI();
            return new[] { lastURL, previousURL }.FirstOrAnyOrDefault(u => u != currentURL);
        }
        string getByKeyOrIndex(string key)
        {
            var has = S.HttpContext.Session.TryGetValue(key, out byte[] uri);
            if (!has)
            {
                return S.URIProvider.GetURLToIndex();
            }

            return Encoding.UTF8.GetString(uri);
        }
    }
}
