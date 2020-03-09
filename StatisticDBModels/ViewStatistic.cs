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

    public class BlogDayStatistic : IDayStatistic
    {
        [Key]public DateTime Day { get; private set; }

        public int PostsCount { get; set; }
        public int CommentaryCount { get; set; }
        public int UsersCount { get; set; }

        BlogDayStatistic() { } // For EFCore
        public BlogDayStatistic(DateTime day, BlogDayStatistic? prototype)
        {
            Day = day;

            if (prototype != null)
            {
                PostsCount = prototype.PostsCount;
                CommentaryCount = prototype.CommentaryCount;
                UsersCount = prototype.UsersCount;
            }
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
