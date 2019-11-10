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
        internal ServicesProvider Services { get; }
        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        public bool PersistLayoutModel { get; set; } = false;

        public ControllerBase(ServicesProvider serviceProvider)
        {
            Services = serviceProvider;
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (PersistLayoutModel)
            {
                HttpContext.Session.SetString(nameof(LayoutModel), LayoutModel.Serialize().ToBase64());
            }
            else
            {
                HttpContext.Session.Remove(nameof(LayoutModel));
            }

            base.OnActionExecuted(context);
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            LayoutModel = HttpContext.Session.Keys.Contains(nameof(LayoutModel))
                ? HttpContext.Session.GetString(nameof(LayoutModel)).FromBase64().Deserialize<LayoutModel>()
                : LayoutModel;

            base.OnActionExecuting(context);
        }

        public async Task<User> GetCurrentUserModelOrThrowAsync()
        {
            var user = await Services.UserManager.GetUserAsync(User);
            if (user == null)
            {
                throw new AuthenticationException();
            }
            else
            {
                return user;
            }
        }
    }
}