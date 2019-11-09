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


namespace BlogTests
{
    public class BasicAccessTests : SingleContextTestsBase
    {
        const string TEST_USER_NAME = "Test";
        const string TEST_USER_PASSWORD = "test@doesnotexists.ru";

        static readonly CustomWebApplicationFactory<Startup> _factory;
        static HttpClient _client;

        static BasicAccessTests()
        {
            _factory = instantiateApplication();
            _client = _factory.CreateClient();
        }

        protected override CustomWebApplicationFactory<Startup> factory => _factory;
        protected override HttpClient currentClient => _client;

        public BasicAccessTests(ITestOutputHelper output) : base(output)
        {

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
                    await assertEndpointAccessible(uri);
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
                { "Password", TEST_USER_PASSWORD },
                { "ConfirmPassword", TEST_USER_PASSWORD },
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
                    await assertEndpointAccessible(uri);
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
                    await assertEndpointAccessible(uri);
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
                await assertEndpointAccessible(testEndpoint);
                await assertEndpointAccessible("/Account/Logout");

                // Assert
                await assertEndpointReturnsErrorPage(testEndpoint, HttpStatusCode.InternalServerError);
            }
        }

        [Fact, Order(500)]
        public async Task LogIn()
        {
            // Arrange
            _client = _factory.CreateClient();
            var uri = "/Account/Login";
            var formData = new Dictionary<string, string>
            {
                { "Login", TEST_USER_NAME },
                { "Password", TEST_USER_PASSWORD },
                { "__RequestVerificationToken", await ensureAntiforgeryTokenAsync(_client, uri) }
            };

            // Act
            var response = await _client.PostAsync(uri, new FormUrlEncodedContent(formData));

            // Assert
            ensureOkResponse(response);
            await CheckEndpoints_NotRestrictedAuthorizedUser();
        }
    }
}
