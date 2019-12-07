using Blog.Misc;
using Blog.Pages.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    public class ErrorsHandlerMiddleware
    {
        readonly RequestDelegate _next;

        public ErrorsHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var shouldRedirectToErrorPage = true;
            try
            {
                await _next(httpContext);
                shouldRedirectToErrorPage = httpContext.Response.StatusCode >= 400;
            }
            catch (UnauthorizedAccessException)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            catch (AuthenticationException)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            catch (NotFoundException)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            
            if (shouldRedirectToErrorPage)
            {
                var endpoint = getEndpoint(httpContext); 
                if (endpoint != null)
                {
                    var isAjaxEndpoint = endpoint.Metadata.GetMetadata<AJAXAttribute>() != null;
                    if (!isAjaxEndpoint) // For AJAX error page wont be rendered
                    {
                        var statusCode = httpContext.Response.StatusCode;
                        httpContext.Response.Clear();
                        httpContext.Response.Redirect($"/Errors/Error?code={statusCode}", false);
                    }
                }
            }
        }

        Endpoint getEndpoint(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Features.Get<IEndpointFeature>()?.Endpoint;
        }
    }
}
