﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBModels
{
    public class BlogContext : IdentityDbContext<User>
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Commentary> Commentaries { get; set; }
        public DbSet<PostEdit> PostsEdits { get; set; }
        public DbSet<CommentaryEdit> CommentaryEdits { get; set; }
        public DbSet<UserRuleViolation> UserRuleViolations { get; set; }
        public DbSet<ProfileStatus> ProfilesStatuses { get; set; }
        public DbSet<Profile> ProfilesInfos { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<ViewStatistic> ViewStatistics { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
            Database.EnsureCreated();
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
        }
    }
}
