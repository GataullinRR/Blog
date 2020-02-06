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
            var statusCode = HttpStatusCode.OK;
            try
            {
                await _next(httpContext);
                shouldRedirectToErrorPage = httpContext.Response.StatusCode >= 400;
            }
            catch (UnauthorizedAccessException)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (AuthenticationException)
            {
                statusCode = HttpStatusCode.Unauthorized;
            }
            catch (NotFoundException)
            {
                statusCode = HttpStatusCode.NotFound;
            }
            catch (Exception ex)
            {
                statusCode = HttpStatusCode.InternalServerError;
            }
            
            if (shouldRedirectToErrorPage)
            {
                var endpoint = getEndpoint(httpContext); 
                if (endpoint != null)
                {
                    var isAjaxEndpoint = endpoint.Metadata.GetMetadata<AJAXAttribute>() != null;
                    if (!isAjaxEndpoint) // For AJAX error page wont be rendered
                    {
                        httpContext.Response.Clear();
                        httpContext.Response.StatusCode = (int)statusCode;
                        httpContext.Response.Redirect($"/Errors/Error?code={httpContext.Response.StatusCode}", false);
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
