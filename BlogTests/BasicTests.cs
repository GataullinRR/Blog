using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using Xunit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using DBModels;
using System.Net.Http;
using System.Linq;
using AngleSharp.Html.Dom;
using AngleSharp;
using System.Threading;
using AngleSharp.Io;
using System.Net.Http.Headers;
using System.Net;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;
using Blog;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Blog.Services;
using BlogTests;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BlogTests
{
    //public class testin
    //{
    //     //CustomWebApplicationFactory<Blog.Startup _factory;
    //    public IDisposable GetServices(out ServicesProvider services)
    //    {
    //        var scope = _factory.Server.Host.Services.CreateScope();
    //        services = scope.ServiceProvider.GetRequiredService<ServicesProvider>();

    //        return scope;
    //    }
    //}

    [TestCaseOrderer(CustomTestCaseOrderer.TypeName, CustomTestCaseOrderer.AssembyName)]
    public class BasicTests : IClassFixture<CustomWebApplicationFactory<Blog.Startup>>
    {
        private readonly CustomWebApplicationFactory<Blog.Startup> _factory;

        readonly HttpClient _client;

        public BasicTests(CustomWebApplicationFactory<Blog.Startup> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();

            using (var scope = _factory.Server.Host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

                DbSampleData.Initialize(db,
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                    scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                    scope.ServiceProvider.GetRequiredService<SignInManager<User>>());
            }
        }

        [Fact, Order(0)]
        public async Task get_Endpoint_UnathorizedUser()
        {
            using (getServices(out var services))
            {
                // Arrange
                var authorizedUris = new string[]
                {
                    "/",
                    "/Index",
                    "/Index?pageIndex=1",
                    $"/Post/1",
                    $"/Account/Profile?id={services.Db.Users.FirstItem().Id}",
                    "/Register",
                    "/Account/Login",
                    "/Account/PasswordRestore"
                };

                await Task.WhenAll(authorizedUris.Select(uri => get_Endpoint(uri)));
            }
        }

        [Fact, Order(100)]
        public async Task Register()
        {
            // Arrange
            var userName = "Test";
            var uri = "/Account/Register";
            var formData = new Dictionary<string, string>
            {
                { "Username", userName },
                { "EMail", "test@doesnotexists.ru" },
                { "Password", "test@doesnotexists.ru" },
                { "ConfirmPassword", "test@doesnotexists.ru" },
                { "__RequestVerificationToken", await ensureAntiforgeryTokenAsync(_client, uri) }
            };

            // Act
            var response = await _client.PostAsync(uri, new FormUrlEncodedContent(formData));

            // Assert
            ensureOkResponse(response);
            using (getServices(out var services))
            {
                var newUser = await services.Db.Users.FirstOrDefaultAsync(u => u.UserName == userName);
                Assert.NotNull(newUser);
                Assert.Equal(userName, newUser.UserName);
                Assert.Equal(ProfileState.RESTRICTED, newUser.Status.State);
                Assert.True(await services.UserManager.IsInOneOfTheRolesAsync(newUser, Roles.USER));
                //Assert.True(services.HttpContext.User.Identity.IsAuthenticated);
                //Assert.Equal(userName, services.HttpContext.User.Identity.Name);
            }
        }

        [Fact, Order(200)]
        public async Task get_Endpoint_RestrictedAuthorizedUser()
        {
            using (getServices(out var services))
            {
                var testUser = await services.Db.Users.FirstAsync(u => u.UserName == "Test");

                // Arrange
                var authorizedUris = new string[]
                {
                    "/",
                    "/Index",
                    "/Index?pageIndex=1",
                    $"/Post/1",
                    $"/Account/Profile?id={(await services.Db.Users.FirstAsync(u => u.Id != testUser.Id)).Id}",

                    "/Account/Profile",
                    $"/Account/Profile?id={testUser.Id}",
                    "/Account/Logout",
                    $"/Account/ProfileEdit?id={testUser.Id}",
                    "/Reporting/ReportProfileAsync/3",
                    "/Reporting/ReportPostAsync/0",
                    "/Reporting/ReportCommentaryAsync/0",
                    $"/Account/PasswordChange?id={testUser.Id}"
                };

                await Task.WhenAll(authorizedUris.Select(uri => get_Endpoint(uri)));
            }
        }

        void ensureOkResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        async Task get_Endpoint(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        IDisposable getServices(out ServicesProvider services)
        {
            var scope = _factory.Server.Host.Services.CreateScope();
            services = scope.ServiceProvider.GetRequiredService<ServicesProvider>();

            return scope;
        }

        //[Theory]
        //[InlineData("/Profile?=0")]
        //[InlineData("/Post?=0")]
        //public async Task Get_PublicResources(string url)
        //{
        //    // Arrange
        //    var client = _factory.CreateClient();

        //    // Act
        //    var response = await client.GetAsync(url);

        //    // Assert
        //    response.EnsureSuccessStatusCode(); // Status Code 200-299
        //    Assert.Equal("text/html; charset=utf-8",
        //        response.Content.Headers.ContentType.ToString());
        //}

        SetCookieHeaderValue _antiforgeryCookie;
        string _antiforgeryToken;
        static readonly Regex AntiforgeryFormFieldRegex = new Regex(@"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
        async Task<string> ensureAntiforgeryTokenAsync(HttpClient client, string uri)
        {
            if (_antiforgeryToken != null)
            {
                return _antiforgeryToken;
            }

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values))
            {
                _antiforgeryCookie = SetCookieHeaderValue.ParseList(values.ToList()).SingleOrDefault(c => c.Name.StartsWith(".AspNetCore.AntiForgery.", StringComparison.InvariantCultureIgnoreCase));
            }
            Assert.NotNull(_antiforgeryCookie);
            client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_antiforgeryCookie.Name, _antiforgeryCookie.Value).ToString());

            var responseHtml = await response.Content.ReadAsStringAsync();
            var match = AntiforgeryFormFieldRegex.Match(responseHtml);
            _antiforgeryToken = match.Success ? match.Groups[1].Captures[0].Value : null;
            Assert.NotNull(_antiforgeryToken);

            return _antiforgeryToken;
        }
    }
}
