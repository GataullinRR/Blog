using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Blog.Filters
{
    public class ReturnIfModelStateInvalidAsyncFilter : IAsyncPageFilter, IAsyncActionFilter
    {
        public ReturnIfModelStateInvalidAsyncFilter()
        {

        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                                                      PageHandlerExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                await next.Invoke();
            }
            else
            {
                returnError(context.HttpContext);
            }
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.ModelState.IsValid)
            {
                await next();
            }
            else
            {
                returnError(context.HttpContext);
            }
        }

        void returnError(HttpContext context)
        {
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
        }
    }
}
