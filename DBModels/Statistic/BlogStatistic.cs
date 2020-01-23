using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBModels
{
    public class BlogStatistic : Statistic<BlogInfo, BlogDayStatistic>
    {

    }

    public class BlogDayStatistic : IDayStatistic, IDbEntity
    {
        public int Id { get; set; }
        public DateTime Day { get; set; }

        public int ActiveUsersCount { get; set; }
        public int BannedUsersCount { get; set; }
        public int UnconfirmedUsersCount { get; set; }
        public virtual ViewStatistic<Commentary> CommentariesViewStatistic { get; set; }
        public virtual ViewStatistic<Post> PostsViewStatistic { get; set; }
    }
}
