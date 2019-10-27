﻿// <auto-generated />
using System;
using DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DBModels.Migrations
{
    [DbContext(typeof(BlogContext))]
    [Migration("20191027095033_ffff")]
    partial class ffff
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DBModels.Commentary", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<string>("Body")
                        .IsRequired();

                    b.Property<DateTime>("CreationTime");

                    b.Property<int>("PostId");

                    b.Property<string>("UserId");

                    b.Property<int>("ViewStatisticId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("PostId");

                    b.HasIndex("UserId");

                    b.HasIndex("ViewStatisticId");

                    b.ToTable("Commentaries");
                });

            modelBuilder.Entity("DBModels.CommentaryEdit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CommentaryId");

                    b.Property<string>("EditAuthorId")
                        .IsRequired();

                    b.Property<DateTime>("EditTime");

                    b.Property<string>("Reason");

                    b.HasKey("Id");

                    b.HasIndex("CommentaryId");

                    b.HasIndex("EditAuthorId");

                    b.ToTable("CommentaryEdits");
                });

            modelBuilder.Entity("DBModels.Post", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<string>("Body")
                        .IsRequired();

                    b.Property<DateTime>("CreationTime");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("ViewStatisticId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ViewStatisticId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DBModels.PostEdit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("EditAuthorId")
                        .IsRequired();

                    b.Property<DateTime>("EditTime");

                    b.Property<int?>("PostId");

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("EditAuthorId");

                    b.HasIndex("PostId");

                    b.ToTable("PostsEdits");
                });

            modelBuilder.Entity("DBModels.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("About");

                    b.Property<int>("Gender");

                    b.Property<string>("Image");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<int>("ViewStatisticId");

                    b.HasKey("Id");

                    b.HasIndex("ViewStatisticId");

                    b.ToTable("ProfilesInfos");
                });

            modelBuilder.Entity("DBModels.ProfileStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("BannedTill");

                    b.Property<DateTime?>("LastPasswordRestoreAttempt");

                    b.Property<int>("State");

                    b.Property<string>("StateReason");

                    b.HasKey("Id");

                    b.ToTable("ProfilesStatuses");
                });

            modelBuilder.Entity("DBModels.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CommentaryId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<int?>("PostId");

                    b.Property<int?>("ProfileId");

                    b.Property<int>("ReportObjectId");

                    b.Property<string>("ReportObjectOwnerId");

                    b.Property<int>("ReportObjectType");

                    b.Property<string>("ReporterId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CommentaryId");

                    b.HasIndex("PostId");

                    b.HasIndex("ProfileId");

                    b.HasIndex("ReportObjectOwnerId");

                    b.HasIndex("ReporterId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("DBModels.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<int>("ProfileId");

                    b.Property<string>("SecurityStamp");

                    b.Property<int>("StatusId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("ProfileId");

                    b.HasIndex("StatusId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("DBModels.UserRuleViolation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("ObjectId");

                    b.Property<int>("ObjectType");

                    b.Property<string>("ReporterId");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("ReporterId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRuleViolations");
                });

            modelBuilder.Entity("DBModels.ViewStatistic", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("RegistredUserViews");

                    b.Property<int>("TotalViews");

                    b.HasKey("Id");

                    b.ToTable("ViewStatistics");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("DBModels.Commentary", b =>
                {
                    b.HasOne("DBModels.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("DBModels.Post", "Post")
                        .WithMany()
                        .HasForeignKey("PostId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.User")
                        .WithMany("Commentaries")
                        .HasForeignKey("UserId");

                    b.HasOne("DBModels.ViewStatistic", "ViewStatistic")
                        .WithMany()
                        .HasForeignKey("ViewStatisticId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DBModels.CommentaryEdit", b =>
                {
                    b.HasOne("DBModels.Commentary")
                        .WithMany("Edits")
                        .HasForeignKey("CommentaryId");

                    b.HasOne("DBModels.User", "EditAuthor")
                        .WithMany()
                        .HasForeignKey("EditAuthorId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DBModels.Post", b =>
                {
                    b.HasOne("DBModels.User", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.ViewStatistic", "ViewStatistic")
                        .WithMany()
                        .HasForeignKey("ViewStatisticId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DBModels.PostEdit", b =>
                {
                    b.HasOne("DBModels.User", "EditAuthor")
                        .WithMany()
                        .HasForeignKey("EditAuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.Post")
                        .WithMany("Edits")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("DBModels.Profile", b =>
                {
                    b.HasOne("DBModels.ViewStatistic", "ViewStatistic")
                        .WithMany()
                        .HasForeignKey("ViewStatisticId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DBModels.Report", b =>
                {
                    b.HasOne("DBModels.Commentary")
                        .WithMany("Reports")
                        .HasForeignKey("CommentaryId");

                    b.HasOne("DBModels.Post")
                        .WithMany("Reports")
                        .HasForeignKey("PostId");

                    b.HasOne("DBModels.Profile")
                        .WithMany("Reports")
                        .HasForeignKey("ProfileId");

                    b.HasOne("DBModels.User", "ReportObjectOwner")
                        .WithMany("Reports")
                        .HasForeignKey("ReportObjectOwnerId");

                    b.HasOne("DBModels.User", "Reporter")
                        .WithMany("ReportedReports")
                        .HasForeignKey("ReporterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DBModels.User", b =>
                {
                    b.HasOne("DBModels.Profile", "Profile")
                        .WithMany()
                        .HasForeignKey("ProfileId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.ProfileStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DBModels.UserRuleViolation", b =>
                {
                    b.HasOne("DBModels.User", "Reporter")
                        .WithMany("ReportedViolations")
                        .HasForeignKey("ReporterId");

                    b.HasOne("DBModels.User", "User")
                        .WithMany("Violations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("DBModels.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("DBModels.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("DBModels.User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
