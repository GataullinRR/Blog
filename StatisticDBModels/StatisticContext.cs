using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace StatisticDBModels
{
    public class StatisticContext : DbContext
    {
        public DbSet<ViewStatistic<int, Markers.AClass>> PostsViewStatistic { get; set; }
        public DbSet<ViewStatistic<int, Markers.BClass>> CommentariesViewStatistic { get; set; }
        public DbSet<ViewStatistic<int, Markers.CClass>> ProfilesViewStatistic { get; set; }

        public DbSet<BlogDayStatistic> BlogDaysStatistic { get; set; }

        public StatisticContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<BlogDayStatistic>()
            //    .Property(ds => ds.Day)
            //    .HasField(BlogDayStatistic.ID_FIELD_NAME)
            //    .UsePropertyAccessMode(PropertyAccessMode.FieldDuringConstruction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
