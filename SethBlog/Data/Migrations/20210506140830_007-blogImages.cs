using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SethBlog.Data.Migrations
{
    public partial class _007blogImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "BlogImage",
                table: "Blog",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Blog",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogImage",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Blog");
        }
    }
}
