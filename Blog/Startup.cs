using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCore.IServiceCollection.AddIUrlHelper;
using Blog.Middlewares;
using Blog.Misc;
using Blog.Pages.Account;
using Blog.Services;
using DBModels;
using Ganss.XSS;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Utilities.Types;

namespace Blog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<BlogContext>(options => options
                    .UseLazyLoadingProxies()
                    .UseSqlServer(connection, 
                        sqlOpt => sqlOpt.CommandTimeout(60)));
            //services.AddResponseCaching();
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                    .AddCookie(options => {
                        options.LoginPath = "/Account/Login/";
                    });
            services.AddIdentity<User, IdentityRole>(options =>
                    {
                        options.User.RequireUniqueEmail = false;
                    })
                    .AddEntityFrameworkStores<BlogContext>()
                    .AddTokenProvider<BasicTokenProvider>(nameof(BasicTokenProvider));
            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 3;

                // Lockout settings.
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
                options.User.RequireUniqueEmail = false;
            });
            services.AddMvc(options =>
            {
                //options.CacheProfiles.Add(ResponseCaching.DAILY, new CacheProfile()
                //{
                //    Duration = 60 * 60 * 24
                //});
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSession();
            services.AddMemoryCache();
            services.AddUrlHelper();
            services.AddScoped<EMailService>();
            services.AddScoped<ConfirmationLinksGeneratorService>();
            services.AddScoped<PermissionsService>();
            services.AddScoped<HistoryService>();
            services.AddSingleton<AutounbanService>();
            services.AddScoped<DecisionsService>();
            services.AddScoped<DbEntitiesUpdateService>();
            services.AddScoped<LinkBuilderService>();
            services.AddScoped<ServicesLocator>();
            services.AddScoped<ActivationLinkGeneratorService>();
            services.AddScoped<SessionMutatorsManagerService>();
            services.AddScoped<BanningService>();
            services.AddScoped<UtilitiesService>();
            services.AddScoped<EntitiesProviderService>();
            services.AddScoped<ModerationService>();
            registerServices();

            void registerServices()
            {
                var assembly = Assembly.GetExecutingAssembly();
                foreach (var type in assembly.DefinedTypes)
                {
                    var serviceType = type.GetCustomAttribute<ServiceAttribute>()?.ServiceType;
                    if (serviceType.HasValue)
                    {
                        switch (serviceType.Value)
                        {
                            case ServiceType.SCOPED:
                                services.AddScoped(type);
                                break;
                            case ServiceType.SINGLETON:
                                services.AddSingleton(type);
                                break;

                            default:
                                throw new NotSupportedException();
                        }
                    }
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Required to run
            app.ApplicationServices.GetService<AutounbanService>();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
            }
            else
            {

            }

            //app.UseResponseCaching();
            app.UseAuthentication();
            //app.UseMiddleware<CustomResponseCachingMiddleware>(); // disable for debugging
            app.UseMiddleware<ScopedServiceInstantiatorMiddleware<StatisticService>>(); // to execute before all other services
            app.UseMiddleware<ErrorsHandlerMiddleware>();
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = (context) =>
                {
                    var headers = context.Context.Response.GetTypedHeaders();

                    headers.CacheControl = new CacheControlHeaderValue
                    {
                        Public = true,
                        MaxAge = TimeSpan.FromDays(365)
                    };
                }
            });
            app.UseSession();
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
