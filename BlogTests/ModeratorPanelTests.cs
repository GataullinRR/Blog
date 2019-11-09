using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using Xunit;
using System.Net.Http;
using Blog;
using Microsoft.Extensions.DependencyInjection;
using DBModels;
using Microsoft.AspNetCore.Identity;
using Xunit.Abstractions;
using System.Threading.Tasks;

namespace BlogTests
{
    public class ModeratorPanelTests : SingleContextTestsBase
    {
        static readonly CustomWebApplicationFactory<Startup> _factory;
        static HttpClient _client;

        static ModeratorPanelTests()
        {
            _factory = instantiateApplication();
            _client = _factory.CreateClient();
        }

        protected override CustomWebApplicationFactory<Startup> factory => _factory;
        protected override HttpClient currentClient => _client;

        public ModeratorPanelTests(ITestOutputHelper output) : base(output)
        {

        }

        //[Fact]
        //public async Task ass()
        //{
        //    using (getServices(out var services))
        //    {
        //        var moderator = await services.UserManager.FindByNameAsync("Oleg");
        //        await loginAsync(moderator.UserName, "");
        //        var targetCommentary = services.Db.Commentaries.FirstItem();
        //        targetCommentary.Reports.Add(new Report(await services.UserManager.FindByNameAsync("_KSY_"), targetCommentary.Author, targetCommentary));
        //        await services.Db.SaveChangesAsync();
        //        for (int i = 0; i < 10; i++)
        //        {
        //            await _client.GetAsync($"/Commentary?id={targetCommentary.Id}");
        //        }
        //        await _client.GetAsync($"/Account/ReportCommentary?id={targetCommentary.Id}");
        //    }
        //}

        //protected async Task loginAsync(string login, string password)
        //{

        //}
    }
}
