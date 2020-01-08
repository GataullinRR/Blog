using Blog.Misc;
using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class RepositoryService : ServiceBase
    {
        public RepositoryService(ServicesProvider services) : base(services)
        {

        }

        public async Task AddUserActionAsync(User targetUser, UserAction newAction)
        {
            targetUser.Actions.Add(newAction);
            ensureHasThisDayStatistic();
            ensureHasAppropriateCounter().Count++;

            return; ////////////////////////////////////////////

            void ensureHasThisDayStatistic()
            {
                var today = newAction.ActionDate.Date;
                UserDayStatistic dayStatistic;
                if (today != targetUser.Statistic.LastDayStatistic?.Day)
                {
                    dayStatistic = new UserDayStatistic()
                    {
                        Day = today
                    };
                    targetUser.Statistic.DayStatistics.Add(dayStatistic);
                }
                else
                {
                    dayStatistic = targetUser.Statistic.LastDayStatistic;
                }
            }

            IActionStatistic ensureHasAppropriateCounter()
            {
                ActionStatistic<UserStatistic> actionCounter;
                UserDayStatistic thisDayStatistic = targetUser.Statistic.LastDayStatistic;
                if (thisDayStatistic.Actions.Any(s => s.ActionType == newAction.ActionType))
                {
                    actionCounter = thisDayStatistic.Actions.Find(s => s.ActionType == newAction.ActionType);
                }
                else
                {
                    actionCounter = new ActionStatistic<UserStatistic>()
                    {
                        ActionType = newAction.ActionType
                    };
                    thisDayStatistic.Actions.Add(actionCounter);
                }

                return actionCounter;
            }
        }
    }
}
