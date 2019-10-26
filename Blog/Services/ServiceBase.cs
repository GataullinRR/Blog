using DBModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class ServiceBase
    {
        readonly IServiceProvider _serviceProvider;

        readonly Lazy<IHttpContextAccessor> _httpContext;
        readonly Lazy<BlogContext> _db;

        protected HttpContext httpContext => _httpContext.Value.HttpContext;
        protected BlogContext db => _db.Value;

        public ServiceBase(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _httpContext = _serviceProvider.GetLazyService<IHttpContextAccessor>();
            _db = _serviceProvider.GetLazyService<BlogContext>();
        }
    }
}
