using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DBModels
{
    public class BlogContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Commentary> Commentaries { get; set; }

        public BlogContext(DbContextOptions<BlogContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Commentary>()
            //    .HasOne(c => c.Author)
            //    .WithMany()
            //    .HasForeignKey(c => c.Author)
            //    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Commentaries)
            //    .WithMany()
            //    .HasForeignKey(u => u.Commentaries)
            //    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Commentary>()
    .HasOne(c => c.Author)
    .WithMany()
    .OnDelete(DeleteBehavior.Restrict);
            //modelBuilder.Entity<User>()
            //    .HasOne(u => u.Commentaries)
            //    .WithMany()
            //    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
