using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Controllers
{
    public abstract class ControllerBase : Controller, ILayoutModelProvider
    {
        internal ServicesLocator S { get; }
        public LayoutModel LayoutModel { get; private set; }

        public ControllerBase(ServicesLocator serviceProvider)
        {
            S = serviceProvider;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            LayoutModel.UpdateMessages();
            LayoutModel.Save(HttpContext.Session);

            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            LayoutModel = LayoutModel.LoadOrNew(HttpContext.Session);

            base.OnActionExecuting(context);
        }

        public Task<string> RenderPartialToStringAsync<T>(string partialName, T model)
        {
            return this.RenderPartialToStringAsync(partialName, model);
        }
    }
}