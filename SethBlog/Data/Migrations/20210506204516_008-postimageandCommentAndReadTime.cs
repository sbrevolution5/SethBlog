using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace SethBlog.Data.Migrations
{
    public partial class _008postimageandCommentAndReadTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "Post",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "PostImage",
                table: "Post",
                type: "bytea",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReadTime",
                table: "Post",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Comment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PostId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comment_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comment_PostId",
                table: "Comment",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comment");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "PostImage",
                table: "Post");

            migrationBuilder.DropColumn(
                name: "ReadTime",
                table: "Post");
        }
    }
}
