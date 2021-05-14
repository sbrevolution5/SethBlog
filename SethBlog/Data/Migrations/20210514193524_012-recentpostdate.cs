using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SethBlog.Data.Migrations
{
    public partial class _012recentpostdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LatestPostDate",
                table: "Blog",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LatestPostDate",
                table: "Blog");
        }
    }
}
