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
using System.Diagnostics;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BlogTests
{
    [TestCaseOrderer(CustomTestCaseOrderer.TypeName, CustomTestCaseOrderer.AssembyName)]
    public class BasicTests /*: IClassFixture<CustomWebApplicationFactory<Blog.Startup>>*/
    {
        const string TEST_USER_NAME = "Test";
        static readonly CustomWebApplicationFactory<Blog.Startup> _factory = new CustomWebApplicationFactory<Startup>();
        static HttpClient _client;

        static BasicTests()
        {
            _client = _factory.CreateClient();

            using (var scope = _factory.Server.Host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

                DbSampleData.Initialize(db,
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                    scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                    scope.ServiceProvider.GetRequiredService<SignInManager<User>>());
            }

            Debug.WriteLine("Static");
        }

        readonly ITestOutputHelper _output;

        public BasicTests(ITestOutputHelper output)
        {
            this._output = output;
        }

        [Fact, Order(0)]
        public async Task CheckEndpoints_UnathorizedUser()
        {
            using (getServices(out var services))
            {
                // Arrange
                var authorizedUris = new string[]
                {
                    "/",
                    "/Index",
                    "/Index?pageIndex=1",
                    "/Post/1",
                    $"/Account/Profile?id={services.Db.Users.FirstItem().Id}",
                    "/Register",
                    "/Account/Login",
                    "/Account/PasswordRestore"
                };

                foreach (var uri in authorizedUris)
                {
                    await assertEndpointAccesible(uri);
                }
            }

            _client = _factory.CreateClient();
        }

        [Fact, Order(100)]
        public async Task Register()
        {
            // Arrange
            var userName = TEST_USER_NAME;
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
            }
        }

        [Fact, Order(200)]
        public async Task CheckEndpoints_RestrictedAuthorizedUser()
        {
            using (getServices(out var services))
            {
                var testUser = await services.Db.Users.FirstAsync(u => u.UserName == TEST_USER_NAME);

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
                    //"/Account/Logout",
                    $"/Account/ProfileEdit?id={testUser.Id}",
                    $"/Reporting/ReportProfileAsync?id={services.Db.Users.FirstItem().Profile.Id}",
                    $"/Reporting/ReportPostAsync?id={services.Db.Posts.FirstItem().Id}",
                    $"/Reporting/ReportCommentaryAsync?id={services.Db.Commentaries.FirstItem().Id}",
                };
                var unauthorizedUris = new Dictionary<string, HttpStatusCode>
                {
                    { "/ModeratorPanel", HttpStatusCode.Unauthorized },
                    { "/PostCreate", HttpStatusCode.Unauthorized },
                    {  $"/PostEdit?id={services.Db.Posts.FirstItem().Id}", HttpStatusCode.Unauthorized },
                    {  $"/Account/PasswordChange?id={testUser.Id}", HttpStatusCode.Unauthorized }
                };

                foreach (var uri in authorizedUris)
                {
                    await assertEndpointAccesible(uri);
                }
                foreach (var uri in unauthorizedUris)
                {
                    await assertEndpointReturnsErrorPage(uri.Key, uri.Value);
                }
            }
        }

        [Fact, Order(300)]
        public async Task CheckEndpoints_NotRestrictedAuthorizedUser()
        {
            using (var disposer = getServices(out var services))
            {
                // Arrange
                var testUser = await services.Db.Users.FirstAsync(u => u.UserName == TEST_USER_NAME);
                testUser.Status.State = ProfileState.ACTIVE;
                await services.Db.SaveChangesAsync();

                var authorizedUris = new string[]
                {
                    "/",
                    "/Index",
                    "/Index?pageIndex=1",
                    $"/Post/1",
                    $"/Account/Profile?id={(await services.Db.Users.FirstAsync(u => u.Id != testUser.Id)).Id}",

                    "/Account/Profile",
                    $"/Account/Profile?id={testUser.Id}",
                    //"/Account/Logout",
                    $"/Account/ProfileEdit?id={testUser.Id}",
                    $"/Reporting/ReportProfileAsync?id={services.Db.Users.FirstItem().Profile.Id}",
                    $"/Reporting/ReportPostAsync?id={services.Db.Posts.FirstItem().Id}",
                    $"/Reporting/ReportCommentaryAsync?id={services.Db.Commentaries.FirstItem().Id}",

                    "/PostCreate",
                    $"/PostEdit?id={services.Db.Posts.FirstItem().Id}",
                    $"/Account/PasswordChange?id={testUser.Id}",
                };
                var unauthorizedUris = new Dictionary<string, HttpStatusCode>
                {
                    { "/ModeratorPanel", HttpStatusCode.Unauthorized },
                };
                disposer.Dispose();

                foreach (var uri in authorizedUris)
                {
                    await assertEndpointAccesible(uri);
                }
                foreach (var uri in unauthorizedUris)
                {
                    await assertEndpointReturnsErrorPage(uri.Key, uri.Value);
                }
            }
        }

        [Fact, Order(400)]
        public async Task LogOut()
        {
            // Arrange
            var testEndpoint = "/Account/Profile";
            using (getServices(out var services))
            {
                // Act
                await assertEndpointAccesible(testEndpoint);
                await assertEndpointAccesible("/Account/Logout");

                // Assert
                await assertEndpointReturnsErrorPage(testEndpoint, HttpStatusCode.InternalServerError);
            }
        }

        async Task assertEndpointAccesible(string url)
        {
            _output.WriteLine(url);

            // Act
            var response = await _client.GetAsync(url);

            // Assert
            ensureOkResponse(response);
        }
        async Task assertEndpointReturnsErrorPage(string url, HttpStatusCode statusCode)
        {
            _output.WriteLine(url);

            // Act
            var response = await _client.GetAsync(url);

            ensureOkResponse(response);
            Assert.NotNull(response?.RequestMessage?.RequestUri?.AbsoluteUri);
            Assert.EndsWith($"code={(int)statusCode}", response.RequestMessage.RequestUri.AbsoluteUri);
        }
        void ensureOkResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        IDisposable getServices(out ServicesProvider services)
        {
            var scope = _factory.Server.Host.Services.CreateScope();
            services = scope.ServiceProvider.GetRequiredService<ServicesProvider>();

            return scope;
        }

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
            client.DefaultRequestHeaders.Add("Cookie", new Microsoft.Net.Http.Headers.CookieHeaderValue(_antiforgeryCookie.Name, _antiforgeryCookie.Value).ToString());

            var responseHtml = await response.Content.ReadAsStringAsync();
            var match = AntiforgeryFormFieldRegex.Match(responseHtml);
            _antiforgeryToken = match.Success 
                ? match.Groups[1].Captures[0].Value 
                : null;
            Assert.NotNull(_antiforgeryToken);

            return _antiforgeryToken;
        }
    }
}
