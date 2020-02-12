using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Models
{
    public interface ILayoutModelProvider
    {
        LayoutModel LayoutModel { get; }
    }

    public abstract class PageModelBase : PageModel, ILayoutModelProvider
    {
        public ServiceLocator S { get; set; }

        public PermissionsService Permissions => S.Permissions;
        public LayoutModel LayoutModel { get; private set; }
        protected bool autoSaveDbChanges = false;

        public PageModelBase(ServiceLocator services)
        {
            S = services;
        }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            LayoutModel = await LayoutModel.LoadOrNewAsync(S);

            try
            {
                await base.OnPageHandlerExecutionAsync(context, next);
            }
            finally
            {
                LayoutModel.UpdateMessages();
                LayoutModel.Save(HttpContext.Session);
                S.History.SaveCurrentURL();
                if (autoSaveDbChanges)
                {
                    await S.Db.SaveChangesAsync();
                }
            }
        }
    }
}
