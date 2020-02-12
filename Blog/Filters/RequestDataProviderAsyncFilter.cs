using Blog.HttpContextFeatures;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Blog.Filters
{
    public class RequestDataProviderAsyncFilter : IAsyncPageFilter, IAsyncActionFilter
    {
        public RequestDataProviderAsyncFilter()
        {

        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                                                      PageHandlerExecutionDelegate next)
        {
            await next.Invoke();

            var feature = new RouteDataProviderFeature(context.RouteData.Values);
            context.HttpContext.Features.Set(feature);
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next.Invoke();

            var feature = new RouteDataProviderFeature(context.RouteData.Values);
            context.HttpContext.Features.Set(feature);
        }
    }
}
