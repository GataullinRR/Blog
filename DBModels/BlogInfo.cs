using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBModels
{
    public class BlogInfo : IDbEntity
    {
        public int Id { get; set; }
        [Required] public virtual BlogStatistic Statistic { get; set; }

        public BlogInfo()
        {

        }

        public BlogInfo(BlogStatistic statistic)
        {
            Statistic = statistic ?? throw new ArgumentNullException(nameof(statistic));
        }
    }
}
