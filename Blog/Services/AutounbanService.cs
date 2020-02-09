using DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Blog.Services
{
    public class AutounbanService
    {
        public static readonly TimeSpan UPDATE_INTERVAL = TimeSpan.FromDays(1);

        readonly ILogger<AutounbanService> _log;
        readonly IServiceScopeFactory _scopeFactory;

        public AutounbanService(ILogger<AutounbanService> log, IServiceScopeFactory scopeFactory)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));

            unbanLoopAsync();
        }

#warning slow...
        async void unbanLoopAsync()
        {
            await ThreadingUtils.ContinueAtDedicatedThread();

            while (true)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var db = scope.ServiceProvider.GetService<BlogContext>();
                    foreach (var user in db.Users.Where(u => u.Status.State == ProfileState.BANNED))
                    {
                        if (user.Status.BannedTill < DateTime.UtcNow)
                        {
                            _log.LogInformation($"Trying to unban {user.UserName}");

                            user.Status.State = user.EmailConfirmed
                                ? ProfileState.ACTIVE
                                : ProfileState.RESTRICTED;
                            user.Status.BannedTill = null;
                            user.Status.StateReason = null;
                        }
                    }

                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _log.LogError(ex, "Can't unban");
                    }
                }

                await Task.Delay(UPDATE_INTERVAL);
            }
        }
    }
}
