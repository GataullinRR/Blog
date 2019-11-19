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
        public ServicesProvider S { get; set; }

        public PermissionsService Permissions => S.Permissions;
        public LayoutModel LayoutModel { get; private set; }
        protected bool autoSaveDbChanges = false;

        public PageModelBase(ServicesProvider services)
        {
            S = services;
        }

        public override void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            LayoutModel = LayoutModel.LoadOrNew(HttpContext.Session);

            base.OnPageHandlerSelected(context);
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            LayoutModel.UpdateMessages();
            LayoutModel.Save(HttpContext.Session);
            S.History.SaveCurrentURL();
            if (autoSaveDbChanges)
            {
                S.Db.SaveChanges();
            }

            base.OnPageHandlerExecuted(context);
        }
    }
}
