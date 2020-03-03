using ASPCoreUtilities;
using ASPCoreUtilities.Extensions;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.IO;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Controllers
{
    public abstract class ControllerBase : Controller, ILayoutModelProvider
    {
        internal ServiceLocator S { get; }
        public LayoutModel LayoutModel { get; private set; }

        public ControllerBase(ServiceLocator serviceProvider)
        {
            S = serviceProvider;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            LayoutModel = await LayoutModel.LoadOrNewAsync(S);

            try
            {
                await base.OnActionExecutionAsync(context, next);
            }
            finally
            {
                LayoutModel.UpdateMessages();
                LayoutModel.Save(HttpContext.Session);
            }
        }

        protected static string getURIToAction(string controllerNameOf, string actionName)
        {
            return $"/{controllerNameOf.GetController()}/{actionName}";
        }
    }
}