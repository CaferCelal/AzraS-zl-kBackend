using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AzraSözlük.Migrations
{
    /// <inheritdoc />
    public partial class V1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogModel_AspNetUsers_UserId",
                table: "BlogModel");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogModel_TagModel_TagId",
                table: "BlogModel");

            migrationBuilder.DropForeignKey(
                name: "FK_TagModel_AspNetUsers_UserId",
                table: "TagModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TagModel",
                table: "TagModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BlogModel",
                table: "BlogModel");

            migrationBuilder.RenameTable(
                name: "TagModel",
                newName: "Tags");

            migrationBuilder.RenameTable(
                name: "BlogModel",
                newName: "Blogs");

            migrationBuilder.RenameIndex(
                name: "IX_TagModel_UserId",
                table: "Tags",
                newName: "IX_Tags_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogModel_UserId",
                table: "Blogs",
                newName: "IX_Blogs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogModel_TagId",
                table: "Blogs",
                newName: "IX_Blogs_TagId");

            migrationBuilder.AddColumn<DateTime>(
                name: "GeneratedDate",
                table: "Tags",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "GeneratedDate",
                table: "Blogs",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tags",
                table: "Tags",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", maxLength: 36, nullable: false),
                    BlogId = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 5000, nullable: false),
                    GeneratedDate = table.Column<DateTime>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Id",
                table: "Tags",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_Id",
                table: "Blogs",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_BlogId",
                table: "Comments",
                column: "BlogId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Id",
                table: "Comments",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UserId",
                table: "Comments",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_AspNetUsers_UserId",
                table: "Blogs",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Tags_TagId",
                table: "Blogs",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AspNetUsers_UserId",
                table: "Tags",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_AspNetUsers_UserId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Tags_TagId",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AspNetUsers_UserId",
                table: "Tags");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tags",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Id",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blogs",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_Id",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "GeneratedDate",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "GeneratedDate",
                table: "Blogs");

            migrationBuilder.RenameTable(
                name: "Tags",
                newName: "TagModel");

            migrationBuilder.RenameTable(
                name: "Blogs",
                newName: "BlogModel");

            migrationBuilder.RenameIndex(
                name: "IX_Tags_UserId",
                table: "TagModel",
                newName: "IX_TagModel_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Blogs_UserId",
                table: "BlogModel",
                newName: "IX_BlogModel_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Blogs_TagId",
                table: "BlogModel",
                newName: "IX_BlogModel_TagId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TagModel",
                table: "TagModel",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BlogModel",
                table: "BlogModel",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogModel_AspNetUsers_UserId",
                table: "BlogModel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogModel_TagModel_TagId",
                table: "BlogModel",
                column: "TagId",
                principalTable: "TagModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TagModel_AspNetUsers_UserId",
                table: "TagModel",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
