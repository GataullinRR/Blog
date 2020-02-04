﻿using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    public class AttributesProviderAsyncPageFilter : IAsyncPageFilter
    {
        public AttributesProviderAsyncPageFilter()
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

            var attributes = context.HandlerMethod.MethodInfo.GetCustomAttributes(false);
            var feature = new PageHandlerAttributesProviderFeature(attributes);
            context.HttpContext.Features.Set(feature);
        }
    }
}
