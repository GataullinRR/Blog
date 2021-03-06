﻿using Utilities.Types;
using Blog.Attributes;
using Blog.Misc;
using Confluent.Kafka;
using DBModels;
using Microsoft.EntityFrameworkCore;
using MVVMUtilities.Types;
using StatisticServiceExports;
using StatisticServiceExports.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class StatisticService : ServiceBase
    {
        enum ExecutionOrder
        {
            EXECUTE_AFTER_SAVE_CHANGES
        }

        delegate IAsyncEnumerator<ExecutionOrder> StepDelegate();
        StepDelegate _queue;

        public StatisticService(ServiceLocator services) : base(services)
        {
            S.Db.SavingChanges += Db_SavingChanges;
            S.Db.ChangesSaved += Db_ChangesSaved;
        }

        async void Db_SavingChanges(BusyObject savingChanges)
        {
            using (savingChanges.BusyMode)
            {
                S.Db.ChangeTracker.DetectChanges();

                foreach (var entry in S.Db.ChangeTracker.Entries().ToArray())
                {
                    if (!entry.State.IsOneOf(EntityState.Unchanged, EntityState.Detached))
                    {
                        var entity = entry.Entity;
                        if (entity is ProfileStatus) // Update blog statistic
                        {
                            var stateProperty = entry.Property(nameof(ProfileStatus.State));
                            if (entry.State == EntityState.Added)
                            {
                                await updateUsersWithStateCount(stateProperty.OriginalValue.To<ProfileState>(), stateProperty.CurrentValue.To<ProfileState>());
                            }
                            else if (stateProperty.IsModified)
                            {
                                await updateUsersWithStateCount(stateProperty.OriginalValue.To<ProfileState>(), stateProperty.CurrentValue.To<ProfileState>());
                            }
                        }
                        if (entity is Post post) // Update blog statistic
                        {
                            _queue += () => updatePostsCount(post, entry.State == EntityState.Added);
                        }
                        if (entity is Commentary commentary) // Update blog statistic
                        {
                            _queue += () => updateCommentariesCount(commentary, entry.State == EntityState.Added);
                        }
                        else if (entry.Entity is IViewStatistic viewStatistic && entry.State == EntityState.Modified) // Update blog statistic
                        {
                            var totalViewsProperty = entry.Property(nameof(IViewStatistic.TotalViews));
                            var registredUserViewsProperty = entry.Property(nameof(IViewStatistic.RegisteredUserViews));
                            var totalViewsDelta = totalViewsProperty.CurrentValue.To<int>() - totalViewsProperty.OriginalValue.To<int>();
                            var registredUserViewsDelta = registredUserViewsProperty.CurrentValue.To<int>() - registredUserViewsProperty.OriginalValue.To<int>();
                            if (viewStatistic is IViewStatistic<Commentary> commentaryVStat)
                            {
                                await updateCommentariesViewStatisticAsync(commentaryVStat.Owner.Id, totalViewsDelta, registredUserViewsDelta);
                            }
                            else if (viewStatistic is IViewStatistic<Post> postVStat)
                            {
                                await updatePostsViewStatisticAsync(postVStat.Owner.Id, totalViewsDelta, registredUserViewsDelta);
                            }
                        }
                        else if (entity is UserAction userAction && entry.State == EntityState.Added) // Update user stat
                        {
                            updateUserActionsStatistic(userAction);
                        }
                        else if (entity is IEntityToCheck entityToCheck && entry.State == EntityState.Modified) // Update mod panel stat
                        {
                            var isResolvedProperty = entry.Property(nameof(IEntityToCheck.ResolvingTime));
                            if (isResolvedProperty.IsModified && 
                                isResolvedProperty.CurrentValue.To<DateTime?>() != null && 
                                entityToCheck.AssignedModerator != null)
                            {
                                await S.Db.Entry(entityToCheck)
                                    .Reference(e => e.AssignedModerator)
                                    .LoadAsync();
                                await S.Db.Entry(entityToCheck.AssignedModerator)
                                    .Reference(m => m.ModeratorsGroup)
                                    .LoadAsync();
                                var moderatorPanel = entityToCheck.AssignedModerator.ModeratorsGroup;
                                await updateResolvedEntitiesStatisticAsync(moderatorPanel, entityToCheck);
                            }
                        }
                    }
                }

                StepDelegate newQueue = null;
                // WTF. Why _queue?.GetInvocationList()?.Select(d => (StepDelegate)d)?.NullToEmpty() fails with NullRefEx?
                foreach (var action in  _queue?.GetInvocationList()?.Select(d => (StepDelegate)d) ?? Enumerable.Empty<StepDelegate>())
                {
                    var iterator = action();
                    var hasValue = await iterator.MoveNextAsync();
                    if (hasValue) // go to next step
                    {
                        switch (iterator.Current)
                        {
                            case ExecutionOrder.EXECUTE_AFTER_SAVE_CHANGES:
                                newQueue += () => iterator;
                                break;
                         
                            default:
                                throw new NotSupportedException();
                        }
                    }
                }
                _queue = newQueue;
            }
        }

        async void Db_ChangesSaved(SaveChangesResult result, BusyObject busy)
        {
            using (busy.BusyMode)
            {
                if (result.IsSuccessful && _queue != null)
                {
                    foreach (var action in _queue.GetInvocationList().Select(d => (StepDelegate)d))
                    {
                        var iterator = action();
                        var hasValue = await iterator.MoveNextAsync();
                        if (hasValue)
                        {
                            throw new InvalidOperationException();
                        }
                    }
                }
            }
        }

        #region ### Blog-wide statistic ###

        async IAsyncEnumerator<ExecutionOrder> updatePostsCount(Post post, bool isCreated)
        {
            var currentDayStatistic = await ensureHasThisDayBlogStatistic();
            currentDayStatistic.PostsCount += isCreated
                ? 1
                : (post.IsDeleted ? -1 : 0);

            if (isCreated) // Because before save changes id is not set
            {
                yield return ExecutionOrder.EXECUTE_AFTER_SAVE_CHANGES;
            }

            await S.StatisticServiceAPI.OnPostActionAsync(new PostNotification(post.Id, isCreated ? PostAction.CREATED : PostAction.DELETED));
        }
        async IAsyncEnumerator<ExecutionOrder> updateCommentariesCount(Commentary commentary, bool isCreated)
        {
            var currentDayStatistic = await ensureHasThisDayBlogStatistic();
            currentDayStatistic.CommentariesCount += isCreated
                ? 1
                : (commentary.IsDeleted ? -1 : 0);

            if (isCreated) // Because before save changes commentary id is not set
            {
                yield return ExecutionOrder.EXECUTE_AFTER_SAVE_CHANGES;
            }

            await S.StatisticServiceAPI.OnCommentaryActionAsync(new CommentaryNotification(commentary.Id, isCreated ? CommentaryAction.CREATED : CommentaryAction.DELETED));
        }

        async Task updateUsersWithStateCount(ProfileState? oldState, ProfileState newState)
        {
            UserNotification userEvent = null;
            if (oldState == null) // User've just registered
            {
                userEvent = new UserNotification(new UserNotification.RegisteredInfo((int)newState));
            }
            else
            {
                userEvent = new UserNotification(new UserNotification.StateChangedInfo((int)oldState, (int)newState));
            }
            await S.StatisticServiceAPI.OnUserActionAsync(userEvent);

            var currentDayStatistic = await ensureHasThisDayBlogStatistic();
            if (oldState != null)
            {
                updateUsersWithStateCount(currentDayStatistic, oldState.Value, -1);
            }
            updateUsersWithStateCount(currentDayStatistic, newState, 1);
        }
        void updateUsersWithStateCount(BlogDayStatistic blogDayStatistic, ProfileState state, int delta)
        {
            switch (state)
            {
                case ProfileState.RESTRICTED:
                    blogDayStatistic.UnconfirmedUsersCount += delta;
                    break;

                case ProfileState.ACTIVE:
                    blogDayStatistic.ActiveUsersCount += delta;
                    break;

                case ProfileState.BANNED:
                    blogDayStatistic.BannedUsersCount += delta;
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        async Task updateCommentariesViewStatisticAsync(int commentaryId, int totalViews, int registeredUserViews)
        {
            await S.StatisticServiceAPI.OnSeenAsync(new SeenNotification(false)
            {
                SeenCommentaries = new Dictionary<int, int> { { commentaryId, totalViews } }
            });
            await S.StatisticServiceAPI.OnSeenAsync(new SeenNotification(true)
            {
                SeenCommentaries = new Dictionary<int, int> { { commentaryId, registeredUserViews } }
            });

            var statistic = await ensureHasThisDayBlogStatistic();
            statistic.CommentariesViewStatistic.TotalViews += totalViews;
            statistic.CommentariesViewStatistic.RegisteredUserViews += registeredUserViews;
        }
        async Task updatePostsViewStatisticAsync(int postId, int totalViews, int registeredUserViews)
        {
            await S.StatisticServiceAPI.OnSeenAsync(new SeenNotification(false)
            {
                SeenCommentaries = new Dictionary<int, int> { { postId, totalViews } }
            });
            await S.StatisticServiceAPI.OnSeenAsync(new SeenNotification(true)
            {
                SeenCommentaries = new Dictionary<int, int> { { postId, registeredUserViews } }
            });

            var statistic = await ensureHasThisDayBlogStatistic();
            statistic.PostsViewStatistic.TotalViews += totalViews;
            statistic.PostsViewStatistic.RegisteredUserViews += registeredUserViews;
        }

        async Task<BlogDayStatistic> ensureHasThisDayBlogStatistic()
        {
            var today = DateTime.UtcNow.Date;
            BlogDayStatistic dayStatistic;

            var lastStatistic = await S.Db.BlogDayStatistics
                .Include(ds => ds.CommentariesViewStatistic)
                .Include(ds => ds.PostsViewStatistic)
                .OrderByDescending(ds => ds.Day)
                .FirstOrDefaultAsync();
            if (today != lastStatistic?.Day)
            {
                lastStatistic = lastStatistic ?? new BlogDayStatistic();
                dayStatistic = new BlogDayStatistic()
                {
                    Day = today,
                    ActiveUsersCount = lastStatistic.ActiveUsersCount,
                    BannedUsersCount = lastStatistic.BannedUsersCount,
                    UnconfirmedUsersCount = lastStatistic.UnconfirmedUsersCount,
                    PostsCount = lastStatistic.PostsCount,
                    CommentariesCount = lastStatistic.CommentariesCount,
                    CommentariesViewStatistic = new ViewStatistic<Commentary>()
                    {
                        RegisteredUserViews = lastStatistic.CommentariesViewStatistic.RegisteredUserViews,
                        TotalViews = lastStatistic.CommentariesViewStatistic.TotalViews,
                    },
                    PostsViewStatistic = new ViewStatistic<Post>()
                    {
                        RegisteredUserViews = lastStatistic.PostsViewStatistic.RegisteredUserViews,
                        TotalViews = lastStatistic.PostsViewStatistic.TotalViews,
                    },
                };

                await S.Db.Blogs
                    .Include(b => b.Statistic)
                    .ThenInclude(s => s.DayStatistics)
                    .FirstOrDefaultAsync()
                    .ThenDo(b => b.Statistic.DayStatistics.Add(dayStatistic));
            }
            else
            {
                dayStatistic = lastStatistic;
            }

            return dayStatistic;
        }

        #endregion

        #region ### ModeratorsGroup statistic ###

        async Task updateResolvedEntitiesStatisticAsync(ModeratorsGroup entityOwner, IEntityToCheck entity)
        {
            if (entity.IsResolved)
            {
                var statistic = await ensureHasThisDayModeratorsGroupStatisticAsync(entityOwner);
                statistic.SummedTimeToAssignation += (entity.AssignationTime.Value - entity.AddTime).TotalSeconds;
                statistic.SummedTimeFromAssignationToResolving += (entity.ResolvingTime.Value - entity.AssignationTime.Value).TotalSeconds;
                statistic.SummedResolveTime += (entity.ResolvingTime.Value - entity.AddTime).TotalSeconds;
                statistic.ResolvedEntitiesCount++;
            }
        }

        async Task<ModeratorsGroupDayStatistic> ensureHasThisDayModeratorsGroupStatisticAsync(ModeratorsGroup moderatorsGroup)
        {
            var today = DateTime.UtcNow.Date;
            ModeratorsGroupDayStatistic dayStatistic;
            await S.Db.Entry(moderatorsGroup)
                .Reference(g => g.Statistic)
                .LoadAsync();
            await S.Db.Entry(moderatorsGroup.Statistic)
                .Collection(s => s.DayStatistics)
                .LoadAsync();
            if (today != moderatorsGroup.Statistic.LastDayStatistic?.Day)
            {
                var lastStatistic = moderatorsGroup.Statistic.LastDayStatistic ?? new ModeratorsGroupDayStatistic();
                dayStatistic = new ModeratorsGroupDayStatistic()
                {
                    Day = today,
                    ResolvedEntitiesCount = lastStatistic.ResolvedEntitiesCount,
                    SummedResolveTime = lastStatistic.SummedResolveTime,
                    SummedTimeFromAssignationToResolving = lastStatistic.SummedTimeFromAssignationToResolving,
                    SummedTimeToAssignation = lastStatistic.SummedTimeToAssignation,
                };
                moderatorsGroup.Statistic.DayStatistics.Add(dayStatistic);
            }
            else
            {
                dayStatistic = moderatorsGroup.Statistic.LastDayStatistic;
            }

            return dayStatistic;
        }

        #endregion

        #region ### User statistic ###

        void updateUserActionsStatistic(UserAction addedAction)
        {
            var statistic = ensureHasThisDayUserStatistic(addedAction.Author);
            ensureHasAppropriateCounter().Count++;

            IActionStatistic ensureHasAppropriateCounter()
            {
                ActionStatistic<UserStatistic> actionCounter;
                if (statistic.Actions.Any(s => s.ActionType == addedAction.ActionType))
                {
                    actionCounter = statistic.Actions.Find(s => s.ActionType == addedAction.ActionType);
                }
                else
                {
                    actionCounter = new ActionStatistic<UserStatistic>()
                    {
                        ActionType = addedAction.ActionType
                    };
                    statistic.Actions.Add(actionCounter);
                }

                return actionCounter;
            }
        }

        UserDayStatistic ensureHasThisDayUserStatistic(User user)
        {
            var today = DateTime.UtcNow.Date;
            UserDayStatistic dayStatistic;
            if (today != user.Statistic.LastDayStatistic?.Day)
            {
                dayStatistic = new UserDayStatistic()
                {
                    Day = today
                };
                user.Statistic.DayStatistics.Add(dayStatistic);
            }
            else
            {
                dayStatistic = user.Statistic.LastDayStatistic;
            }

            return dayStatistic;
        }

        #endregion
    }
}