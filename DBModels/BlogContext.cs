﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace DBModels
{
    public class BlogContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Commentary> Commentaries { get; set; }
        public DbSet<PostEdit> PostsEdits { get; set; }
        public DbSet<CommentaryEdit> CommentaryEdits { get; set; }
        public DbSet<Violation> UserRuleViolations { get; set; }
        public DbSet<ProfileStatus> ProfilesStatuses { get; set; }
        public DbSet<Profile> ProfilesInfos { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ViewStatistic> ViewStatistics { get; set; }
        public DbSet<UserAction> UsersActions { get; set; }
        public DbSet<ModeratorsGroup> ModeratorsGroups { get; set; }
        public DbSet<TokenMetadata> TokenMetadatas { get; set; }
        public DbSet<EntityToCheck<Post>> PostsToCheck { get; set; }
        public DbSet<EntityToCheck<Commentary>> CommentariesToCheck { get; set; }
        public DbSet<EntityToCheck<Profile>> ProfilesToCheck { get; set; }
        public DbSet<EntityToCheck<PostEdit>> PostEditsToCheck { get; set; }
        public IEnumerable<IEntityToCheck> EntitiesToCheck =>
            new Enumerable<IEntityToCheck>()
            {
                PostEditsToCheck,
                PostsToCheck,
                ProfilesToCheck,
                CommentariesToCheck
            };
        public DbSet<ModerationInfo> ModerationInfos { get; set; }
        public DbSet<ModeratorsGroupStatistic> ModeratorsGroupStatistics { get; set; }
        
        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Commentary>()
                .HasOne(c => c.Author)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Post>()
                .HasOne(p => p.ViewStatistic)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Commentary>()
                .HasOne(p => p.ViewStatistic)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Profile>()
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
    }
}
