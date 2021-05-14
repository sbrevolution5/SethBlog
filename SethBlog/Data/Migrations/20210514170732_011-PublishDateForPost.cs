using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SethBlog.Data.Migrations
{
    public partial class _011PublishDateForPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PublishedDate",
                table: "Post",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublishedDate",
                table: "Post");
        }
    }
}
