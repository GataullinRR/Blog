using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

    public class ExtendedPageModel : PageModel, ILayoutModelProvider
    {
        readonly IServiceProvider _serviceProvider;
        readonly Lazy<BlogContext> _db;
        readonly Lazy<PermissionsService> _permissions;
        readonly Lazy<UserManager<User>> _userManager;
        readonly Lazy<SignInManager<User>> _signInManager;
        readonly Lazy<EMailService> _eMail;

        protected BlogContext DB => _db.Value;
        protected UserManager<User> UserManager => _userManager.Value;
        protected SignInManager<User> SignInManager => _signInManager.Value;
        protected EMailService EMail => _eMail.Value;
        public PermissionsService Permissions => _permissions.Value;

        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        public bool PersistLayoutModel { get; set; } = false;

        public ExtendedPageModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _db = new Lazy<BlogContext>(() => (BlogContext)_serviceProvider.GetService(typeof(BlogContext)));
            _permissions = new Lazy<PermissionsService>(() => (PermissionsService)_serviceProvider.GetService(typeof(PermissionsService)));
            _userManager = new Lazy<UserManager<User>>(() => (UserManager<User>)_serviceProvider.GetService(typeof(UserManager<User>)));
            _signInManager = new Lazy<SignInManager<User>>(() => (SignInManager<User>)_serviceProvider.GetService(typeof(SignInManager<User>)));
            _eMail = new Lazy<EMailService>(() => (EMailService)_serviceProvider.GetService(typeof(EMailService)));
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

            base.OnPageHandlerExecuted(context);
        }
    }

    public class ExtendedController : Controller, ILayoutModelProvider
    {
        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        public bool PersistLayoutModel { get; set; } = false;

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
    }
}
