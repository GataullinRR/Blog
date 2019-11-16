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
        public ServicesProvider Services { get; set; }

        public PermissionsService Permissions => Services.Permissions;
        public LayoutModel LayoutModel { get; private set; }
        protected bool autoSaveDbChanges = false;

        public PageModelBase(ServicesProvider services)
        {
            Services = services;
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
            Services.History.SaveCurrentURL();
            if (autoSaveDbChanges)
            {
                Services.Db.SaveChanges();
            }

            base.OnPageHandlerExecuted(context);
        }

        public async Task<User> GetCurrentUserModelOrThrowAsync()
        {
            var user = await Services.UserManager.GetUserAsync(User);
            if (user == null)
            {
                throw new UnauthorizedAccessException();
            }
            else
            {
                return user;
            }
        }
        public async Task<User> FindUserByIdOrGetCurrentOrThrowAsync(string userId)
        {
            var user = await Services.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await GetCurrentUserModelOrThrowAsync();
            }

            return user;
        }
    }
}
