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

    public abstract class ExtendedPageModel : PageModel, ILayoutModelProvider
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
        protected HistoryService History { get; }
        public PermissionsService Permissions => _permissions.Value;

        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        public bool PersistLayoutModel { get; set; } = false;

        public ExtendedPageModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _db = _serviceProvider.GetLazyService<BlogContext>();
            _permissions = _serviceProvider.GetLazyService<PermissionsService>();
            _userManager = _serviceProvider.GetLazyService<UserManager<User>>();
            _signInManager = _serviceProvider.GetLazyService<SignInManager<User>>();
            _eMail = _serviceProvider.GetLazyService<EMailService>();
            History = _serviceProvider.GetService<HistoryService>();
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

            History.SaveCurrentURL();

            base.OnPageHandlerExecuted(context);
        }
    }
}
