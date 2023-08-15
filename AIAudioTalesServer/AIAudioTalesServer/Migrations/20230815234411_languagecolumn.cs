using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIAudioTalesServer.Migrations
{
    public partial class languagecolumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Language",
                table: "PurchasedBooks",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "PurchasedBooks");
        }
    }
}
