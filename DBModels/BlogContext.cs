using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
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

namespace DBModels
{
    public class BlogContext : IdentityDbContext<User>
    {
        /// <summary>
        /// To wait each async event handler to finish execution
        /// </summary>
        readonly BusyObject _savingChanges = new BusyObject();
        public event Action<BusyObject> SavingChanges;

        public DbSet<Post> Posts { get; set; }
        public DbSet<Commentary> Commentaries { get; set; }
        public DbSet<PostEdit> PostsEdits { get; set; }
        public DbSet<CommentaryEdit> CommentaryEdits { get; set; }
        public DbSet<Violation> UserRuleViolations { get; set; }
        public DbSet<ProfileStatus> ProfilesStatuses { get; set; }
        public DbSet<Profile> ProfilesInfos { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<UserAction> UsersActions { get; set; }
        public DbSet<ModeratorsGroup> ModeratorsGroups { get; set; }
        public DbSet<TokenMetadata> TokenMetadatas { get; set; }
        public DbSet<EntityToCheck<Post>> PostsToCheck { get; set; }
        public DbSet<EntityToCheck<Commentary>> CommentariesToCheck { get; set; }
        public DbSet<EntityToCheck<Profile>> ProfilesToCheck { get; set; }
        public IEnumerable<IEntityToCheck> EntitiesToCheck =>
            new Enumerable<IEntityToCheck>()
            {
                PostsToCheck,
                ProfilesToCheck,
                CommentariesToCheck
            };
        public DbSet<ModerationInfo> ModerationInfos { get; set; }
        public DbSet<ViewStatistic<Commentary>> CommentaryViews { get; set; }
        public DbSet<ViewStatistic<Post>> PostViews { get; set; }
        public DbSet<ViewStatistic<Profile>> ProfileViews { get; set; }
        public DbSet<ModeratorsGroupStatistic> ModeratorsGroupStatistics { get; set; }
        public DbSet<BlogDayStatistic> BlogDayStatistics { get; set; }
        public DbSet<BlogStatistic> BlogStatistics { get; set; }
        /// <summary>
        /// Contains single entity
        /// </summary>
        public DbSet<BlogInfo> Blogs { get; set; }
        public BlogInfo Blog => Blogs.Single();

        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
            // Dont want to spend 4ms each time on
            // IF EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE') SELECT 1 ELSE SELECT 0
            // query
            Database.EnsureCreated();

            ChangeTracker.LazyLoadingEnabled = false;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseLazyLoadingProxies(); Not anymore!

            optionsBuilder.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.QueryClientEvaluationWarning));

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Commentary>()
                .HasOne(c => c.Author)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<Profile>()
            //    .HasOne(p => p.ViewStatistic)
            //    .WithOne()
            //    .HasForeignKey<ViewStatistic<Profile>>(b => b.OwnerId);
            modelBuilder.Entity<Post>()
                .HasOne(p => p.ViewStatistic)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Commentary>()
                .HasOne(p => p.ViewStatistic)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<User>()
                .HasOne(p => p.Profile)
                .WithOne(i => i.Author)
                .HasForeignKey<Profile>(b => b.AuthorForeignKey);

            modelBuilder.Entity<ModeratorsGroup>(b =>
            {
                b.HasOne<ModeratorsGroupStatistic>(e => e.Statistic)
                    .WithOne(e => e.Owner)
                    .HasForeignKey<ModeratorsGroupStatistic>(e => e.Id);
            });
            modelBuilder.Entity<User>(b =>
            {
                b.HasOne<UserStatistic>(e => e.Statistic)
                    .WithOne(e => e.Owner)
                    .HasForeignKey<UserStatistic>(e => e.Id);
            });
            modelBuilder.Entity<BlogInfo>(b =>
            {
                b.HasOne<BlogStatistic>(e => e.Statistic)
                    .WithOne(e => e.Owner)
                    .HasForeignKey<BlogStatistic>(e => e.Id);
            });

            //modelBuilder.Entity<ModeratorsGroup>(b =>
            //{
            //    b.OwnsOne<ModeratorsGroupStatistic>(e => e.Statistic);
            //    b.HasOne<ModeratorsGroupStatistic>(e => e.Statistic)
            //        .WithOne(e => e.Owner)
            //        .HasForeignKey<ModeratorsGroupStatistic>(e => e.Id);
            //});
            //modelBuilder.Entity<User>(b =>
            //{
            //    b.OwnsOne<UserStatistic>(e => e.Statistic);
            //    b.HasOne<UserStatistic>(e => e.Statistic)
            //        .WithOne(e => e.Owner)
            //        .HasForeignKey<UserStatistic>(e => e.Id);
            //});

            //modelBuilder.Entity<ModeratorsGroupStatistic>(b =>
            //{
            //    b.OwnsMany<ModeratorsGroupDayStatistic>(e => e.DayStatistics);
            //});
            //modelBuilder.Entity<UserStatistic>(b =>
            //{
            //    b.OwnsMany<UserDayStatistic>(e => e.DayStatistics);
            //});
            //modelBuilder.Entity<UserStatistic>(b =>
            //{
            //    b.HasMany<UserDayStatistic>(e => e.DayStatistics)
            //        .WithOne(e => e.Owner);
            //});

            //modelBuilder.Entity<UserAction>()
            //    .Property(e => e.ActionType)
            //    .HasConversion(
            //        v => v.ToString(),
            //        v => EnumUtils.CastSafe<ActionType>(v.ParseToInt32Invariant())
            //    );

            //modelBuilder.Entity<ModeratorsGroup>()
            //    .HasOne(a => a.Statistic)
            //    .WithOne(b => b.Owner)
            //    .HasForeignKey<ModeratorsGroupStatistic>(b => b.Owner);
        }


        public override int SaveChanges()
        {
            throw new NotSupportedException();
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            throw new NotSupportedException();
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            //SavingChanges?.Invoke(_savingChanges);
            //await _savingChanges.WaitAsync();

            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SavingChanges?.Invoke(_savingChanges);
            await _savingChanges.WaitAsync();

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
