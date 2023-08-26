using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAudioTalesServer.Migrations
{
    public partial class stories : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedBooks_Books_BookId",
                table: "PurchasedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedBooks_Users_UserId",
                table: "PurchasedBooks");

            migrationBuilder.CreateTable(
                name: "Stories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AudioData = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    BookId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stories_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Stories_BookId",
                table: "Stories",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedBooks_Books_BookId",
                table: "PurchasedBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedBooks_Users_UserId",
                table: "PurchasedBooks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedBooks_Books_BookId",
                table: "PurchasedBooks");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchasedBooks_Users_UserId",
                table: "PurchasedBooks");

            migrationBuilder.DropTable(
                name: "Stories");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedBooks_Books_BookId",
                table: "PurchasedBooks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchasedBooks_Users_UserId",
                table: "PurchasedBooks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
