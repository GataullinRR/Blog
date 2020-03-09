using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Utilities.Extensions;

namespace StatisticDBModels
{
    public interface IDayStatistic
    {
        DateTime Day { get; }
    }



    public class ModeratorPanelDayStatistic : IDayStatistic
    {
        public int ModeratorGroupId { get; private set; }
        public DateTime Day { get; private set; }

        public int EntitiesResolved { get; set; }
        public double? SummedTimeToAssignation { get; set; }
        public double? SummedTimeToResolve { get; set; }
    }

    public class ModeratorDayStatistic : IDayStatistic
    {
        public string ModeratorId { get; private set; }
        public DateTime Day { get; private set; }

        public int EntitiesResolved { get; set; }
    }

    public class BlogDayStatistic : IDayStatistic
    {
        [Key]public DateTime Day { get; private set; }

        public int PostsCount { get; set; }
        public int CommentariesCount { get; set; }
        public int UsersCount { get; set; }
        public ICollection<UserWithStateCount> UsersWithStateCount { get; } = new List<UserWithStateCount>();

        BlogDayStatistic() { } // For EFCore
        public BlogDayStatistic(DateTime day, BlogDayStatistic? prototype)
        {
            Day = day;

            if (prototype != null)
            {
                PostsCount = prototype.PostsCount;
                CommentariesCount = prototype.CommentariesCount;
                UsersCount = prototype.UsersCount;
            }
        }
    }

    public class UserWithStateCount
    {
        [InverseProperty(nameof(BlogDayStatistic.UsersWithStateCount))]
        public DateTime Day { get; private set; }
        public int State { get; private set; }

        public int Count { get; set; }

        UserWithStateCount() { } // For EFCore
        public UserWithStateCount(int state)
        {
            State = state;
        }
    }

    public interface IViewStatistic
    {
        int AllViews { get; set; }
        int RegisteredUserViews { get; set; }
    }

    public class ViewStatistic<T, TMarker> : IViewStatistic
    {
        [Key] 
        public T Id { get; set; }

        public int AllViews { get; set; }
        public int RegisteredUserViews { get; set; }
    }
}
