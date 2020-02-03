using Blog.Misc;
using DBModels;
using Microsoft.EntityFrameworkCore;
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
        public StatisticService(ServicesLocator services) : base(services)
        {
            S.Db.SavingChanges += Db_SavingChanges;
        }

        void Db_SavingChanges()
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
                            updateUsersWithStateCount(stateProperty.CurrentValue.To<ProfileState>());
                        }
                        else if (stateProperty.IsModified)
                        {
                            updateUsersWithStateCount(stateProperty.OriginalValue.To<ProfileState>(), stateProperty.CurrentValue.To<ProfileState>());
                        }
                    }
                    if (entity is Post post) // Update blog statistic
                    {
                        updatePostsCount(post, entry.State == EntityState.Added);
                    }
                    if (entity is Commentary commentary) // Update blog statistic
                    {
                        updateCommentariesCount(commentary, entry.State == EntityState.Added);
                    }
                    else if (entry.Entity is IViewStatistic viewStatistic && entry.State == EntityState.Modified) // Update blog statistic
                    {
                        var totalViewsProperty = entry.Property(nameof(IViewStatistic.TotalViews));
                        var registredUserViewsProperty = entry.Property(nameof(IViewStatistic.RegisteredUserViews));
                        var totalViewsDelta = totalViewsProperty.CurrentValue.To<int>() - totalViewsProperty.OriginalValue.To<int>();
                        var registredUserViewsDelta = registredUserViewsProperty.CurrentValue.To<int>() - registredUserViewsProperty.OriginalValue.To<int>();
                        if (viewStatistic is IViewStatistic<Commentary>)
                        {
                            updateCommentariesViewStatistic(totalViewsDelta, registredUserViewsDelta);
                        }
                        else if (viewStatistic is IViewStatistic<Post>)
                        {
                            updatePostsViewStatistic(totalViewsDelta, registredUserViewsDelta);
                        }
                    }
                    else if (entity is UserAction userAction && entry.State == EntityState.Added) // Update user stat
                    {
                        updateUserActionsStatistic(userAction);
                    }
                    else if (entity is IEntityToCheck entityToCheck && entry.State == EntityState.Modified) // Update mod panel stat
                    {
                        var isResolvedProperty = entry.Property(nameof(IEntityToCheck.ResolvingTime));
                        if (isResolvedProperty.IsModified && isResolvedProperty.CurrentValue.To<DateTime?>() != null && entityToCheck.AssignedModerator != null)
                        {
                            var moderatorPanel = entityToCheck.AssignedModerator.ModeratorsGroup;
                            updateResolvedEntitiesStatistic(moderatorPanel, entityToCheck);
                        }
                    }
                }
            }
        }

        #region ### Blog-wide statistic ###

        void updatePostsCount(Post post, bool isCreated)
        {
            var currentDayStatistic = ensureHasThisDayBlogStatistic();
            currentDayStatistic.PostsCount += isCreated
                ? 1
                : (post.IsDeleted ? -1 : 0);
        }
        void updateCommentariesCount(Commentary commentary, bool isCreated)
        {
            var currentDayStatistic = ensureHasThisDayBlogStatistic();
            currentDayStatistic.CommentariesCount += isCreated
                ? 1
                : (commentary.IsDeleted ? -1 : 0);
        }

        void updateUsersWithStateCount(ProfileState newState)
        {
            var currentDayStatistic = ensureHasThisDayBlogStatistic();
            updateUsersWithStateCount(currentDayStatistic, newState, 1);
        }
        void updateUsersWithStateCount(ProfileState oldState, ProfileState newState)
        {
            var currentDayStatistic = ensureHasThisDayBlogStatistic();
            updateUsersWithStateCount(currentDayStatistic, oldState, -1);
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

        void updateCommentariesViewStatistic(int totalViewsDelta, int registredUserViewsDelta)
        {
            var statistic = ensureHasThisDayBlogStatistic();
            statistic.CommentariesViewStatistic.TotalViews += totalViewsDelta;
            statistic.CommentariesViewStatistic.RegisteredUserViews += registredUserViewsDelta;
        }
        void updatePostsViewStatistic(int totalViewsDelta, int registredUserViewsDelta)
        {
            var statistic = ensureHasThisDayBlogStatistic();
            statistic.PostsViewStatistic.TotalViews += totalViewsDelta;
            statistic.PostsViewStatistic.RegisteredUserViews += registredUserViewsDelta;
        }

        BlogDayStatistic ensureHasThisDayBlogStatistic()
        {
            var today = DateTime.UtcNow.Date;
            BlogDayStatistic dayStatistic;
            if (today != S.Db.Blog.Statistic.LastDayStatistic?.Day)
            {
                var lastStatistic = S.Db.Blog.Statistic.LastDayStatistic ?? new BlogDayStatistic();
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
                    }
                };
                S.Db.Blog.Statistic.DayStatistics.Add(dayStatistic);
            }
            else
            {
                dayStatistic = S.Db.Blog.Statistic.LastDayStatistic;
            }

            return dayStatistic;
        }

        #endregion

        #region ### ModeratorsGroup statistic ###

        void updateResolvedEntitiesStatistic(ModeratorsGroup entityOwner, IEntityToCheck entity)
        {
            if (entity.IsResolved)
            {
                var statistic = ensureHasThisDayModeratorsGroupStatistic(entityOwner);
                statistic.SummedTimeToAssignation += entity.AssignationTime.Value - entity.AddTime;
                statistic.SummedTimeFromAssignationToResolving += entity.ResolvingTime.Value - entity.AssignationTime.Value;
                statistic.SummedResolveTime += entity.ResolvingTime.Value - entity.AddTime;
                statistic.ResolvedEntitiesCount++;
            }
        }

        ModeratorsGroupDayStatistic ensureHasThisDayModeratorsGroupStatistic(ModeratorsGroup moderatorsGroup)
        {
            var today = DateTime.UtcNow.Date;
            ModeratorsGroupDayStatistic dayStatistic;
            if (today != moderatorsGroup.Statistic.LastDayStatistic?.Day)
            {
                var lastStatistic = moderatorsGroup.Statistic.LastDayStatistic ?? new ModeratorsGroupDayStatistic();
                dayStatistic = new ModeratorsGroupDayStatistic()
                {
                    Day = today,
                    ResolvedEntitiesCount = lastStatistic.ResolvedEntitiesCount,
                    SummedResolveTime = lastStatistic.SummedResolveTime,
                    SummedTimeFromAssignationToResolving = lastStatistic.SummedTimeFromAssignationToResolving,
                    SummedTimeToAssignation = lastStatistic.SummedTimeToAssignation
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
