﻿using System;
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
        public bool HasLastUri => Services.HttpContext.Session.Keys.Contains(LAST_URL_KEY);

        public HistoryService(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public void SaveCurrentURL()
        {
            if (Services.HttpContext.Request.Method == "GET")
            {
                var url = getCurrentURL();
                var lastURL = getByKeyOrIndex(LAST_URL_KEY);
                if (lastURL != url) // do nothing if we have current url in the history
                {
                    Services.HttpContext.Session.Set(PREV_LAST_URL_KEY, lastURL.GetBytes(Encoding.UTF8));
                    Services.HttpContext.Session.Set(LAST_URL_KEY, url.GetBytes(Encoding.UTF8));
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
            var currentURL = getCurrentURL();
            return new[] { lastURL, previousURL }.FirstOrAnyOrDefault(u => u != currentURL);
        }
        string getByKeyOrIndex(string key)
        {
            var has = Services.HttpContext.Session.TryGetValue(key, out byte[] uri);
            if (!has)
            {
                return getURLToIndex();
            }

            return Encoding.UTF8.GetString(uri);
        }

        string getCurrentURL()
        {
            return new UriBuilder
            {
                Scheme = Services.HttpContext.Request.Scheme,
                Host = Services.HttpContext.Request.Host.Host,
                Port = Services.HttpContext.Request.Host.Port.GetValueOrDefault(80),
                Path = Services.HttpContext.Request.Path.ToString(),
                Query = Services.HttpContext.Request.QueryString.ToString()
            }.Uri.ToString();
        }
        string getURLToIndex()
        {
            return new UriBuilder
            {
                Scheme = Services.HttpContext.Request.Scheme,
                Host = Services.HttpContext.Request.Host.Host,
                Port = Services.HttpContext.Request.Host.Port.GetValueOrDefault(80),
            }.Uri.ToString();
        }
    }
}
