using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAudioTalesServer.Migrations
{
    public partial class catetorytpye : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PurchaseType",
                table: "PurchasedBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "BookCategory",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurchaseType",
                table: "PurchasedBooks");

            migrationBuilder.DropColumn(
                name: "BookCategory",
                table: "Books");
        }
    }
}
