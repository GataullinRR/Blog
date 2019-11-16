using DBModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class ServicesProvider
    {
        readonly Lazy<BlogContext> _db;
        readonly Lazy<PermissionsService> _permissions;
        readonly Lazy<UserManager<User>> _userManager;
        readonly Lazy<RoleManager<User>> _roleManager;
        readonly Lazy<SignInManager<User>> _signInManager;
        readonly Lazy<EMailService> _eMail;
        readonly Lazy<ConfirmationLinksGeneratorService> _confirmationToken;
        readonly Lazy<ActivationLinkGeneratorService> _activationLinks;
        readonly Lazy<HistoryService> _history;
        readonly Lazy<DecisionsService> _decisions;
        readonly Lazy<DbEntitiesUpdateService> _dbUpdator;
        readonly Lazy<IHttpContextAccessor> _httpContext;
        readonly Lazy<IUrlHelper> _urlHelper;
        readonly Lazy<LinkBuilderService> _linkBuilder;
        readonly Lazy<IDataProtectionProvider> _protectionProvider;
        readonly Lazy<SessionMutatorsManagerService> _mutatorsManager;

        public IServiceProvider ServiceProvider { get; }
        public BlogContext Db => _db.Value;
        public UserManager<User> UserManager => _userManager.Value;
        public RoleManager<User> RoleManager => _roleManager.Value;
        public SignInManager<User> SignInManager => _signInManager.Value;
        public EMailService EMail => _eMail.Value;
        public ConfirmationLinksGeneratorService ConfirmationLinks => _confirmationToken.Value;
        public ActivationLinkGeneratorService ActivationLinks => _activationLinks.Value;
        public HistoryService History => _history.Value;
        public DecisionsService Decisions => _decisions.Value;
        public PermissionsService Permissions => _permissions.Value;
        public DbEntitiesUpdateService DbUpdator => _dbUpdator.Value;
        public HttpContext HttpContext => _httpContext.Value.HttpContext;
        public IUrlHelper UrlHelper => _urlHelper.Value;
        public LinkBuilderService LinkBuilder => _linkBuilder.Value;
        public IDataProtectionProvider ProtectionProvider => _protectionProvider.Value;
        public SessionMutatorsManagerService MutatorsManager => _mutatorsManager.Value;

        public ServicesProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;

            _db = ServiceProvider.GetLazyService<BlogContext>();
            _permissions = ServiceProvider.GetLazyService<PermissionsService>();
            _userManager = ServiceProvider.GetLazyService<UserManager<User>>();
            _roleManager = ServiceProvider.GetLazyService<RoleManager<User>>();
            _signInManager = ServiceProvider.GetLazyService<SignInManager<User>>();
            _eMail = ServiceProvider.GetLazyService<EMailService>();
            _confirmationToken = ServiceProvider.GetLazyService<ConfirmationLinksGeneratorService>();
            _activationLinks = ServiceProvider.GetLazyService<ActivationLinkGeneratorService>();
            _history = ServiceProvider.GetLazyService<HistoryService>();
            _decisions = ServiceProvider.GetLazyService<DecisionsService>();
            _dbUpdator = ServiceProvider.GetLazyService<DbEntitiesUpdateService>();
            _httpContext = ServiceProvider.GetLazyService<IHttpContextAccessor>();
            _urlHelper = ServiceProvider.GetLazyService<IUrlHelper>();
            _linkBuilder = ServiceProvider.GetLazyService<LinkBuilderService>();
            _protectionProvider = ServiceProvider.GetLazyService<IDataProtectionProvider>();
            _mutatorsManager = ServiceProvider.GetLazyService<SessionMutatorsManagerService>();
        }
    }
}
