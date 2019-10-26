using Microsoft.AspNetCore.Identity;
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
        public DbSet<ProfileInfo> ProfilesInfos { get; set; }

        public DbSet<ProfileReport> ProfilesReports { get; set; }
        public DbSet<PostReport> PostsReports { get; set; }
        public DbSet<CommentaryReport> CommentariesReports { get; set; }

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
        }
    }
}
