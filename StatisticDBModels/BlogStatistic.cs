using CommonDBModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StatisticDBModels
{
    public class BlogStatistic : Statistic<int, BlogDayStatistic>
    {

    }

    public class BlogDayStatistic : IDayStatistic, IDbEntity
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }

        public int ActiveUsersCount { get; set; }
        public int BannedUsersCount { get; set; }
        public int UnconfirmedUsersCount { get; set; }

        public int CommentariesCount { get; set; }
        public int PostsCount { get; set; }

        public ViewStatistic<int> CommentariesViewStatistic { get; set; }
        public ViewStatistic<int> PostsViewStatistic { get; set; }
    }
}
