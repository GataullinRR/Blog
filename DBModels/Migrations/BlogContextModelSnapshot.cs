﻿// <auto-generated />
using System;
using DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DBModels.Migrations
{
    [DbContext(typeof(BlogContext))]
    partial class BlogContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
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

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsHidden");

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

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<int?>("CommentaryId");

                    b.Property<DateTime>("EditTime");

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("CommentaryId");

                    b.ToTable("CommentaryEdits");
                });

            modelBuilder.Entity("DBModels.EntityToCheck<DBModels.Commentary>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AddTime");

                    b.Property<DateTime?>("AssignationTime");

                    b.Property<string>("AssignedModeratorId");

                    b.Property<int>("CheckReason");

                    b.Property<int>("EntityId");

                    b.Property<string>("EntityOwnerId");

                    b.Property<int?>("ModeratorsGroupId");

                    b.Property<DateTime?>("ResolvingTime");

                    b.HasKey("Id");

                    b.HasIndex("AssignedModeratorId");

                    b.HasIndex("EntityId");

                    b.HasIndex("EntityOwnerId");

                    b.HasIndex("ModeratorsGroupId");

                    b.ToTable("CommentariesToCheck");
                });

            modelBuilder.Entity("DBModels.EntityToCheck<DBModels.Post>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AddTime");

                    b.Property<DateTime?>("AssignationTime");

                    b.Property<string>("AssignedModeratorId");

                    b.Property<int>("CheckReason");

                    b.Property<int>("EntityId");

                    b.Property<string>("EntityOwnerId");

                    b.Property<int?>("ModeratorsGroupId");

                    b.Property<DateTime?>("ResolvingTime");

                    b.HasKey("Id");

                    b.HasIndex("AssignedModeratorId");

                    b.HasIndex("EntityId");

                    b.HasIndex("EntityOwnerId");

                    b.HasIndex("ModeratorsGroupId");

                    b.ToTable("PostsToCheck");
                });

            modelBuilder.Entity("DBModels.EntityToCheck<DBModels.PostEdit>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AddTime");

                    b.Property<DateTime?>("AssignationTime");

                    b.Property<string>("AssignedModeratorId");

                    b.Property<int>("CheckReason");

                    b.Property<int>("EntityId");

                    b.Property<string>("EntityOwnerId");

                    b.Property<int?>("ModeratorsGroupId");

                    b.Property<DateTime?>("ResolvingTime");

                    b.HasKey("Id");

                    b.HasIndex("AssignedModeratorId");

                    b.HasIndex("EntityId");

                    b.HasIndex("EntityOwnerId");

                    b.HasIndex("ModeratorsGroupId");

                    b.ToTable("PostEditsToCheck");
                });

            modelBuilder.Entity("DBModels.EntityToCheck<DBModels.Profile>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("AddTime");

                    b.Property<DateTime?>("AssignationTime");

                    b.Property<string>("AssignedModeratorId");

                    b.Property<int>("CheckReason");

                    b.Property<int>("EntityId");

                    b.Property<string>("EntityOwnerId");

                    b.Property<int?>("ModeratorsGroupId");

                    b.Property<DateTime?>("ResolvingTime");

                    b.HasKey("Id");

                    b.HasIndex("AssignedModeratorId");

                    b.HasIndex("EntityId");

                    b.HasIndex("EntityOwnerId");

                    b.HasIndex("ModeratorsGroupId");

                    b.ToTable("ProfilesToCheck");
                });

            modelBuilder.Entity("DBModels.ModerationInfo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("State");

                    b.Property<string>("StateReasoning");

                    b.HasKey("Id");

                    b.ToTable("ModerationInfos");
                });

            modelBuilder.Entity("DBModels.ModeratorsGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("Id");

                    b.ToTable("ModeratorsGroups");
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

                    b.Property<string>("BodyPreview")
                        .IsRequired();

                    b.Property<DateTime>("CreationTime");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ModerationInfoId");

                    b.Property<string>("Title")
                        .IsRequired();

                    b.Property<int>("ViewStatisticId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ModerationInfoId");

                    b.HasIndex("ViewStatisticId");

                    b.ToTable("Posts");
                });

            modelBuilder.Entity("DBModels.PostEdit", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("AuthorId")
                        .IsRequired();

                    b.Property<DateTime>("EditTime");

                    b.Property<int>("ModerationInfoId");

                    b.Property<string>("NewBody")
                        .IsRequired();

                    b.Property<string>("NewBodyPreview")
                        .IsRequired();

                    b.Property<string>("NewTitle")
                        .IsRequired();

                    b.Property<int?>("PostId");

                    b.Property<string>("Reason")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ModerationInfoId");

                    b.HasIndex("PostId");

                    b.ToTable("PostsEdits");
                });

            modelBuilder.Entity("DBModels.Profile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("About");

                    b.Property<string>("AuthorForeignKey");

                    b.Property<int>("Gender");

                    b.Property<string>("Image");

                    b.Property<DateTime>("RegistrationDate");

                    b.Property<int>("ViewStatisticId");

                    b.HasKey("Id");

                    b.HasIndex("AuthorForeignKey")
                        .IsUnique()
                        .HasFilter("[AuthorForeignKey] IS NOT NULL");

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

                    b.Property<int?>("CommentaryObjectId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("ObjectOwnerId");

                    b.Property<int?>("PostObjectId");

                    b.Property<int?>("ProfileObjectId");

                    b.Property<string>("ReporterId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CommentaryObjectId");

                    b.HasIndex("ObjectOwnerId");

                    b.HasIndex("PostObjectId");

                    b.HasIndex("ProfileObjectId");

                    b.HasIndex("ReporterId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("DBModels.TokenMetadata", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsUsedOrExpired");

                    b.HasKey("Id");

                    b.ToTable("TokenMetadatas");
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

                    b.Property<int?>("ModeratorsGroupId");

                    b.Property<int?>("ModeratorsInChargeGroupId");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<int>("StatusId");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("ModeratorsGroupId");

                    b.HasIndex("ModeratorsInChargeGroupId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.HasIndex("StatusId");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("DBModels.UserAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("ActionDate");

                    b.Property<int>("ActionType");

                    b.Property<int?>("CommentaryObjectId");

                    b.Property<int?>("PostObjectId");

                    b.Property<int?>("ProfileObjectId");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CommentaryObjectId");

                    b.HasIndex("PostObjectId");

                    b.HasIndex("ProfileObjectId");

                    b.HasIndex("UserId");

                    b.ToTable("UsersActions");
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

            modelBuilder.Entity("DBModels.Violation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CommentaryObjectId");

                    b.Property<DateTime>("CreationDate");

                    b.Property<string>("Description")
                        .IsRequired();

                    b.Property<string>("ObjectOwnerId");

                    b.Property<int?>("PostObjectId");

                    b.Property<int?>("ProfileObjectId");

                    b.Property<string>("ReporterId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("CommentaryObjectId");

                    b.HasIndex("ObjectOwnerId");

                    b.HasIndex("PostObjectId");

                    b.HasIndex("ProfileObjectId");

                    b.HasIndex("ReporterId");

                    b.ToTable("UserRuleViolations");
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
                    b.HasOne("DBModels.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.Commentary")
                        .WithMany("Edits")
                        .HasForeignKey("CommentaryId");
                });

            modelBuilder.Entity("DBModels.EntityToCheck<DBModels.Commentary>", b =>
                {
                    b.HasOne("DBModels.User", "AssignedModerator")
                        .WithMany()
                        .HasForeignKey("AssignedModeratorId");

                    b.HasOne("DBModels.Commentary", "Entity")
                        .WithMany()
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.User", "EntityOwner")
                        .WithMany()
                        .HasForeignKey("EntityOwnerId");

                    b.HasOne("DBModels.ModeratorsGroup")
                        .WithMany("CommentariesToCheck")
                        .HasForeignKey("ModeratorsGroupId");
                });

            modelBuilder.Entity("DBModels.EntityToCheck<DBModels.Post>", b =>
                {
                    b.HasOne("DBModels.User", "AssignedModerator")
                        .WithMany()
                        .HasForeignKey("AssignedModeratorId");

                    b.HasOne("DBModels.Post", "Entity")
                        .WithMany()
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.User", "EntityOwner")
                        .WithMany()
                        .HasForeignKey("EntityOwnerId");

                    b.HasOne("DBModels.ModeratorsGroup")
                        .WithMany("PostsToCheck")
                        .HasForeignKey("ModeratorsGroupId");
                });

            modelBuilder.Entity("DBModels.EntityToCheck<DBModels.PostEdit>", b =>
                {
                    b.HasOne("DBModels.User", "AssignedModerator")
                        .WithMany()
                        .HasForeignKey("AssignedModeratorId");

                    b.HasOne("DBModels.PostEdit", "Entity")
                        .WithMany()
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.User", "EntityOwner")
                        .WithMany()
                        .HasForeignKey("EntityOwnerId");

                    b.HasOne("DBModels.ModeratorsGroup")
                        .WithMany("PostEditsToCheck")
                        .HasForeignKey("ModeratorsGroupId");
                });

            modelBuilder.Entity("DBModels.EntityToCheck<DBModels.Profile>", b =>
                {
                    b.HasOne("DBModels.User", "AssignedModerator")
                        .WithMany()
                        .HasForeignKey("AssignedModeratorId");

                    b.HasOne("DBModels.Profile", "Entity")
                        .WithMany()
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.User", "EntityOwner")
                        .WithMany()
                        .HasForeignKey("EntityOwnerId");

                    b.HasOne("DBModels.ModeratorsGroup")
                        .WithMany("ProfilesToCheck")
                        .HasForeignKey("ModeratorsGroupId");
                });

            modelBuilder.Entity("DBModels.Post", b =>
                {
                    b.HasOne("DBModels.User", "Author")
                        .WithMany("Posts")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.ModerationInfo", "ModerationInfo")
                        .WithMany()
                        .HasForeignKey("ModerationInfoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.ViewStatistic", "ViewStatistic")
                        .WithMany()
                        .HasForeignKey("ViewStatisticId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DBModels.PostEdit", b =>
                {
                    b.HasOne("DBModels.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.ModerationInfo", "ModerationInfo")
                        .WithMany()
                        .HasForeignKey("ModerationInfoId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("DBModels.Post", "Post")
                        .WithMany("Edits")
                        .HasForeignKey("PostId");
                });

            modelBuilder.Entity("DBModels.Profile", b =>
                {
                    b.HasOne("DBModels.User", "Author")
                        .WithOne("Profile")
                        .HasForeignKey("DBModels.Profile", "AuthorForeignKey");

                    b.HasOne("DBModels.ViewStatistic", "ViewStatistic")
                        .WithMany()
                        .HasForeignKey("ViewStatisticId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("DBModels.Report", b =>
                {
                    b.HasOne("DBModels.Commentary", "CommentaryObject")
                        .WithMany("Reports")
                        .HasForeignKey("CommentaryObjectId");

                    b.HasOne("DBModels.User", "ObjectOwner")
                        .WithMany("Reports")
                        .HasForeignKey("ObjectOwnerId");

                    b.HasOne("DBModels.Post", "PostObject")
                        .WithMany("Reports")
                        .HasForeignKey("PostObjectId");

                    b.HasOne("DBModels.Profile", "ProfileObject")
                        .WithMany("Reports")
                        .HasForeignKey("ProfileObjectId");

                    b.HasOne("DBModels.User", "Reporter")
                        .WithMany("ReportedReports")
                        .HasForeignKey("ReporterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DBModels.User", b =>
                {
                    b.HasOne("DBModels.ModeratorsGroup", "ModeratorsGroup")
                        .WithMany("Moderators")
                        .HasForeignKey("ModeratorsGroupId");

                    b.HasOne("DBModels.ModeratorsGroup", "ModeratorsInChargeGroup")
                        .WithMany("TargetUsers")
                        .HasForeignKey("ModeratorsInChargeGroupId");

                    b.HasOne("DBModels.ProfileStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("DBModels.UserAction", b =>
                {
                    b.HasOne("DBModels.Commentary", "CommentaryObject")
                        .WithMany()
                        .HasForeignKey("CommentaryObjectId");

                    b.HasOne("DBModels.Post", "PostObject")
                        .WithMany()
                        .HasForeignKey("PostObjectId");

                    b.HasOne("DBModels.Profile", "ProfileObject")
                        .WithMany()
                        .HasForeignKey("ProfileObjectId");

                    b.HasOne("DBModels.User")
                        .WithMany("Actions")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("DBModels.Violation", b =>
                {
                    b.HasOne("DBModels.Commentary", "CommentaryObject")
                        .WithMany("Violations")
                        .HasForeignKey("CommentaryObjectId");

                    b.HasOne("DBModels.User", "ObjectOwner")
                        .WithMany("Violations")
                        .HasForeignKey("ObjectOwnerId");

                    b.HasOne("DBModels.Post", "PostObject")
                        .WithMany("Violations")
                        .HasForeignKey("PostObjectId");

                    b.HasOne("DBModels.Profile", "ProfileObject")
                        .WithMany("Violations")
                        .HasForeignKey("ProfileObjectId");

                    b.HasOne("DBModels.User", "Reporter")
                        .WithMany("ReportedViolations")
                        .HasForeignKey("ReporterId")
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
