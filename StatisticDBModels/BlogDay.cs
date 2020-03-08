using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticDBModels
{
    public interface IDayStatistic
    {
        DateTime Day { get; set; }
    }

    class PostDayStat : IDayStatistic
    {
        public DateTime Day { get; set; }
        
        public ICollection<ViewType> ViewStatistic { get; set; }


    }

    class BlogDay : IDayStatistic
    {
        public int BlogDayId { get; set; }

        public DateTime Day { get; set; }
        public PublicEntityStatistic PostsStatistic { get; set; }
        public PublicEntityStatistic CommentaryStatistic { get; set; }
        public PublicEntityStatistic ProfileStatistic { get; set; }
    }

    public class PublicEntityStatistic
    {
        public int PublicEntityStatisticId { get; set; }
     
        public ICollection<ViewType> ViewStatistic { get; set; }
        public int EntitiesCount { get; set; }

        public int BlogDayId { get; set; }
    }

    public class ViewType
    {
        public int ViewTypeId { get; set; }

        public int TotalViews { get; set; }
        public int RegisteredUserViews { get; set; }

        public int PublicEntityStatisticId { get; set; }
    }
}
