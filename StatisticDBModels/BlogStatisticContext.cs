using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MVVMUtilities.Types;

namespace StatisticDBModels
{
    public class BlogStatisticContext : DbContext
    {
        public DbSet<ViewStatistic<int>> PostViewStatistics { get; set; }
        public DbSet<ViewStatistic<int>> CommentaryViewStatistics { get; set; }
        public DbSet<ViewStatistic<int>> ProfileViewStatistics { get; set; }
        public DbSet<BlogStatistic> BlogStatistic { get; set; }
        public DbSet<ModeratorsGroupStatistic> ModeratorsGroupStatistics { get; set; }
        
        public DbSet<UserStatistic> UserStatistics { get; set; }
        public DbSet<UserDayStatistic> UserDayStatistics { get; set; }
             
        public BlogStatisticContext(DbContextOptions<BlogStatisticContext> options)
            : base(options)
        {
            // Dont want to spend 4ms each time on
            // IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') SELECT 1 ELSE SELECT 0
            // query

            // Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Not required for ef core 3.0
            // optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}