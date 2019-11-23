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
using System.Linq;
using Blog.Controllers;
using Blog.Pages.Account;

namespace BlogTests
{
    public class ModeratorPanelTests : SingleContextTestsBase, IClassFixture<CustomWebApplicationFactory>
    {
        static HttpClient _testsCommonClient;
        protected override HttpClient currentClient
        {
            get => base.currentClient;
            set
            {
                base.currentClient = value;
                _testsCommonClient = value;
            }
        }


        public ModeratorPanelTests(CustomWebApplicationFactory factory, ITestOutputHelper output) 
            : base(factory, output)
        {
            if (_testsCommonClient == null)
            {
                initializeApplication();
                currentClient = _factory.CreateClient();
            }
            else
            {
                currentClient = _testsCommonClient;
            }
        }

        [Theory(), Order(0)]
        [InlineData("Commentary")]
        [InlineData("Post")]
        [InlineData("Profile")]
        public async Task CommentaryReport(string testingEntity)
        {
            using (getServices(out var services))
            {
                var moderator = await services.UserManager.GetUsersInRoleAsync(Roles.MODERATOR).ThenDo(ms => ms.Single());
                var target = testingEntity.Select<IEnumerable<IReportable>>(
                    (te => (string)te == "Commentary", services.Db.Commentaries),
                    (te => (string)te == "Post", services.Db.Posts),
                    (te => (string)te == "Profile", services.Db.ProfilesInfos))
                    .Single().Where(c => c.Author != moderator).FirstItem();
                target.ViewStatistic.TotalViews = 10;
                await services.Db.SaveChangesAsync();
                var reporters = new[] 
                {
                    await services.UserManager.FindByNameAsync("_KSY_"),
                    await services.UserManager.FindByNameAsync("QTU100")
                };
                foreach (var reporter in reporters)
                {
                    using (await loggedUserScope(reporter.UserName))
                    {
                        await assertGetWithNoErrorOfAnyKindAsync($"/Reporting/Report{testingEntity}Async?id={target.Id}");
                    }
                }
                await services.Db.SaveChangesAsync();
                await services.Db.Entry(target).ReloadAsync();
                await services.Db.Entry(moderator).ReloadAsync();
                Assert.Equal(reporters.Length, target.Reports.Count());
                if (target is Commentary commentary)
                {
                    Assert.True(commentary.IsHidden);
                }
                Assert.Equal(target.Id, moderator.ModeratorsGroup.EntitiesToCheck.LastItem().Entity.To<IDbEntity>().Id);
                Assert.Equal(moderator, target.Author.ModeratorsInChargeGroup.Moderators.Single());
                Assert.Empty(moderator.ModeratorsInChargeGroup.Moderators);

                async Task<IDisposable> loggedUserScope(string userName)
                {
                    await loginAsync(userName, "QTU100@yandex.ru");
                    return new DisposingAction(() =>
                    {
                        logoutAsync().GetAwaiter().GetResult();
                    });
                }
            }
        }
    }
}
