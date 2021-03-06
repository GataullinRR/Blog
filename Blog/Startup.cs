﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using AspNetCore.IServiceCollection.AddIUrlHelper;
using ASPCoreUtilities.Extensions;
using Blog.Attributes;
using Blog.Filters;
using Blog.Middlewares;
using Blog.Misc;
using Blog.Misc.ModelBinder;
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
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Utilities.Types;
using StatisticServiceExports;
using StatisticServiceClient;
using Utilities.Extensions;
using StatisticServiceExports.Kafka;

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
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_";
                options.User.RequireUniqueEmail = false;
            });
            services.AddMvc(options =>
            {
                options.Filters.Add(new ReturnIfModelStateInvalidAsyncFilter());
                options.Filters.Add(new AttributesProviderAsyncPageFilter());
                options.Filters.Add(new RequestDataProviderAsyncFilter());
                options.Filters.Add(new ClientResponseCacheAsyncFilter());

                options.ModelBinderProviders.Insert(0, new IdToEntityModelBinderProvider());
            });

            services.AddSession();
            services.AddMemoryCache();
            services.AddUrlHelper();
            services.AddSingleton<IStatisticServiceAPI, StatisticServiceAPI>();
            Assembly.GetExecutingAssembly().FindAndRegisterServicesTo(services);
            overrideObjectModelValidator();

            void overrideObjectModelValidator()
            {
                IObjectModelValidator defaultValidator = null;
                using (var sp = services.BuildServiceProvider())
                {
                    defaultValidator = sp.GetRequiredService<IObjectModelValidator>();
                }
                var conditionalValidator = new ConditionalObjectModelValidatorProxy(defaultValidator);
                services.AddSingleton<IObjectModelValidator>(conditionalValidator);
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Required to run
            app.ApplicationServices.GetService<AutounbanService>();
            app.ApplicationServices.GetRequiredService<IObjectModelValidator>();

            //app.UseResponseCaching();
            app.UseAuthentication();
            //app.UseEndpointRouting();
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
            app.UseRouting();
            app.UseEndpoints(b => b.MapRazorPages());
            //app.UseMvc();
            //app.UseMvc(routes =>
            //{
            //    routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            //});
        }
    }
}
