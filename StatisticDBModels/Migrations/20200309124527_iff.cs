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
                    CommentaryCount = table.Column<int>(nullable: false),
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlogDaysStatistic");

            migrationBuilder.DropTable(
                name: "CommentariesViewStatistic");

            migrationBuilder.DropTable(
                name: "PostsViewStatistic");

            migrationBuilder.DropTable(
                name: "ProfilesViewStatistic");
        }
    }
}
