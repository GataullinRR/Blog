﻿using Blog.Models;
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
        readonly Lazy<HistoryService> _history;

        protected BlogContext DB => _db.Value;
        protected UserManager<User> UserManager => _userManager.Value;
        protected RoleManager<User> RoleManager => _roleManager.Value;
        protected SignInManager<User> SignInManager => _signInManager.Value;
        protected EMailService EMail => _eMail.Value;
        protected ConfirmationTokenService ConfirmationTokens => _confirmationToken.Value;
        protected HistoryService History => _history.Value;
        public PermissionsService Permissions => _permissions.Value;

        public LayoutModel LayoutModel { get; private set; } = new LayoutModel();
        public bool PersistLayoutModel { get; set; } = false;

        public ExtendedController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _db = _serviceProvider.GetLazyService<BlogContext>();
            _permissions = _serviceProvider.GetLazyService<PermissionsService>();
            _userManager = _serviceProvider.GetLazyService<UserManager<User>>();
            _roleManager = _serviceProvider.GetLazyService<RoleManager<User>>();
            _signInManager = _serviceProvider.GetLazyService<SignInManager<User>>();
            _eMail = _serviceProvider.GetLazyService<EMailService>();
            _confirmationToken = _serviceProvider.GetLazyService<ConfirmationTokenService>();
            _history = _serviceProvider.GetLazyService<HistoryService>();
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

            History.SaveCurrentURL();

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