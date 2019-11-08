using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    public class UnathorizedExceptionToStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public UnathorizedExceptionToStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (UnauthorizedAccessException)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            catch
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
        }
    }
}
