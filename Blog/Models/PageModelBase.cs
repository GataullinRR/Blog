using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        internal bool PersistLayoutModel { get; set; } = false;

        public PageModelBase(ServicesProvider services)
        {
            Services = services;
        }

        public override void OnPageHandlerSelected(PageHandlerSelectedContext context)
        {
            LayoutModel = HttpContext.Session.Keys.Contains(nameof(LayoutModel)) 
                ? HttpContext.Session.GetString(nameof(LayoutModel)).FromBase64().Deserialize<LayoutModel>() 
                : LayoutModel;

            base.OnPageHandlerSelected(context);
        }

        public override void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        {
            if (PersistLayoutModel)
            {
                HttpContext.Session.SetString(nameof(LayoutModel), LayoutModel.Serialize().ToBase64());
            }
            else
            {
                HttpContext.Session.Remove(nameof(LayoutModel));
            }

            Services.History.SaveCurrentURL();

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
    }
}
