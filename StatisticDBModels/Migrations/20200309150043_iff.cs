using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace StatisticDBModels.Migrations
{
    public partial class iff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BlogDaysStatistic",
                columns: table => new
                {
                    Day = table.Column<DateTime>(nullable: false),
                    PostsCount = table.Column<int>(nullable: false),
                    CommentariesCount = table.Column<int>(nullable: false),
                    UsersCount = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogDaysStatistic", x => x.Day);
                });

            migrationBuilder.CreateTable(
                name: "CommentariesViewStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllViews = table.Column<int>(nullable: false),
                    RegisteredUserViews = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommentariesViewStatistic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostsViewStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllViews = table.Column<int>(nullable: false),
                    RegisteredUserViews = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostsViewStatistic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProfilesViewStatistic",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AllViews = table.Column<int>(nullable: false),
                    RegisteredUserViews = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfilesViewStatistic", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserWithStateCount",
                columns: table => new
                {
                    Day = table.Column<DateTime>(nullable: false),
                    Count = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    BlogDayStatisticDay = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserWithStateCount", x => new { x.Day, x.Count });
                    table.ForeignKey(
                        name: "FK_UserWithStateCount_BlogDaysStatistic_BlogDayStatisticDay",
                        column: x => x.BlogDayStatisticDay,
                        principalTable: "BlogDaysStatistic",
                        principalColumn: "Day",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserWithStateCount_BlogDayStatisticDay",
                table: "UserWithStateCount",
                column: "BlogDayStatisticDay");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommentariesViewStatistic");

            migrationBuilder.DropTable(
                name: "PostsViewStatistic");

            migrationBuilder.DropTable(
                name: "ProfilesViewStatistic");

            migrationBuilder.DropTable(
                name: "UserWithStateCount");

            migrationBuilder.DropTable(
                name: "BlogDaysStatistic");
        }
    }
}
