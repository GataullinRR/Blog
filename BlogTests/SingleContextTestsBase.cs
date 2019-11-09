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
        protected static CustomWebApplicationFactory<Startup> instantiateApplication()
        {
            var factory = new CustomWebApplicationFactory<Startup>();
            factory.CreateClient(); // It'll initialize the server
            using (var scope = factory.Server.Host.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<BlogContext>();

                DbSampleData.Initialize(db,
                    scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>(),
                    scope.ServiceProvider.GetRequiredService<UserManager<User>>(),
                    scope.ServiceProvider.GetRequiredService<SignInManager<User>>());
            }

            return factory;
        }

        protected readonly ITestOutputHelper _output;
        protected abstract HttpClient currentClient { get; }
        protected abstract CustomWebApplicationFactory<Startup> factory { get; }

        public SingleContextTestsBase(ITestOutputHelper output)
        {
            this._output = output;
        }

        protected async Task assertEndpointAccessible(string url)
        {
            _output.WriteLine(url);

            // Act
            var response = await currentClient.GetAsync(url);

            // Assert
            ensureOkResponse(response);
        }
        protected async Task assertEndpointReturnsErrorPage(string url, HttpStatusCode statusCode)
        {
            _output.WriteLine(url);

            // Act
            var response = await currentClient.GetAsync(url);

            ensureOkResponse(response);
            Assert.NotNull(response?.RequestMessage?.RequestUri?.AbsoluteUri);
            Assert.EndsWith($"code={(int)statusCode}", response.RequestMessage.RequestUri.AbsoluteUri);
        }
        protected void ensureOkResponse(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8", response.Content.Headers.ContentType.ToString());
        }

        protected IDisposable getServices(out ServicesProvider services)
        {
            var scope = factory.Server.Host.Services.CreateScope();
            services = scope.ServiceProvider.GetRequiredService<ServicesProvider>();

            return scope;
        }

        static SetCookieHeaderValue _antiforgeryCookie;
        static string _antiforgeryToken;
        static readonly Regex AntiforgeryFormFieldRegex = new Regex(@"\<input name=""__RequestVerificationToken"" type=""hidden"" value=""([^""]+)"" \/\>");
        protected static async Task<string> ensureAntiforgeryTokenAsync(HttpClient client, string uri)
        {
            //if (_antiforgeryToken != null)
            //{
            //    return _antiforgeryToken;
            //}

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
