using Blog.Attributes;
using Blog.Services.Models;
using DBModels;
using Ganss.XSS;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPCoreUtilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class ServiceLocator
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
        readonly Lazy<UtilitiesService> _utilities;
        readonly Lazy<BanningService> _banning;
        readonly Lazy<PostSanitizerService> _sanitizer;
        readonly Lazy<ModerationService> _moderation;
        readonly Lazy<StorageService> _storage;
        readonly Lazy<RepositoryService> _repository;
        readonly Lazy<URIProviderService> _uriProvider;
        readonly Lazy<IMemoryCache> _memoryCache;
        readonly Lazy<ContactEmailProviderService> _contactEmailProvider;
        readonly Lazy<CacheManagerService> _cacheManager;
        readonly Lazy<StatisticService> _statisticService;

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
        public UtilitiesService Utilities => _utilities.Value;
        public BanningService Banning => _banning.Value;
        public PostSanitizerService Sanitizer => _sanitizer.Value;
        public ModerationService Moderation => _moderation.Value;
        public StorageService Storage => _storage.Value;
        public RepositoryService Repository => _repository.Value;
        public URIProviderService URIProvider => _uriProvider.Value;
        public IMemoryCache MemoryCache => _memoryCache.Value;
        public ContactEmailProviderService ContactEmailProvider => _contactEmailProvider.Value;
        public CacheManagerService CacheManager => _cacheManager.Value;
        public StatisticService StatisticService => _statisticService.Value;

        public ServiceLocator(IServiceProvider serviceProvider)
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
            _utilities = ServiceProvider.GetLazyService<UtilitiesService>();
            _banning = ServiceProvider.GetLazyService<BanningService>();
            _sanitizer = ServiceProvider.GetLazyService<PostSanitizerService>();
            _moderation = ServiceProvider.GetLazyService<ModerationService>();
            _storage = ServiceProvider.GetLazyService<StorageService>();
            _repository = ServiceProvider.GetLazyService<RepositoryService>();
            _uriProvider = ServiceProvider.GetLazyService<URIProviderService>();
            _memoryCache = ServiceProvider.GetLazyService<IMemoryCache>();
            _contactEmailProvider = ServiceProvider.GetLazyService<ContactEmailProviderService>();
            _cacheManager = ServiceProvider.GetLazyService<CacheManagerService>();
            _statisticService = ServiceProvider.GetLazyService<StatisticService>();
        }
    }
}
