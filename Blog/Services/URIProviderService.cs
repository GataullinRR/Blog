using Blog.Attributes;
using Blog.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class URIProviderService : ServiceBase
    {
        public URIProviderService(ServiceLocator services) : base(services)
        {

        }

        public string GetURLToIndex()
        {
            return new UriBuilder
            {
                Scheme = S.HttpContext.Request.Scheme,
                Host = S.HttpContext.Request.Host.Host,
                Port = S.HttpContext.Request.Host.Port.GetValueOrDefault(80),
            }.Uri.ToString();
        }

        public string GetCurrentRequestURI()
        {
            return new UriBuilder
            {
                Scheme = S.HttpContext.Request.Scheme,
                Host = S.HttpContext.Request.Host.Host,
                Port = S.HttpContext.Request.Host.Port.GetValueOrDefault(80),
                Path = S.HttpContext.Request.Path.ToString(),
                Query = S.HttpContext.Request.QueryString.ToString()
            }.Uri.ToString();
        }
    }
}
