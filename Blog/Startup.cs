using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
                    .UseSqlServer(connection));
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

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
            services.AddScoped<ServicesProvider>();
            services.AddScoped<ActivationLinkGeneratorService>();
            services.AddScoped<SessionMutatorsManagerService>();
            services.AddScoped<BanningService>();
            services.AddScoped<UtilitiesService>();
            services.AddSingleton<HtmlSanitizerService>();
            services.AddScoped<EntitiesProviderService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.ApplicationServices.GetService<AutounbanService>();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseStatusCodePagesWithRedirects("/Errors/Error?code={0}");
                //app.UseExceptionHandler("/Errors/Error");
                //app.UseDeveloperExceptionPage();
            }
            else
            {

            }

            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseSession();
            app.UseMiddleware<ExceptionToStatusCodeMiddleware>();
            //app.Use(async (ctx, next) =>
            //{
            //    await next();

            //    if (ctx.Response.StatusCode == 404 && !ctx.Response.HasStarted)
            //    {
            //        ctx.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            //        ////Re-execute the request so the user gets the error page
            //        //string originalPath = ctx.Request.Path.Value;
            //        //ctx.Items["originalPath"] = originalPath;
            //        //ctx.Request.Path = "/error/404";
            //        await next();
            //    }
            //});
            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
