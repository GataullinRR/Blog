using System;
using System.Collections.Generic;
using Utilities.Extensions;
using Xunit;
using System.Threading.Tasks;
using DBModels;
using System.Net.Http;
using System.Linq;
using System.Net;
using Microsoft.Net.Http.Headers;
using System.Text.RegularExpressions;
using Blog;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Blog.Services;
using Xunit.Abstractions;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace BlogTests
{
    [TestCaseOrderer(CustomTestCaseOrderer.TypeName, CustomTestCaseOrderer.AssembyName)]
    public abstract class SingleContextTestsBase
    {
        protected readonly ITestOutputHelper _output;
        protected CustomWebApplicationFactory _factory;
        protected virtual HttpClient currentClient { get; set; }

        public SingleContextTestsBase(CustomWebApplicationFactory factory, ITestOutputHelper output)
        {
            _output = output;
            _factory = factory;
        }

        protected void initializeApplication()
        {
            _factory.CreateClient(); // It'll initialize the server
            using (var scope = _factory.Server.Host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

                DbSampleData.Initialize(db,
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                    scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                    scope.ServiceProvider.GetRequiredService<SignInManager<User>>());
            }
        }

        protected async Task assertFormPostWithNoErrorOfAnyKindAsync(string url, Dictionary<string, string> formValues)
        {
            _output.WriteLine($"Form post: {url}");

            // Arrange
            var formData = new Dictionary<string, string>();
            formData.AddRange(formValues);
            formData.Add("__RequestVerificationToken", await ensureAntiforgeryTokenAsync(currentClient, url));

            // Act
            var response = await currentClient.PostAsync(url, new FormUrlEncodedContent(formData));

            // Assert
            assertResponseWithNoErrorOfAnyKind(response);
        }
        protected async Task assertGetWithNoErrorOfAnyKindAsync(string url)
        {
            _output.WriteLine($"Get: {url}");

            // Act
            var response = await currentClient.GetAsync(url);
            
            // Assert
            assertResponseWithNoErrorOfAnyKind(response);
        }
        protected async Task assertEndpointReturnsErrorPage(string url, HttpStatusCode statusCode)
        {
            _output.WriteLine(url);

            // Act
            var response = await currentClient.GetAsync(url);

            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.NotNull(response?.RequestMessage?.RequestUri?.AbsoluteUri);
            Assert.EndsWith($"code={(int)statusCode}", response.RequestMessage.RequestUri.AbsoluteUri);
        }
        protected void assertResponseWithNoErrorOfAnyKind(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.NotNull(response?.RequestMessage?.RequestUri?.AbsoluteUri);
            Assert.DoesNotContain("/Error?code=", response.RequestMessage.RequestUri.AbsoluteUri);
        }

        protected IDisposable getServices(out ServicesProvider services)
        {
            var scope = _factory.Server.Host.Services.CreateScope();
            services = scope.ServiceProvider.GetRequiredService<ServicesProvider>();

            return scope;
        }

        protected async Task loginAsync(string userName, string password)
        {
            var uri = "/Account/Login";
            var formData = new Dictionary<string, string>
            {
                { "Login", userName },
                { "Password", password },
            };

            await assertFormPostWithNoErrorOfAnyKindAsync(uri, formData);
            await assertGetWithNoErrorOfAnyKindAsync("/Account/CheckIfAuthentificated");

            //// Arrange
            //var uri = "/Account/Login";
            //var formData = new Dictionary<string, string>
            //{
            //    { "Login", userName },
            //    { "Password", password },
            //    { "__RequestVerificationToken", await ensureAntiforgeryTokenAsync(currentClient, uri) }
            //};

            //// Act
            //var response = await currentClient.PostAsync(uri, new FormUrlEncodedContent(formData));

            //// Assert
            //assertResponseWithNoErrorOfAnyKind(response);

        }

        protected async Task logoutAsync()
        {
            // Arrange
            var testEndpoint = "/Account/CheckIfAuthentificated";
            using (getServices(out var services))
            {
                // Act
                await assertGetWithNoErrorOfAnyKindAsync(testEndpoint);
                await assertGetWithNoErrorOfAnyKindAsync("/Account/Logout");

                // Assert
                await assertEndpointReturnsErrorPage(testEndpoint, HttpStatusCode.Unauthorized);
            }
        }

        static SetCookieHeaderValue _antiforgeryCookie;
        static string _antiforgeryToken;
        static readonly Regex AntiforgeryFormFieldRegex = new Regex(@"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
        protected static async Task<string> ensureAntiforgeryTokenAsync(HttpClient client, string uri)
        {
            if (_antiforgeryToken != null)
            {
                return _antiforgeryToken;
            }

            _antiforgeryCookie = null;
            _antiforgeryToken = null;
            client.DefaultRequestHeaders.Remove("Cookie");

            var response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            var hasValues = response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> values);
            hasValues = hasValues || response.Headers.TryGetValues("Cookie", out values);
            if (hasValues)
            {
                _antiforgeryCookie = SetCookieHeaderValue
                    .ParseList(values.ToList())
                    .SingleOrDefault(c => c.Name.StartsWith(".AspNetCore.AntiForgery.", StringComparison.InvariantCultureIgnoreCase));
            }
            Assert.NotNull(_antiforgeryCookie);
            client.DefaultRequestHeaders.Add("Cookie", new CookieHeaderValue(_antiforgeryCookie.Name, _antiforgeryCookie.Value).ToString());

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
