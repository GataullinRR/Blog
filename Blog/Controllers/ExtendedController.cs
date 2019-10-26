using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using Utilities.Extensions;

namespace Blog.Controllers
{
    public abstract class ExtendedController : Controller, ILayoutModelProvider
    {
        readonly IServiceProvider _serviceProvider;
        readonly Lazy<BlogContext> _db;
        readonly Lazy<PermissionsService> _permissions;
        readonly Lazy<UserManager<User>> _userManager;
        readonly Lazy<RoleManager<User>> _roleManager;
        readonly Lazy<SignInManager<User>> _signInManager;
        readonly Lazy<EMailService> _eMail;
        readonly Lazy<ConfirmationTokenService> _confirmationToken;

        protected BlogContext DB => _db.Value;
        protected UserManager<User> UserManager => _userManager.Value;
        protected RoleManager<User> RoleManager => _roleManager.Value;
        protected SignInManager<User> SignInManager => _signInManager.Value;
        protected EMailService EMail => _eMail.Value;
        protected ConfirmationTokenService ConfirmationTokens => _confirmationToken.Value;
        public PermissionsService Permissions => _permissions.Value;

        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        public bool PersistLayoutModel { get; set; } = false;

        public ExtendedController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _db = new Lazy<BlogContext>(() => (BlogContext)_serviceProvider.GetService(typeof(BlogContext)));
            _permissions = new Lazy<PermissionsService>(() => (PermissionsService)_serviceProvider.GetService(typeof(PermissionsService)));
            _userManager = new Lazy<UserManager<User>>(() => (UserManager<User>)_serviceProvider.GetService(typeof(UserManager<User>)));
            _roleManager = new Lazy<RoleManager<User>>(() => (RoleManager<User>)_serviceProvider.GetService(typeof(RoleManager<User>)));
            _signInManager = new Lazy<SignInManager<User>>(() => (SignInManager<User>)_serviceProvider.GetService(typeof(SignInManager<User>)));
            _eMail = new Lazy<EMailService>(() => (EMailService)_serviceProvider.GetService(typeof(EMailService)));
            _confirmationToken = new Lazy<ConfirmationTokenService>(() => (ConfirmationTokenService)_serviceProvider.GetService(typeof(ConfirmationTokenService)));
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
    }
}
