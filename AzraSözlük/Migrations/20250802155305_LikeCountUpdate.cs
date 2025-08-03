using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzraSözlük.Migrations
{
    /// <inheritdoc />
    public partial class LikeCountUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DislikeCount",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "DislikeCount",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "LikeCount",
                table: "Blogs");

            migrationBuilder.CreateTable(
                name: "UserBlogLikes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    BlogId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Like = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Dislike = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Neutral = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBlogLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserBlogLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBlogLikes_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserCommentLikes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    UserId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    CommentId = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    Like = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Dislike = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    Neutral = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCommentLikes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCommentLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCommentLikes_Comments_CommentId",
                        column: x => x.CommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserBlogLikes_BlogId",
                table: "UserBlogLikes",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBlogLikes_Id",
                table: "UserBlogLikes",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBlogLikes_UserId",
                table: "UserBlogLikes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommentLikes_CommentId",
                table: "UserCommentLikes",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCommentLikes_Id",
                table: "UserCommentLikes",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserCommentLikes_UserId",
                table: "UserCommentLikes",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserBlogLikes");

            migrationBuilder.DropTable(
                name: "UserCommentLikes");

            migrationBuilder.AddColumn<int>(
                name: "DislikeCount",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Comments",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DislikeCount",
                table: "Blogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LikeCount",
                table: "Blogs",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
