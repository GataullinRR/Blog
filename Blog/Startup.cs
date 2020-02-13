using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCore.IServiceCollection.AddIUrlHelper;
using Blog.Attributes;
using Blog.Filters;
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
using Microsoft.AspNetCore.Internal;
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
                    //.UseLazyLoadingProxies()
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
                options.Filters.Add(new AttributesProviderAsyncPageFilter());
                options.Filters.Add(new RequestDataProviderAsyncFilter());
                options.Filters.Add(new ClientResponseCacheAsyncFilter());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddSession();
            services.AddMemoryCache();
            services.AddUrlHelper();
            registerServices();

            void registerServices()
            {
                var allTypes = Assembly.GetExecutingAssembly().DefinedTypes
                    .Select(t => t.AsType())
                    .ToArray();
                foreach (var implementationType in allTypes)
                {
                    var serviceInfo = implementationType.GetCustomAttribute<ServiceAttribute>();
                    if (serviceInfo != null)
                    {
                        var serviceType = serviceInfo.RegisterAs ?? implementationType;
                        switch (serviceInfo.ServiceType)
                        {
                            case ServiceType.SCOPED:
                                services.AddScoped(serviceType, implementationType);
                                break;
                            case ServiceType.SINGLETON:
                                services.AddSingleton(serviceType, implementationType);
                                break;
                            case ServiceType.TRANSIENT:
                                services.AddTransient(serviceType, implementationType);
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
            app.UseEndpointRouting();
            app.UseMiddleware<ServerResponseCachingMiddleware>();
            app.UseMiddleware<ErrorsHandlerMiddleware>();
            app.UseMiddleware<ScopedServiceInstantiatorMiddleware<StatisticService>>(); // to execute before all other services
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
