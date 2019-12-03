using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DBModels.Migrations
{
    public partial class iiii : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModerationInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    State = table.Column<int>(nullable: false),
                    StateReasoning = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModerationInfos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ModeratorsGroups",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModeratorsGroups", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfilesStatuses",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    State = table.Column<int>(nullable: false),
                    StateReason = table.Column<string>(nullable: true),
                    BannedTill = table.Column<DateTime>(nullable: true),
                    LastPasswordRestoreAttempt = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilesStatuses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TokenMetadatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsUsedOrExpired = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TokenMetadatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ViewStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegistredUserViews = table.Column<int>(nullable: false),
                    TotalViews = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ViewStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    StatusId = table.Column<int>(nullable: false),
                    ModeratorsGroupId = table.Column<int>(nullable: true),
                    ModeratorsInChargeGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_ModeratorsGroups_ModeratorsGroupId",
                        column: x => x.ModeratorsGroupId,
                        principalTable: "ModeratorsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_ModeratorsGroups_ModeratorsInChargeGroupId",
                        column: x => x.ModeratorsInChargeGroupId,
                        principalTable: "ModeratorsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_ProfilesStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ProfilesStatuses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    AuthorId = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    BodyPreview = table.Column<string>(nullable: false),
                    ViewStatisticId = table.Column<int>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ModerationInfoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Posts_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_ModerationInfos_ModerationInfoId",
                        column: x => x.ModerationInfoId,
                        principalTable: "ModerationInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Posts_ViewStatistics_ViewStatisticId",
                        column: x => x.ViewStatisticId,
                        principalTable: "ViewStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfilesInfos",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    Gender = table.Column<int>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    About = table.Column<string>(nullable: true),
                    ViewStatisticId = table.Column<int>(nullable: false),
                    AuthorForeignKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilesInfos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfilesInfos_AspNetUsers_AuthorForeignKey",
                        column: x => x.AuthorForeignKey,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfilesInfos_ViewStatistics_ViewStatisticId",
                        column: x => x.ViewStatisticId,
                        principalTable: "ViewStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Commentaries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<string>(nullable: false),
                    CreationTime = table.Column<DateTime>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    Body = table.Column<string>(nullable: false),
                    ViewStatisticId = table.Column<int>(nullable: false),
                    IsHidden = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Commentaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Commentaries_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commentaries_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Commentaries_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Commentaries_ViewStatistics_ViewStatisticId",
                        column: x => x.ViewStatisticId,
                        principalTable: "ViewStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostsEdits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<string>(nullable: false),
                    Reason = table.Column<string>(nullable: false),
                    EditTime = table.Column<DateTime>(nullable: false),
                    NewTitle = table.Column<string>(nullable: false),
                    NewBody = table.Column<string>(nullable: false),
                    NewBodyPreview = table.Column<string>(nullable: false),
                    PostId = table.Column<int>(nullable: true),
                    ModerationInfoId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsEdits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostsEdits_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsEdits_ModerationInfos_ModerationInfoId",
                        column: x => x.ModerationInfoId,
                        principalTable: "ModerationInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsEdits_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostsToCheck",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false),
                    AssignationTime = table.Column<DateTime>(nullable: true),
                    ResolvingTime = table.Column<DateTime>(nullable: true),
                    CheckReason = table.Column<int>(nullable: false),
                    AssignedModeratorId = table.Column<string>(nullable: true),
                    EntityOwnerId = table.Column<string>(nullable: true),
                    ModeratorsGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsToCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostsToCheck_AspNetUsers_AssignedModeratorId",
                        column: x => x.AssignedModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostsToCheck_Posts_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostsToCheck_AspNetUsers_EntityOwnerId",
                        column: x => x.EntityOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostsToCheck_ModeratorsGroups_ModeratorsGroupId",
                        column: x => x.ModeratorsGroupId,
                        principalTable: "ModeratorsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfilesToCheck",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false),
                    AssignationTime = table.Column<DateTime>(nullable: true),
                    ResolvingTime = table.Column<DateTime>(nullable: true),
                    CheckReason = table.Column<int>(nullable: false),
                    AssignedModeratorId = table.Column<string>(nullable: true),
                    EntityOwnerId = table.Column<string>(nullable: true),
                    ModeratorsGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilesToCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfilesToCheck_AspNetUsers_AssignedModeratorId",
                        column: x => x.AssignedModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfilesToCheck_ProfilesInfos_EntityId",
                        column: x => x.EntityId,
                        principalTable: "ProfilesInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfilesToCheck_AspNetUsers_EntityOwnerId",
                        column: x => x.EntityOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProfilesToCheck_ModeratorsGroups_ModeratorsGroupId",
                        column: x => x.ModeratorsGroupId,
                        principalTable: "ModeratorsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentariesToCheck",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false),
                    AssignationTime = table.Column<DateTime>(nullable: true),
                    ResolvingTime = table.Column<DateTime>(nullable: true),
                    CheckReason = table.Column<int>(nullable: false),
                    AssignedModeratorId = table.Column<string>(nullable: true),
                    EntityOwnerId = table.Column<string>(nullable: true),
                    ModeratorsGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentariesToCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentariesToCheck_AspNetUsers_AssignedModeratorId",
                        column: x => x.AssignedModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentariesToCheck_Commentaries_EntityId",
                        column: x => x.EntityId,
                        principalTable: "Commentaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentariesToCheck_AspNetUsers_EntityOwnerId",
                        column: x => x.EntityOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CommentariesToCheck_ModeratorsGroups_ModeratorsGroupId",
                        column: x => x.ModeratorsGroupId,
                        principalTable: "ModeratorsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommentaryEdits",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AuthorId = table.Column<string>(nullable: false),
                    Reason = table.Column<string>(nullable: false),
                    EditTime = table.Column<DateTime>(nullable: false),
                    CommentaryId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentaryEdits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommentaryEdits_AspNetUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommentaryEdits_Commentaries_CommentaryId",
                        column: x => x.CommentaryId,
                        principalTable: "Commentaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReporterId = table.Column<string>(nullable: false),
                    ObjectOwnerId = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CommentaryObjectId = table.Column<int>(nullable: true),
                    PostObjectId = table.Column<int>(nullable: true),
                    ProfileObjectId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Commentaries_CommentaryObjectId",
                        column: x => x.CommentaryObjectId,
                        principalTable: "Commentaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_ObjectOwnerId",
                        column: x => x.ObjectOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_Posts_PostObjectId",
                        column: x => x.PostObjectId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_ProfilesInfos_ProfileObjectId",
                        column: x => x.ProfileObjectId,
                        principalTable: "ProfilesInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reports_AspNetUsers_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRuleViolations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReporterId = table.Column<string>(nullable: false),
                    ObjectOwnerId = table.Column<string>(nullable: true),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CommentaryObjectId = table.Column<int>(nullable: true),
                    PostObjectId = table.Column<int>(nullable: true),
                    ProfileObjectId = table.Column<int>(nullable: true),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRuleViolations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRuleViolations_Commentaries_CommentaryObjectId",
                        column: x => x.CommentaryObjectId,
                        principalTable: "Commentaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRuleViolations_AspNetUsers_ObjectOwnerId",
                        column: x => x.ObjectOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRuleViolations_Posts_PostObjectId",
                        column: x => x.PostObjectId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRuleViolations_ProfilesInfos_ProfileObjectId",
                        column: x => x.ProfileObjectId,
                        principalTable: "ProfilesInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRuleViolations_AspNetUsers_ReporterId",
                        column: x => x.ReporterId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersActions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ActionType = table.Column<int>(nullable: false),
                    ActionDate = table.Column<DateTime>(nullable: false),
                    CommentaryObjectId = table.Column<int>(nullable: true),
                    PostObjectId = table.Column<int>(nullable: true),
                    ProfileObjectId = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersActions_Commentaries_CommentaryObjectId",
                        column: x => x.CommentaryObjectId,
                        principalTable: "Commentaries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersActions_Posts_PostObjectId",
                        column: x => x.PostObjectId,
                        principalTable: "Posts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersActions_ProfilesInfos_ProfileObjectId",
                        column: x => x.ProfileObjectId,
                        principalTable: "ProfilesInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsersActions_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostEditsToCheck",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EntityId = table.Column<int>(nullable: false),
                    AddTime = table.Column<DateTime>(nullable: false),
                    AssignationTime = table.Column<DateTime>(nullable: true),
                    ResolvingTime = table.Column<DateTime>(nullable: true),
                    CheckReason = table.Column<int>(nullable: false),
                    AssignedModeratorId = table.Column<string>(nullable: true),
                    EntityOwnerId = table.Column<string>(nullable: true),
                    ModeratorsGroupId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostEditsToCheck", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostEditsToCheck_AspNetUsers_AssignedModeratorId",
                        column: x => x.AssignedModeratorId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostEditsToCheck_PostsEdits_EntityId",
                        column: x => x.EntityId,
                        principalTable: "PostsEdits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostEditsToCheck_AspNetUsers_EntityOwnerId",
                        column: x => x.EntityOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PostEditsToCheck_ModeratorsGroups_ModeratorsGroupId",
                        column: x => x.ModeratorsGroupId,
                        principalTable: "ModeratorsGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ModeratorsGroupId",
                table: "AspNetUsers",
                column: "ModeratorsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ModeratorsInChargeGroupId",
                table: "AspNetUsers",
                column: "ModeratorsInChargeGroupId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_StatusId",
                table: "AspNetUsers",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaries_AuthorId",
                table: "Commentaries",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaries_PostId",
                table: "Commentaries",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaries_UserId",
                table: "Commentaries",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Commentaries_ViewStatisticId",
                table: "Commentaries",
                column: "ViewStatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentariesToCheck_AssignedModeratorId",
                table: "CommentariesToCheck",
                column: "AssignedModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentariesToCheck_EntityId",
                table: "CommentariesToCheck",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentariesToCheck_EntityOwnerId",
                table: "CommentariesToCheck",
                column: "EntityOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentariesToCheck_ModeratorsGroupId",
                table: "CommentariesToCheck",
                column: "ModeratorsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentaryEdits_AuthorId",
                table: "CommentaryEdits",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_CommentaryEdits_CommentaryId",
                table: "CommentaryEdits",
                column: "CommentaryId");

            migrationBuilder.CreateIndex(
                name: "IX_PostEditsToCheck_AssignedModeratorId",
                table: "PostEditsToCheck",
                column: "AssignedModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostEditsToCheck_EntityId",
                table: "PostEditsToCheck",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PostEditsToCheck_EntityOwnerId",
                table: "PostEditsToCheck",
                column: "EntityOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PostEditsToCheck_ModeratorsGroupId",
                table: "PostEditsToCheck",
                column: "ModeratorsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_AuthorId",
                table: "Posts",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ModerationInfoId",
                table: "Posts",
                column: "ModerationInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_ViewStatisticId",
                table: "Posts",
                column: "ViewStatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsEdits_AuthorId",
                table: "PostsEdits",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsEdits_ModerationInfoId",
                table: "PostsEdits",
                column: "ModerationInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsEdits_PostId",
                table: "PostsEdits",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsToCheck_AssignedModeratorId",
                table: "PostsToCheck",
                column: "AssignedModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsToCheck_EntityId",
                table: "PostsToCheck",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsToCheck_EntityOwnerId",
                table: "PostsToCheck",
                column: "EntityOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_PostsToCheck_ModeratorsGroupId",
                table: "PostsToCheck",
                column: "ModeratorsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesInfos_AuthorForeignKey",
                table: "ProfilesInfos",
                column: "AuthorForeignKey",
                unique: true,
                filter: "[AuthorForeignKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesInfos_ViewStatisticId",
                table: "ProfilesInfos",
                column: "ViewStatisticId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesToCheck_AssignedModeratorId",
                table: "ProfilesToCheck",
                column: "AssignedModeratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesToCheck_EntityId",
                table: "ProfilesToCheck",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesToCheck_EntityOwnerId",
                table: "ProfilesToCheck",
                column: "EntityOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfilesToCheck_ModeratorsGroupId",
                table: "ProfilesToCheck",
                column: "ModeratorsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_CommentaryObjectId",
                table: "Reports",
                column: "CommentaryObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ObjectOwnerId",
                table: "Reports",
                column: "ObjectOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_PostObjectId",
                table: "Reports",
                column: "PostObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ProfileObjectId",
                table: "Reports",
                column: "ProfileObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ReporterId",
                table: "Reports",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRuleViolations_CommentaryObjectId",
                table: "UserRuleViolations",
                column: "CommentaryObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRuleViolations_ObjectOwnerId",
                table: "UserRuleViolations",
                column: "ObjectOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRuleViolations_PostObjectId",
                table: "UserRuleViolations",
                column: "PostObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRuleViolations_ProfileObjectId",
                table: "UserRuleViolations",
                column: "ProfileObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRuleViolations_ReporterId",
                table: "UserRuleViolations",
                column: "ReporterId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersActions_CommentaryObjectId",
                table: "UsersActions",
                column: "CommentaryObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersActions_PostObjectId",
                table: "UsersActions",
                column: "PostObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersActions_ProfileObjectId",
                table: "UsersActions",
                column: "ProfileObjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersActions_UserId",
                table: "UsersActions",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "CommentariesToCheck");

            migrationBuilder.DropTable(
                name: "CommentaryEdits");

            migrationBuilder.DropTable(
                name: "PostEditsToCheck");

            migrationBuilder.DropTable(
                name: "PostsToCheck");

            migrationBuilder.DropTable(
                name: "ProfilesToCheck");

            migrationBuilder.DropTable(
                name: "Reports");

            migrationBuilder.DropTable(
                name: "TokenMetadatas");

            migrationBuilder.DropTable(
                name: "UserRuleViolations");

            migrationBuilder.DropTable(
                name: "UsersActions");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "PostsEdits");

            migrationBuilder.DropTable(
                name: "Commentaries");

            migrationBuilder.DropTable(
                name: "ProfilesInfos");

            migrationBuilder.DropTable(
                name: "Posts");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "ModerationInfos");

            migrationBuilder.DropTable(
                name: "ViewStatistics");

            migrationBuilder.DropTable(
                name: "ModeratorsGroups");

            migrationBuilder.DropTable(
                name: "ProfilesStatuses");
        }
    }
}
